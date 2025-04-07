using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Models.Constants;
using PegsBase.Models.Enums;
using PegsBase.Models.ViewModels;
using System.Diagnostics;


namespace PegsBase.Controllers
{
    [Authorize(Roles =
        Roles.Surveyor + "," +
        Roles.MineSurveyor + "," +
        Roles.SurveyAnalyst + "," +
        Roles.Master)]
    public class SurveyNotesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public SurveyNotesController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _dbContext = db;
            _env = env;
        }

        public async Task<IActionResult> Index(string? search, SurveyNoteType? type, string? level, bool? status, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.SurveyNotes.AsQueryable();

            bool canViewAll = RoleGroups.CanViewAllNotes.Any(role => User.IsInRole(role));
            if (!canViewAll)
            {
                query = query.Where(n => n.IsSigned == true);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keywords = search
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var keyword in keywords)
                {
                    query = query.Where(n => 
                    EF.Functions.ILike(n.Title, $"%{keyword}%") ||
                    EF.Functions.ILike(n.Locality, $"%{keyword}%") ||
                    EF.Functions.ILike(n.Level, $"%{keyword}%"));
                }
            }

            if (type.HasValue)
                query = query.Where(n => n.NoteType == type.Value);

            if (!string.IsNullOrWhiteSpace(level))
                query = query.Where(n => n.Level == level);

            if (status.HasValue)
                query = query.Where(n => n.IsSigned == status.Value);

            var total = await query.CountAsync();
            var notes = await query
                .OrderByDescending(n => n.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new SurveyNotesListViewModel
            {
                Notes = notes,
                Page = page,
                PageSize = pageSize,
                Total = total,
                FilterSearch = search,
                FilterType = type,
                FilterLevel = level,
                FilterIsSigned = status
            };

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> GetNotes(string search, SurveyNoteType? type, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.SurveyNotes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(n => n.Title.Contains(search));

            if (type.HasValue)
                query = query.Where(n => n.NoteType == type.Value);

            var total = await query.CountAsync();
            var notes = await query
                .OrderByDescending(n => n.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new SurveyNotesListViewModel
            {
                Notes = notes,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return PartialView("_SurveyNotesList", viewModel); // ✅ Must be PartialView
        }



        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(SurveyNoteUploadViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var fileName = $"{Guid.NewGuid()}.pdf";
            var folder = model.IsSigned ? "Signed" : "InProgress";
            var instanceFolder = Path.Combine(_env.WebRootPath, "SurveyNotes", folder);
            Directory.CreateDirectory(instanceFolder);

            var filePath = Path.Combine(instanceFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            // Default thumbnail to a placeholder for local dev
            var thumbnailFileName = Path.ChangeExtension(fileName, ".jpg");
            var thumbnailFolder = Path.Combine(_env.WebRootPath, "SurveyNotes", "Thumbnails", folder);
            Directory.CreateDirectory(thumbnailFolder);
            var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);
            var relativeThumbnailPath = Path.Combine("SurveyNotes", "Thumbnails", folder, thumbnailFileName);

            #if DEBUG
            // Use a placeholder image in dev mode
            var placeholder = Path.Combine(_env.WebRootPath, "images", "placeholder-thumbnail.png");
            System.IO.File.Copy(placeholder, thumbnailPath, overwrite: true);
            #else
             // If in production, generate using pdftoppm
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "pdftoppm",
                            Arguments = $"-jpeg -f 1 -singlefile \"{filePath}\" \"{Path.Combine(thumbnailFolder, Path.GetFileNameWithoutExtension(fileName))}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    await process.WaitForExitAsync();
                }
                catch (Exception ex)
                {
        
                }
            #endif

            var note = new SurveyNote
            {
                Title = model.Title,
                UploadedBy = model.Surveyor,
                Level = model.Level,
                Locality = model.Locality,
                FilePath = Path.Combine("SurveyNotes", folder, fileName),
                UploadedAt = DateTime.UtcNow,
                IsSigned = model.IsSigned,
                IsVerified = false,
                NoteType = model.NoteType,
                ThumbnailPath = relativeThumbnailPath
            };

            _dbContext.SurveyNotes.Add(note);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            SurveyNote? surveyNote = _dbContext.SurveyNotes.Find(id);

            if (surveyNote == null)
            {
                return NotFound();
            }

            return View(surveyNote);
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpPost]
        public async Task<IActionResult> Edit(SurveyNote obj)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ",
                    ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => $"{x.Key}: {x.Value.Errors[0].ErrorMessage}")
                );
                TempData["Debug"] = $"ModelState invalid: {errors}";
                return View(obj);
            }

            var existingNote = await _dbContext.SurveyNotes.FirstOrDefaultAsync(n => n.Id == obj.Id);

            if (existingNote == null)
            {
                TempData["Debug"] = "Note not found";
                return NotFound();
            }

            existingNote.Title = obj.Title;
            existingNote.Level = obj.Level;
            existingNote.Locality = obj.Locality;
            existingNote.NoteType = obj.NoteType;
            existingNote.UploadedBy = obj.UploadedBy;
            existingNote.UploadedAt = obj.UploadedAt;
            existingNote.IsAbandoned = obj.IsAbandoned;
            existingNote.AbandonmentReason = obj.AbandonmentReason;
            existingNote.IsSigned = obj.IsSigned;
            existingNote.UploadedAt = DateTime.SpecifyKind(obj.UploadedAt, DateTimeKind.Utc);

            await _dbContext.SaveChangesAsync();
            TempData["Success"] = "Note updated successfully.";
            return RedirectToAction("Index");
        }



        [Authorize(Roles =
            Roles.Master + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            SurveyNote? pegEntry = _dbContext.SurveyNotes.Find(id);

            if (pegEntry == null)
            {
                return NotFound();
            }

            return View(pegEntry);
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [Authorize(Roles = Roles.Master + "," + Roles.MineSurveyor + "," + Roles.SurveyAnalyst)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid note ID.";
                return RedirectToAction("Index");
            }

            var note = await _dbContext.SurveyNotes.FirstOrDefaultAsync(n => n.Id == id);
            if (note == null)
            {
                TempData["Error"] = "Survey note not found.";
                return RedirectToAction("Index");
            }

            // Delete the PDF file
            var filePath = Path.Combine(_env.WebRootPath, note.FilePath ?? "");
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Delete the thumbnail file
            var thumbnailPath = Path.Combine(_env.WebRootPath, note.ThumbnailPath ?? "");
            if (System.IO.File.Exists(thumbnailPath))
            {
                System.IO.File.Delete(thumbnailPath);
            }

            _dbContext.SurveyNotes.Remove(note);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = $"Note '{note.Title}' deleted successfully.";
            return RedirectToAction("Index");
        }

    }
}
