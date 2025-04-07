using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.Constants;
using PegsBase.Models.Enums;
using PegsBase.Models.MinePlans;
using PegsBase.Models.ViewModels;
using System.Diagnostics;

namespace PegsBase.Controllers
{
    [Authorize(Roles = Roles.Surveyor + "," + Roles.MineSurveyor + "," + Roles.SurveyAnalyst + "," + Roles.Master)]
    public class MinePlansController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public MinePlansController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        public async Task<IActionResult> Index(string? search, MinePlanType? type, string? level, bool? isSigned, bool? isSuperseded, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.MinePlans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keywords = search
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var keyword in keywords)
                {
                    query = query.Where(p =>
                        EF.Functions.ILike(p.PlanName, $"%{keyword}%") ||
                        EF.Functions.ILike(p.Level, $"%{keyword}%") ||
                        EF.Functions.ILike(p.Locality, $"%{keyword}%"));
                }
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(p => p.Level == level);
            }

            if (type.HasValue)
            {
                query = query.Where(p => p.PlanType == type.Value);
            }

            if (isSigned.HasValue)
            {
                query = query.Where(p => p.IsSigned == isSigned.Value);
            }

            if (isSuperseded.HasValue)
            {
                query = query.Where(p => p.IsSuperseded == isSuperseded.Value);
            }

            var total = await query.CountAsync();
            var plans = await query
                .OrderByDescending(p => p.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new MinePlanListViewModel
            {
                Plans = plans,
                Total = total,
                Page = page,
                PageSize = pageSize,
                FilterSearch = search,
                FilterLevel = level,
                FilterType = type,
                FilterIsSigned = isSigned,
                FilterIsSuperseded = isSuperseded
            };

            return View(model);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(MinePlanUploadViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var fileName = $"{Guid.NewGuid()}.pdf";
            var folder = model.IsSigned ? "Signed" : "InProgress";
            var fullFolder = Path.Combine(_env.WebRootPath, "MinePlans", folder);
            Directory.CreateDirectory(fullFolder);

            var filePath = Path.Combine(fullFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            var thumbnailFileName = Path.ChangeExtension(fileName, ".jpg");
            var thumbnailFolder = Path.Combine(_env.WebRootPath, "MinePlans", "Thumbnails", folder);
            Directory.CreateDirectory(thumbnailFolder);
            var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);
            var relativeThumbnailPath = Path.Combine("MinePlans", "Thumbnails", folder, thumbnailFileName);

#if DEBUG
            var placeholder = Path.Combine(_env.WebRootPath, "images", "placeholder-thumbnail.png");
            System.IO.File.Copy(placeholder, thumbnailPath, true);
#else
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
            catch { }
#endif

            var plan = new MinePlan
            {
                PlanName = model.PlanName,
                Level = model.Level,
                Locality = model.Locality,
                Scale = model.Scale,
                PlanType = model.PlanType,
                IsSigned = model.IsSigned,
                UploadedAt = DateTime.UtcNow,
                IsSuperseded = false,
                FilePath = Path.Combine("MinePlans", folder, fileName),
                ThumbnailPath = relativeThumbnailPath
            };

            _dbContext.MinePlans.Add(plan);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _dbContext.MinePlans.FindAsync(id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MinePlan updated)
        {
            if (!ModelState.IsValid) return View(updated);

            var existing = await _dbContext.MinePlans.FirstOrDefaultAsync(p => p.Id == updated.Id);
            if (existing == null) return NotFound();

            existing.PlanName = updated.PlanName;
            existing.Level = updated.Level;
            existing.Locality = updated.Locality;
            existing.Scale = updated.Scale;
            existing.IsSigned = updated.IsSigned;
            existing.IsSuperseded = updated.IsSuperseded;
            existing.SupersededReference = updated.SupersededReference;
            existing.PlanType = updated.PlanType;
            existing.UploadedAt = updated.UploadedAt;

            await _dbContext.SaveChangesAsync();
            TempData["Success"] = "Mine plan updated successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _dbContext.MinePlans.FindAsync(id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _dbContext.MinePlans.FindAsync(id);
            if (plan == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, plan.FilePath ?? "");
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            var thumbPath = Path.Combine(_env.WebRootPath, plan.ThumbnailPath ?? "");
            if (System.IO.File.Exists(thumbPath)) System.IO.File.Delete(thumbPath);

            _dbContext.MinePlans.Remove(plan);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = $"Deleted mine plan: {plan.PlanName}";
            return RedirectToAction("Index");
        }
    }
}