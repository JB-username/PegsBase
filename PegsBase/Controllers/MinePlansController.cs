using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.MinePlans;

namespace PegsBase.Controllers
{
    [Authorize]
    public class MinePlansController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public MinePlansController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        public async Task<IActionResult> Index(
                    string? search,
                    int? planTypeId,
                    string? level,
                    bool? isSigned,
                    bool? isSuperseded,
                    int page = 1,
                    int pageSize = 10
                )
        {
            // 1) Eager-load the navs
            var query = _dbContext.MinePlans
                .Include(p => p.Level)
                .Include(p => p.Locality)
                .Include(p => p.PlanType)
                .AsQueryable();

            // 2) Text search across PlanName, Level.Name, Locality.Name
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keywords = search
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var kw in keywords)
                {
                    query = query.Where(p =>
                        EF.Functions.ILike(p.PlanName, $"%{kw}%") ||
                        EF.Functions.ILike(p.Level.Name, $"%{kw}%") ||
                        EF.Functions.ILike(p.Locality.Name, $"%{kw}%")
                    );
                }
            }

            // 3) Filter by level name
            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(p => p.Level.Name == level);
            }

            // 4) Filter by PlanTypeId (your new lookup table)
            if (planTypeId.HasValue)
            {
                query = query.Where(p => p.PlanTypeId == planTypeId.Value);
            }

            // 5) Signed / In-Progress
            if (isSigned.HasValue)
            {
                query = query.Where(p => p.IsSigned == isSigned.Value);
            }

            // 6) Superseded or not
            if (isSuperseded.HasValue)
            {
                query = query.Where(p => p.IsSuperseded == isSuperseded.Value);
            }

            // 7) Paging + execution
            var total = await query.CountAsync();
            var plans = await query
                .OrderByDescending(p => p.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 8) Load your dropdown list of PlanTypes
            var planTypeOptions = await _dbContext.PlanTypes
                .OrderBy(pt => pt.Name)
                .Select(pt => new SelectListItem
                {
                    Text = pt.Name,
                    Value = pt.Id.ToString(),
                    Selected = (pt.Id == planTypeId)
                })
                .ToListAsync();

            // 9) Build the ViewModel
            var model = new MinePlanListViewModel
            {
                Plans = plans,
                Total = total,
                Page = page,
                PageSize = pageSize,
                FilterSearch = search,
                FilterLevel = level,
                FilterPlanTypeId = planTypeId,        // now an int?
                FilterIsSigned = isSigned,
                FilterIsSuperseded = isSuperseded,
                PlanTypeOptions = planTypeOptions    // your new dropdown source
            };

            return View(model);
        }


        private async Task PopulateDropDownsAsync(MinePlanUploadViewModel vm)
        {
            vm.LevelOptions = await _dbContext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new SelectListItem(l.Name, l.Id.ToString()))
                .ToListAsync();

            vm.LocalityOptions = await _dbContext.Localities
                .OrderBy(loc => loc.Name)
                .Select(loc => new SelectListItem(loc.Name, loc.Id.ToString()))
                .ToListAsync();

            var planTypes = await _dbContext.PlanTypes
                                    .OrderBy(pt => pt.Name)
                                    .ToListAsync();

            vm.PlanTypeOptions = planTypes
                .Select(pt => new SelectListItem
                {
                    Text = pt.Name,
                    Value = pt.Id.ToString(),
                    Selected = (pt.Id == vm.PlanTypeId)   // <— use vm.PlanTypeId, not a local var
                })
                .ToList();
        }

        private async Task PopulateDropDownsAsync(MinePlanEditViewModel vm)
        {
            vm.LevelOptions = await _dbContext.Levels
                .OrderBy(l => l.Name)
                .Select(l => new SelectListItem(l.Name, l.Id.ToString()))
                .ToListAsync();

            vm.LocalityOptions = await _dbContext.Localities
                .OrderBy(loc => loc.Name)
                .Select(loc => new SelectListItem(loc.Name, loc.Id.ToString()))
                .ToListAsync();

            var planTypes = await _dbContext.PlanTypes
                                    .OrderBy(pt => pt.Name)
                                    .ToListAsync();

            vm.PlanTypeOptions = planTypes
                .Select(pt => new SelectListItem
                {
                    Text = pt.Name,
                    Value = pt.Id.ToString(),
                    Selected = (pt.Id == vm.PlanTypeId)
                })
                .ToList();

            await PopulateDropDownsAsync((MinePlanEditViewModel)vm);
        }


        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var vm = new MinePlanUploadViewModel();
            await PopulateDropDownsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(MinePlanUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(model);
                return View(model);
            }
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
                PlanTypeId = model.PlanTypeId.Value,
                Scale = model.Scale,
                IsSigned = model.IsSigned,
                IsSuperseded = false,

                LevelId = model.LevelId,
                LocalityId = model.LocalityId,

                UploadedAt = DateTime.UtcNow,
                FilePath = Path.Combine("MinePlans", folder, fileName),
                ThumbnailPath = relativeThumbnailPath
            };

            _dbContext.MinePlans.Add(plan);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet, Authorize(Policy = "SurveyDepartment")]
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _dbContext.MinePlans.FindAsync(id);
            if (plan == null) return NotFound();

            var vm = new MinePlanEditViewModel
            {
                Id = plan.Id,
                PlanName = plan.PlanName,
                PlanTypeId = plan.PlanTypeId,
                Scale = plan.Scale,
                IsSigned = plan.IsSigned,
                IsSuperseded = plan.IsSuperseded,
                SupersededReference = plan.SupersededReference,
                LevelId = plan.LevelId,
                LocalityId = plan.LocalityId ?? 0,
                UploadedAt = plan.UploadedAt,
                // SurveyorId     = plan.SurveyorId ?? ""
            };
            await PopulateDropDownsAsync(vm);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "SurveyDepartment")]
        public async Task<IActionResult> Edit(MinePlanEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(vm);
                return View(vm);
            }

            var existing = await _dbContext.MinePlans.FindAsync(vm.Id);

            if (existing == null) return NotFound();

            existing.PlanName = vm.PlanName;
            existing.PlanTypeId = vm.PlanTypeId;
            existing.Scale = vm.Scale;
            existing.IsSigned = vm.IsSigned;
            existing.IsSuperseded = vm.IsSuperseded;
            existing.SupersededReference = vm.SupersededReference;

            existing.LevelId = vm.LevelId;
            existing.LocalityId = vm.LocalityId;

            existing.UploadedAt = DateTime.SpecifyKind(vm.UploadedAt, DateTimeKind.Utc);

            await _dbContext.SaveChangesAsync();
            TempData["Success"] = "Mine plan updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, Authorize(Policy = "SurveyManagers")]
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _dbContext.MinePlans.FindAsync(id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Policy = "SurveyManagers")]
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





        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var plan = await _dbContext.MinePlans.FindAsync(id);
            if (plan == null || string.IsNullOrEmpty(plan.FilePath))
                return NotFound();

            // FilePath is something like "MinePlans/Signed/abc123.pdf"
            var absolute = Path.Combine(_env.WebRootPath, plan.FilePath);
            if (!System.IO.File.Exists(absolute))
                return NotFound();

            // force a download of the PDF
            var fileName = Path.GetFileName(absolute);
            const string contentType = "application/pdf";
            return PhysicalFile(absolute, contentType, fileName);
        }

    }
}