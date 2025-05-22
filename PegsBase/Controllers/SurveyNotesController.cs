using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.Constants;
using PegsBase.Models.Enums;
using PegsBase.Models.SurveyNotes;
using System.Diagnostics;
using System.Threading.Tasks;


namespace PegsBase.Controllers
{
    [Authorize]
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
            var query = _dbContext.SurveyNotes
                .Include(n => n.Level)
                .Include(n => n.Locality)
                .Include(n => n.UploadedBy)
                .AsQueryable();

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
                    EF.Functions.ILike(n.Locality.Name, $"%{keyword}%") ||
                    EF.Functions.ILike(n.Level.Name, $"%{keyword}%"));
                }
            }

            if (type.HasValue)
                query = query.Where(n => n.NoteType == type.Value);

            if (!string.IsNullOrWhiteSpace(level))
                query = query.Where(n => n.Level.Name == level);

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


        private async Task PopulateDropDownsAsync(SurveyNoteUploadViewModel vm)
        {
            vm.Levels = await _dbContext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new SelectListItem(l.Name, l.Id.ToString()))
                .ToListAsync();

            vm.Localities = await _dbContext.Localities
                .OrderBy(loc => loc.Name)
                .Select(loc => new SelectListItem(loc.Name, loc.Id.ToString()))
                .ToListAsync();

            vm.Surveyors = await _dbContext.Users
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem(u.UserName, u.Id.ToString()))
                .ToListAsync();
        }

        private async Task PopulateDropDownsAsync(SurveyNoteEditViewModel vm)
        {
            vm.LevelOptions = await _dbContext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new SelectListItem(l.Name, l.Id.ToString()))
                .ToListAsync();
            vm.LocalityOptions = await _dbContext.Localities
                .OrderBy(loc => loc.Name)
                .Select(loc => new SelectListItem(loc.Name, loc.Id.ToString()))
                .ToListAsync();
            vm.SurveyorOptions = await _dbContext.Users
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem(u.UserName, u.Id))
                .ToListAsync();
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


        [Authorize(Policy = "SurveyDepartment")]
        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var vm = new SurveyNoteUploadViewModel();
            await PopulateDropDownsAsync(vm);
            return View(vm);
        }

        [Authorize(Policy = "SurveyDepartment")]
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

            var note = new SurveyNoteModel
            {
                Title = model.Title,
                NoteType = model.NoteType,

                UploadedAt = DateTime.UtcNow,
                UploadedById = model.SurveyorId == null ? string.Empty : model.SurveyorId,           // ✅ string FK to ApplicationUser.Id
                
                LevelId = model.LevelId,             // ✅ int FK to Level
                LocalityId = model.LocalityId,       // ✅ int FK to Locality
                
                IsSigned = model.IsSigned,
                IsVerified = false,
                
                FilePath = Path.Combine("SurveyNotes", folder, fileName),
                ThumbnailPath = relativeThumbnailPath
            };

            _dbContext.SurveyNotes.Add(note);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet, Authorize(Policy = "SurveyDepartment")]
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _dbContext.SurveyNotes.FindAsync(id);
            if (note == null) return NotFound();

            var vm = new SurveyNoteEditViewModel
            {
                Id = note.Id,
                Title = note.Title,
                NoteType = note.NoteType,
                IsSigned = note.IsSigned,
                IsAbandoned = note.IsAbandoned,
                AbandonmentReason = note.AbandonmentReason,
                UploadedAt = note.UploadedAt,
                LevelId = note.LevelId,
                LocalityId = note.LocalityId,
                SurveyorId = note.UploadedById
            };
            await PopulateDropDownsAsync(vm);
            return View(vm);
        }

        [HttpPost, Authorize(Policy = "SurveyDepartment")]
        public async Task<IActionResult> Edit(SurveyNoteEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ",
                    ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => $"{x.Key}: {x.Value.Errors[0].ErrorMessage}")
                );
                TempData["Debug"] = $"ModelState invalid: {errors}";
                await PopulateDropDownsAsync(vm);
                return View(vm);
            }

            var existingNote = await _dbContext.SurveyNotes.FirstOrDefaultAsync(n => n.Id == vm.Id);

            if (existingNote == null)
            {
                TempData["Debug"] = "Note not found";
                return NotFound();
            }

            existingNote.Title = vm.Title;
            existingNote.LevelId = vm.LevelId;
            existingNote.LocalityId = vm.LocalityId;
            existingNote.NoteType = vm.NoteType;
            existingNote.UploadedById = vm.SurveyorId;
            existingNote.UploadedAt = vm.UploadedAt;
            existingNote.IsAbandoned = vm.IsAbandoned;
            existingNote.AbandonmentReason = vm.AbandonmentReason;
            existingNote.IsSigned = vm.IsSigned;
            existingNote.UploadedAt = DateTime.SpecifyKind(vm.UploadedAt, DateTimeKind.Utc);

            await _dbContext.SaveChangesAsync();
            TempData["Success"] = "Note updated successfully.";
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Policy = "SurveyDepartment")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id.Value == 0)
            {
                return NotFound();
            }

            var note = await _dbContext.SurveyNotes
                .Include(n => n.Level)
                .Include(n => n.Locality)
                .Include(n => n.UploadedBy)
                .FirstOrDefaultAsync(n => n.Id == id.Value);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        [Authorize(Policy = "SurveyDepartment")]
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
