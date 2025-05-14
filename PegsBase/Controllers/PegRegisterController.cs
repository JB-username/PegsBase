using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Models.Constants;
using PegsBase.Models.Enums;
using PegsBase.Models.Identity;
using PegsBase.Models.ViewModels;
using PegsBase.Services.Parsing;
using PegsBase.Services.Parsing.Interfaces;
using PegsBase.Services.PegCalc.Interfaces;
using System.Text;
using Rotativa.AspNetCore;
using PegsBase.Services.Settings;

namespace PegsBase.Controllers
{
    [Authorize]
    public class PegRegisterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPegFileParser _pegFileParser;
        private readonly ICoordinateDatParserService _coordinateDatParserService;
        private readonly IPegCalcService _pegCalcService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapImportModelsToPegs _pegMapper;
        private readonly IImportSettingsService _importSettingsService;

        public PegRegisterController(
            ApplicationDbContext db,
            IPegFileParser pegFileParser,
            ICoordinateDatParserService coordinateDatParserService,
            IPegCalcService pegCalcService,
            UserManager<ApplicationUser> user,
            IMapImportModelsToPegs mapImportModelsToPeg,
            IImportSettingsService importSettingsService)
        {
            _dbContext = db;
            _pegFileParser = pegFileParser;
            _coordinateDatParserService = coordinateDatParserService;
            _pegCalcService = pegCalcService;
            _userManager = user;
            _pegMapper = mapImportModelsToPeg;
            _importSettingsService = importSettingsService;
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.Mrm + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        public async Task<IActionResult> Index(SurveyPointType? filter, string sortOrder)
        {
            ViewBag.Levels = await _dbContext.Levels.OrderBy(l => l.Name).ToListAsync();

            var objPegsList = _dbContext.PegRegister
                .Include(p => p.Level)
                .Include(p => p.Locality)
                .Include(p => p.Surveyor)
                .AsQueryable();

            if (filter.HasValue)
            {
                objPegsList = objPegsList.Where(p => p.PointType == filter.Value);
            }

            switch (sortOrder)
            {
                case "date_asc":
                    objPegsList = objPegsList.OrderBy(p => p.SurveyDate);
                    break;
                case "date_desc":
                    objPegsList = objPegsList.OrderByDescending(p => p.SurveyDate);
                    break;
                case "name_asc":
                    objPegsList = objPegsList.OrderBy(p => p.PegName);
                    break;
                case "name_desc":
                    objPegsList = objPegsList.OrderByDescending(p => p.PegName);
                    break;
                default:
                    objPegsList = objPegsList.OrderByDescending(p => p.SurveyDate);
                    break;
            }

            return View(await objPegsList.ToListAsync());
        }

        [HttpPost]
        public IActionResult Index(List<int> selectedIds)
        {
            return Content("Selected: " + string.Join(", ", selectedIds ?? new List<int>()));
        }


        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users.ToListAsync();

            ViewBag.Levels = await _dbContext.Levels
                .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
                .ToListAsync();

            ViewBag.Localities = await _dbContext.Localities
                .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
                .ToListAsync();

            ViewBag.Surveyors = users
                .Select(u => new SelectListItem { Value = u.Id, Text = u.DisplayName })
                .ToList();

            var peg = new PegRegister
            {
                PegName = string.Empty,
                SurveyDate = DateOnly.FromDateTime(DateTime.Today)
            };

            return View(peg); // Return just the model (not wrapped in a view model)
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PegRegister peg)
        {
            TempData["DebugSurveyor"] = peg.SurveyorId ?? "(null)";

            if (string.IsNullOrWhiteSpace(peg.SurveyorId) && string.IsNullOrWhiteSpace(peg.SurveyorNameText))
            {
                ModelState.AddModelError("SurveyorId", "Please select a surveyor or enter a historical name.");
            }

            if (!string.IsNullOrWhiteSpace(peg.SurveyorId))
            {
                peg.SurveyorNameText = null;
            }

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                var users = await _userManager.Users.ToListAsync();

                ViewBag.Levels = await _dbContext.Levels
                    .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
                    .ToListAsync();

                ViewBag.Localities = await _dbContext.Localities
                    .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
                    .ToListAsync();

                ViewBag.Surveyors = users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.DisplayName
                    }).ToList();

                return View(peg);
            }

            _dbContext.PegRegister.Add(peg);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Peg created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var peg = await _dbContext.PegRegister
                .Include(p => p.Level)
                .Include(p => p.Locality)
                .Include(p => p.Surveyor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (peg == null) return NotFound();

            var users = await _userManager.Users.ToListAsync();

            ViewBag.Levels = await _dbContext.Levels
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToListAsync();

            ViewBag.Localities = await _dbContext.Localities
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToListAsync();

            ViewBag.Surveyors = users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.DisplayName
                }).ToList();

            return View(peg);
        }


        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PegRegister peg, string? filter)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                var users = await _userManager.Users.ToListAsync();

                ViewBag.Levels = await _dbContext.Levels
                    .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
                    .ToListAsync();

                ViewBag.Localities = await _dbContext.Localities
                    .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
                    .ToListAsync();

                ViewBag.Surveyors = users
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.DisplayName })
                    .ToList();

                return View(peg);
            }

            _dbContext.PegRegister.Update(peg);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Peg updated successfully.";
            return RedirectToAction("Index", new { filter });
        }


        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpGet]
        public IActionResult Delete(int? id, SurveyPointType? filter)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            if (filter.HasValue)
            {
                TempData["LastFilter"] = filter.Value;
            }

            PegRegister? pegEntry = _dbContext.PegRegister.Find(id);

            if (pegEntry == null)
            {
                return NotFound();
            }

            return View(pegEntry);
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var pegToDelete = _dbContext.PegRegister.FirstOrDefault(p => p.Id == id);

            if (pegToDelete == null)
            {
                TempData["Error"] = "Peg Not Found.";
                return RedirectToAction("Index");
            }

            _dbContext.PegRegister.Remove(pegToDelete);
            _dbContext.SaveChanges();

            TempData["Success"] = $"Peg {pegToDelete.PegName} deleted successfully!";
            return RedirectToAction("Index");

        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst)]
        [HttpPost]
        public IActionResult DeleteSelected(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "No pegs were selected for deletion.";
                return RedirectToAction("Index");
            }

            var pegsToDelete = _dbContext.PegRegister
                .Where(p => selectedIds.Contains(p.Id))
                .ToList();

            _dbContext.PegRegister.RemoveRange(pegsToDelete);
            _dbContext.SaveChanges();

            TempData["Success"] = $"{pegsToDelete.Count} peg(s) deleted successfully.";
            return RedirectToAction("Index");
        }


        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpPost]
        public IActionResult ExportSelectedToCsvPlans(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "Please select at least one peg to export.";
                return RedirectToAction("Index");
            }

            var selectedPegs = _dbContext.PegRegister
                .Where(p => selectedIds.Contains(p.Id))
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("PegName,YCoord,XCoord,ZCoord,GradeElevation");

            foreach (var peg in selectedPegs)
            {
                // Export data AS-IS
                csv.AppendLine($"{peg.PegName},{peg.YCoord},{peg.XCoord},{peg.ZCoord},{peg.GradeElevation}");
            }

            var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"SelectedPegs_Plans_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return File(fileBytes, "text/csv", fileName);
        }



        [Authorize(Roles =
        Roles.Master + "," +
        Roles.Admin + "," +
        Roles.MineSurveyor + "," +
        Roles.SurveyAnalyst + "," +
        Roles.Surveyor)]
        [HttpPost]
        public IActionResult ExportSelectedToCsvInstrument(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "Please select at least one peg to export.";
                return RedirectToAction("Index");
            }

            var selectedPegs = _dbContext.PegRegister
                .Where(p => selectedIds.Contains(p.Id))
                .ToList();

            var settings = _importSettingsService.GetSettings();

            var csv = new StringBuilder();
            csv.AppendLine("PegName,YCoord,XCoord,ZCoord,GradeElevation");

            foreach (var peg in selectedPegs)
            {
                decimal x = peg.XCoord;
                decimal y = peg.YCoord;

                // 🔥 Reverse the AppSettings logic in reverse order
                // 1. Reverse InvertX and InvertY (signs first)
                if (settings.InvertX)
                {
                    x = -x;
                }

                if (settings.InvertY)
                {
                    y = -y;
                }

                // 2. Reverse SwapXY
                if (settings.SwapXY)
                {
                    var temp = x;
                    x = y;
                    y = temp;
                }

                csv.AppendLine($"{peg.PegName},{y},{x},{peg.ZCoord},{peg.GradeElevation}");
            }

            var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"SelectedPegs_Instrument_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return File(fileBytes, "text/csv", fileName);
        }



        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpPost]
        public IActionResult ExportSelectedToDxf(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "Please select at least one peg to export.";
                return RedirectToAction("Index");
            }

            var pegs = _dbContext.PegRegister.Where(p => selectedIds.Contains(p.Id)).ToList();
            var sb = new StringBuilder();

            sb.AppendLine("0");
            sb.AppendLine("SECTION");
            sb.AppendLine("2");
            sb.AppendLine("HEADER");
            sb.AppendLine("0");
            sb.AppendLine("ENDSEC");

            sb.AppendLine("0");
            sb.AppendLine("SECTION");
            sb.AppendLine("2");
            sb.AppendLine("ENTITIES");

            foreach (var peg in pegs)
            {
                // POINT - plotted on 'Pegs' layer
                sb.AppendLine("0");
                sb.AppendLine("POINT");
                sb.AppendLine("8");
                sb.AppendLine("Pegs");
                sb.AppendLine("10");
                sb.AppendLine(peg.YCoord.ToString("F3"));
                sb.AppendLine("20");
                sb.AppendLine(peg.XCoord.ToString("F3"));
                sb.AppendLine("30");
                sb.AppendLine(peg.ZCoord.ToString("F3"));

                // CIRCLE 1 - radius 0.6
                sb.AppendLine("0");
                sb.AppendLine("CIRCLE");
                sb.AppendLine("8");
                sb.AppendLine("Pegs");
                sb.AppendLine("10");
                sb.AppendLine(peg.YCoord.ToString("F3"));
                sb.AppendLine("20");
                sb.AppendLine(peg.XCoord.ToString("F3"));
                sb.AppendLine("30");
                sb.AppendLine(peg.ZCoord.ToString("F3"));
                sb.AppendLine("40");
                sb.AppendLine("0.6");

                // CIRCLE 2 - radius 0.8
                sb.AppendLine("0");
                sb.AppendLine("CIRCLE");
                sb.AppendLine("8");
                sb.AppendLine("Pegs");
                sb.AppendLine("10");
                sb.AppendLine(peg.YCoord.ToString("F3"));
                sb.AppendLine("20");
                sb.AppendLine(peg.XCoord.ToString("F3"));
                sb.AppendLine("30");
                sb.AppendLine(peg.ZCoord.ToString("F3"));
                sb.AppendLine("40");
                sb.AppendLine("0.8");

                // TEXT label - on separate layer 'PegNames'
                sb.AppendLine("0");
                sb.AppendLine("TEXT");
                sb.AppendLine("8");
                sb.AppendLine("PegNames"); // new layer
                sb.AppendLine("10");
                sb.AppendLine((peg.YCoord + 2.5m).ToString("F3")); // offset so it doesn't sit on the peg
                sb.AppendLine("20");
                sb.AppendLine((peg.XCoord + 0.0m).ToString("F3"));
                sb.AppendLine("30");
                sb.AppendLine(peg.ZCoord.ToString("F3"));
                sb.AppendLine("40");
                sb.AppendLine("0.5"); // text height
                sb.AppendLine("1");
                sb.AppendLine(peg.PegName);
            }

            sb.AppendLine("0");
            sb.AppendLine("ENDSEC");
            sb.AppendLine("0");
            sb.AppendLine("EOF");


            return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/dxf", "SelectedPegs.dxf");
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a CSV file to upload");
                return View();
            }

            using var stream = file.OpenReadStream();
            var importModels = _pegFileParser.Parse(stream); // List<PegRegisterImportModel>

            var settings = _importSettingsService.GetSettings();

            foreach (var model in importModels)
            {
                if (settings.SwapXY)
                {
                    var temp = model.XCoord;
                    model.XCoord = model.YCoord;
                    model.YCoord = temp;
                }

                if (settings.InvertX)
                {
                    model.XCoord = -model.XCoord;
                }

                if (settings.InvertY)
                {
                    model.YCoord = -model.YCoord;
                }
            }

            TempData["ParsedPegs"] = JsonConvert.SerializeObject(importModels);

            return RedirectToAction("Preview");
        }


        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.Mrm + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpGet]
        public IActionResult Preview()
        {
            if (!TempData.TryGetValue("ParsedPegs", out var rawData) || rawData == null)
                return RedirectToAction("Upload");

            var pegs = JsonConvert.DeserializeObject<List<PegRegisterImportModel>>(rawData.ToString());
            return View(pegs); // View now expects PegRegisterImportModel
        }


        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpPost]
        public IActionResult ConfirmImport(string pegs)
        {
            if (string.IsNullOrWhiteSpace(pegs))
                return RedirectToAction("Upload");

            try
            {
                var deserializedPegs = JsonConvert.DeserializeObject<List<PegRegister>>(pegs);

                if (deserializedPegs == null || !deserializedPegs.Any())
                {
                    TempData["Error"] = "No pegs found to import.";
                    return RedirectToAction("Upload");
                }

                _dbContext.PegRegister.AddRange(deserializedPegs);
                _dbContext.SaveChanges();

                TempData["Success"] = $"{deserializedPegs.Count} pegs imported successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Optionally log exception
                TempData["Error"] = "An error occurred while importing pegs.";
                return RedirectToAction("Upload");
            }
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        public IActionResult DownloadTemplate()
        {
            var csv = "PegName,XCoord,YCoord,ZCoord,GradeElevation,Locality,Level,Surveyor,SurveyDate,PointType\n";
            var bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "PegTemplate.csv");
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.Mrm + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpPost]
        public IActionResult PrintSelected(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "No pegs selected to print.";
                return RedirectToAction("Index");
            }

            var pegs = _dbContext.PegRegister
                .Where(p => selectedIds.Contains(p.Id))
                .ToList();

            return View("PrintView", pegs); // Pass data to a dedicated print view
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpGet]
        public IActionResult UploadDat()
        {
            return View(new CoordinateUploadViewModel());
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        [HttpPost]
        public async Task<IActionResult> UploadDat(CoordinateUploadViewModel model)
        {
            if (model.CoordinateFile == null || model.CoordinateFile.Length == 0)
            {
                ModelState.AddModelError("CoordinateFile", "Please select a .dat file.");
                return View(model);
            }

            using var reader = new StreamReader(model.CoordinateFile.OpenReadStream());
            var parsedRows = await _coordinateDatParserService.ParseDatAsync(reader);

            var settings = _importSettingsService.GetSettings();

            foreach (var row in parsedRows)
            {
                if (settings.SwapXY)
                {
                    var temp = row.XCoord;
                    row.XCoord = row.YCoord;
                    row.YCoord = temp;
                }

                if (settings.InvertX)
                {
                    row.XCoord = -row.XCoord;
                }

                if (settings.InvertY)
                {
                    row.YCoord = -row.YCoord;
                }
            }

            model.PreviewRows = parsedRows.Take(2).ToList(); // Only show first 2

            if (!model.PreviewRows.Any())
            {
                ModelState.AddModelError("", "No valid rows were found in the file.");
                return View(model);
            }

            ViewBag.Levels = new SelectList(_dbContext.Levels.OrderBy(l => l.Name), "Id", "Name");
            ViewBag.Localities = new SelectList(_dbContext.Localities.OrderBy(l => l.Name), "Id", "Name");
            ViewBag.Surveyors = new SelectList(
                _dbContext.Users
                    .OrderBy(u => u.LastName)
                    .Select(u => new
                    {
                        u.Id,
                        Name = u.FirstName.Substring(0, 1) + ". " + u.LastName
                    }),
                "Id",
                "Name"
            );
            TempData["RedirectAfterSaveUrl"] = model.RedirectAfterSaveUrl;

            return View("PreviewCoordinate", model);
        }


        [HttpPost]
        public async Task<IActionResult> SaveCoordinatePegs(CoordinateUploadViewModel model)
        {
            if (model.PreviewRows == null || !model.PreviewRows.Any())
            {
                ModelState.AddModelError("", "No pegs to save.");
                return View("PreviewCoordinate", model);
            }

            var rowsToSave = model.PreviewRows.Take(2).ToList();
            int savedCount = 0;
            List<string> duplicatePegNames = new();

            foreach (var preview in rowsToSave)
            {
                if (string.IsNullOrWhiteSpace(preview.PegName)) continue;

                bool exists = _dbContext.PegRegister.Any(p => p.PegName == preview.PegName);
                if (exists)
                {
                    duplicatePegNames.Add(preview.PegName);
                    continue;
                }

                var peg = new PegRegister
                {
                    PegName = preview.PegName,
                    XCoord = preview.XCoord,
                    YCoord = preview.YCoord,
                    ZCoord = preview.ZCoord,
                    GradeElevation = preview.GradeElevation,
                    SurveyorId = preview.SurveyorId ?? "system",
                    LocalityId = preview.LocalityId,
                    SurveyDate = preview.SurveyDate ?? DateOnly.FromDateTime(DateTime.Today),
                    LevelId = preview.LevelId,
                    PointType = preview.Type.GetValueOrDefault(SurveyPointType.Peg)
                };

                _dbContext.PegRegister.Add(peg);
                savedCount++;
            }

            await _dbContext.SaveChangesAsync();

            if (duplicatePegNames.Any())
            {
                TempData["Success"] = $"Uploaded {savedCount} peg(s). Skipped {duplicatePegNames.Count} duplicate(s): {string.Join(", ", duplicatePegNames)}.";
            }
            else
            {
                TempData["Success"] = $"Successfully uploaded {savedCount} coordinate peg(s).";
            }


            return RedirectToAction("Index", "PegRegister");
        }

        [Authorize(Roles =
            Roles.Master + "," +
            Roles.Admin + "," +
            Roles.MineSurveyor + "," +
            Roles.SurveyAnalyst + "," +
            Roles.Surveyor)]
        public async Task<IActionResult> ViewPeg(int id)
        {
            var peg = await _dbContext.PegRegister
                .Include(p => p.Level)
                .Include(p => p.Locality)
                .Include(p => p.Surveyor)
                .FirstOrDefaultAsync(p => p.Id == id);


            if (peg == null)
                return NotFound();

            if (peg.HasPegCalc)
            {
                var rawData = await _dbContext.RawSurveyData
                    .FirstOrDefaultAsync(r => r.ForeSightPeg == peg.PegName);

                if (rawData == null)
                    return View("BasicPegView", peg); // fallback if raw missing

                var viewModel = new PegCalcViewModel
                {
                    Id = peg.Id,

                    ForeSightPeg = rawData.ForeSightPeg,
                    StationPeg = rawData.StationPeg,
                    BackSightPeg = rawData.BackSightPeg,

                    TargetHeightBacksight = rawData.TargetHeightBacksight,
                    TargetHeightForesight = rawData.TargetHeightForesight,
                    InstrumentHeight = rawData.InstrumentHeight,
                    SlopeDistanceBacksight = rawData.SlopeDistanceBacksight,
                    SlopeDistanceForesight = rawData.SlopeDistanceForesight,

                    HorizontalDistanceBacksight = rawData.HorizontalDistanceBacksight,
                    HorizontalDistanceForesight = rawData.HorizontalDistanceForesight,

                    VerticalDifferenceBacksight = rawData.VerticalDifferenceBacksight,
                    VerticalDifferenceForesight = rawData.VerticalDifferenceForesight,

                    BackCheckHorizontalDistance = rawData.BackCheckHorizontalDistance,
                    BackCheckHorizontalDifference = rawData.BackCheckHorizontalDifference,

                    BackCheckPegElevations = rawData.BackCheckPegElevations,
                    BackCheckVerticalError = rawData.BackCheckVerticalError,

                    HAngleDirectArc1Backsight = rawData.HAngleDirectArc1Backsight,
                    HAngleDirectArc1Foresight = rawData.HAngleDirectArc1Foresight,
                    HAngleTransitArc1Backsight = rawData.HAngleTransitArc1Backsight,
                    HAngleTransitArc1Foresight = rawData.HAngleTransitArc1Foresight,

                    HAngleDirectArc2Backsight = rawData.HAngleDirectArc2Backsight,
                    HAngleDirectArc2Foresight = rawData.HAngleDirectArc2Foresight,
                    HAngleTransitArc2Backsight = rawData.HAngleTransitArc2Backsight,
                    HAngleTransitArc2Foresight = rawData.HAngleTransitArc2Foresight,

                    VAngleDirectArc1Backsight = rawData.VAngleDirectArc1Backsight,
                    VAngleDirectArc1Foresight = rawData.VAngleDirectArc1Foresight,
                    VAngleTransitArc1Backsight = rawData.VAngleTransitArc1Backsight,
                    VAngleTransitArc1Foresight = rawData.VAngleTransitArc1Foresight,

                    VAngleDirectArc2Backsight = rawData.VAngleDirectArc2Backsight,
                    VAngleDirectArc2Foresight = rawData.VAngleDirectArc2Foresight,
                    VAngleTransitArc2Backsight = rawData.VAngleTransitArc2Backsight,
                    VAngleTransitArc2Foresight = rawData.VAngleTransitArc2Foresight,

                    HAngleDirectReducedArc1 = rawData.HAngleDirectReducedArc1,
                    HAngleTransitReducedArc1 = rawData.HAngleTransitReducedArc1,
                    HAngleDirectReducedArc2 = rawData.HAngleDirectReducedArc2,
                    HAngleTransitReducedArc2 = rawData.HAngleTransitReducedArc2,

                    HAngleMeanArc1 = rawData.HAngleMeanArc1,
                    HAngleMeanArc2 = rawData.HAngleMeanArc2,
                    HAngleMeanFinal = rawData.HAngleMeanFinal,
                    HAngleMeanFinalReturn = rawData.HAngleMeanFinalReturn,

                    VAngleBacksightMeanArc1 = rawData.VAngleBacksightMeanArc1,
                    VAngleBacksightMeanArc2 = rawData.VAngleBacksightMeanArc2,
                    VAngleBacksightMeanFinal = rawData.VAngleBacksightMeanFinal,

                    VAngleForesightMeanArc1 = rawData.VAngleForesightMeanArc1,
                    VAngleForesightMeanArc2 = rawData.VAngleForesightMeanArc2,
                    VAngleForesightMeanFinal = rawData.VAngleForesightMeanFinal,

                    BackBearingReturn = rawData.BackBearingReturn,
                    ForwardBearing = rawData.ForwardBearing,
                    ForwardBearingReturn = rawData.ForwardBearingReturn,

                    BacksightPegX = rawData.BacksightPegX,
                    BacksightPegY = rawData.BacksightPegY,
                    BacksightPegZ = rawData.BacksightPegZ,

                    StationPegX = rawData.StationPegX,
                    StationPegY = rawData.StationPegY,
                    StationPegZ = rawData.StationPegZ,

                    NewPegX = rawData.NewPegX,
                    NewPegY = rawData.NewPegY,
                    NewPegZ = rawData.NewPegZ,

                    DeltaX = rawData.DeltaX,
                    DeltaY = rawData.DeltaY,
                    DeltaZ = rawData.DeltaZ,

                    // Metadata
                    Surveyor = rawData.Surveyor,
                    SurveyDate = rawData.SurveyDate,
                    LocalityId = rawData.LocalityId,
                    Level = peg.Level.Id,
                    PointType = peg.PointType,
                    PegFailed = rawData.PegFailed,

                    // Display info
                    SurveyorDisplayName = peg.Surveyor?.DisplayName ?? rawData.Surveyor,
                    LocalityName = peg.Locality?.Name,
                    LevelName = peg.Level?.Name
                };

                return View("PegCalcResultViewOnly", viewModel);
            }
            else
            {
                return View("BasicPegView", peg);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredPegTable(string search, string filter, string sortOrder, string levelId)
        {
            var query = _dbContext.PegRegister
                .Include(p => p.Level)
                .Include(p => p.Locality)
                .Include(p => p.Surveyor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToUpper(); // Normalize for case-insensitive matching

                query = query.Where(p =>
                    p.PegName.ToUpper().Contains(search) ||
                    p.Surveyor.NormalizedFullName.ToUpper().Contains(search) ||
                    p.SurveyorNameText.ToUpper().Contains(search) ||
                    p.Locality.Name.ToUpper().Contains(search));
            }


            if (!string.IsNullOrEmpty(filter) && Enum.TryParse<SurveyPointType>(filter, out var pointType))
            {
                query = query.Where(p => p.PointType == pointType);
            }

            if (!string.IsNullOrEmpty(levelId) && int.TryParse(levelId, out var parsedLevelId))
            {
                query = query.Where(p => p.LevelId == parsedLevelId);
            }


            // Example sort (customize as needed)
            switch (sortOrder)
            {
                case "date_asc":
                    query = query.OrderBy(p => p.SurveyDate);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.PegName);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.PegName);
                    break;
                default:
                    query = query.OrderByDescending(p => p.SurveyDate); // default: newest first
                    break;
            }

            var pegs = await query.ToListAsync();
            return PartialView("_PegTable", pegs);
        }



        public async Task<IActionResult> ExportPegCalcPdf(int id)
        {
            var peg = await _dbContext.PegRegister
                .Include(p => p.Level)
                .Include(p => p.Locality)
                .Include(p => p.Surveyor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (peg == null)
                return NotFound();

            var rawData = await _dbContext.RawSurveyData
                .FirstOrDefaultAsync(r => r.ForeSightPeg == peg.PegName);

            if (rawData == null)
                return View("BasicPegView", peg);

            var model = new PegCalcViewModel
            {
                Id = peg.Id,

                ForeSightPeg = rawData.ForeSightPeg,
                StationPeg = rawData.StationPeg,
                BackSightPeg = rawData.BackSightPeg,

                TargetHeightBacksight = rawData.TargetHeightBacksight,
                TargetHeightForesight = rawData.TargetHeightForesight,
                InstrumentHeight = rawData.InstrumentHeight,
                SlopeDistanceBacksight = rawData.SlopeDistanceBacksight,
                SlopeDistanceForesight = rawData.SlopeDistanceForesight,

                HorizontalDistanceBacksight = rawData.HorizontalDistanceBacksight,
                HorizontalDistanceForesight = rawData.HorizontalDistanceForesight,

                VerticalDifferenceBacksight = rawData.VerticalDifferenceBacksight,
                VerticalDifferenceForesight = rawData.VerticalDifferenceForesight,

                BackCheckHorizontalDistance = rawData.BackCheckHorizontalDistance,
                BackCheckHorizontalDifference = rawData.BackCheckHorizontalDifference,

                BackCheckPegElevations = rawData.BackCheckPegElevations,
                BackCheckVerticalError = rawData.BackCheckVerticalError,

                HAngleDirectArc1Backsight = rawData.HAngleDirectArc1Backsight,
                HAngleDirectArc1Foresight = rawData.HAngleDirectArc1Foresight,
                HAngleTransitArc1Backsight = rawData.HAngleTransitArc1Backsight,
                HAngleTransitArc1Foresight = rawData.HAngleTransitArc1Foresight,

                HAngleDirectArc2Backsight = rawData.HAngleDirectArc2Backsight,
                HAngleDirectArc2Foresight = rawData.HAngleDirectArc2Foresight,
                HAngleTransitArc2Backsight = rawData.HAngleTransitArc2Backsight,
                HAngleTransitArc2Foresight = rawData.HAngleTransitArc2Foresight,

                VAngleDirectArc1Backsight = rawData.VAngleDirectArc1Backsight,
                VAngleDirectArc1Foresight = rawData.VAngleDirectArc1Foresight,
                VAngleTransitArc1Backsight = rawData.VAngleTransitArc1Backsight,
                VAngleTransitArc1Foresight = rawData.VAngleTransitArc1Foresight,

                VAngleDirectArc2Backsight = rawData.VAngleDirectArc2Backsight,
                VAngleDirectArc2Foresight = rawData.VAngleDirectArc2Foresight,
                VAngleTransitArc2Backsight = rawData.VAngleTransitArc2Backsight,
                VAngleTransitArc2Foresight = rawData.VAngleTransitArc2Foresight,

                HAngleDirectReducedArc1 = rawData.HAngleDirectReducedArc1,
                HAngleTransitReducedArc1 = rawData.HAngleTransitReducedArc1,
                HAngleDirectReducedArc2 = rawData.HAngleDirectReducedArc2,
                HAngleTransitReducedArc2 = rawData.HAngleTransitReducedArc2,

                HAngleMeanArc1 = rawData.HAngleMeanArc1,
                HAngleMeanArc2 = rawData.HAngleMeanArc2,
                HAngleMeanFinal = rawData.HAngleMeanFinal,
                HAngleMeanFinalReturn = rawData.HAngleMeanFinalReturn,

                VAngleBacksightMeanArc1 = rawData.VAngleBacksightMeanArc1,
                VAngleBacksightMeanArc2 = rawData.VAngleBacksightMeanArc2,
                VAngleBacksightMeanFinal = rawData.VAngleBacksightMeanFinal,

                VAngleForesightMeanArc1 = rawData.VAngleForesightMeanArc1,
                VAngleForesightMeanArc2 = rawData.VAngleForesightMeanArc2,
                VAngleForesightMeanFinal = rawData.VAngleForesightMeanFinal,

                BackBearingReturn = rawData.BackBearingReturn,
                ForwardBearing = rawData.ForwardBearing,
                ForwardBearingReturn = rawData.ForwardBearingReturn,

                BacksightPegX = rawData.BacksightPegX,
                BacksightPegY = rawData.BacksightPegY,
                BacksightPegZ = rawData.BacksightPegZ,

                StationPegX = rawData.StationPegX,
                StationPegY = rawData.StationPegY,
                StationPegZ = rawData.StationPegZ,

                NewPegX = rawData.NewPegX,
                NewPegY = rawData.NewPegY,
                NewPegZ = rawData.NewPegZ,

                DeltaX = rawData.DeltaX,
                DeltaY = rawData.DeltaY,
                DeltaZ = rawData.DeltaZ,

                // Metadata
                Surveyor = rawData.Surveyor,
                SurveyDate = rawData.SurveyDate,
                LocalityId = rawData.LocalityId,
                Level = peg.Level.Id,
                PointType = peg.PointType,
                PegFailed = rawData.PegFailed,

                // Display info
                SurveyorDisplayName = peg.Surveyor?.DisplayName ?? rawData.Surveyor,
                LocalityName = peg.Locality?.Name,
                LevelName = peg.Level?.Name
            };

            return new ViewAsPdf("PegCalcResultViewOnly", model)
            {
                FileName = $"PegCalc_{model.ForeSightPeg}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--enable-local-file-access --debug-javascript"
            };
        }

        public async Task<IActionResult> ExportPegCalcTestPdf(int id)
        {
            // Minimal logic for testing
            var model = new PegCalcViewModel
            {
                ForeSightPeg = "TEST-PEG",
                SurveyDate = DateOnly.FromDateTime(DateTime.Today),
                SurveyorDisplayName = "J. Tester",
                LocalityName = "Test Locality",
                LevelName = "Test Level",
                PegFailed = false,

                HAngleDirectArc1Backsight = 123.456m,
                HAngleDirectArc1Foresight = 234.567m,
                HAngleTransitArc1Backsight = 345.678m,
                HAngleTransitArc1Foresight = 456.789m,

                HAngleDirectArc2Backsight = 0,
                HAngleDirectArc2Foresight = 0,
                HAngleTransitArc2Backsight = 0,
                HAngleTransitArc2Foresight = 0,

                HAngleDirectReducedArc1 = 123.456m,
                HAngleTransitReducedArc1 = 234.567m,
                HAngleDirectReducedArc2 = 0,
                HAngleTransitReducedArc2 = 0,

                HAngleMeanArc1 = 179.999m,
                HAngleMeanArc2 = 0,
                HAngleMeanFinal = 180.000m,
                HAngleMeanFinalReturn = 0
            };


            return new ViewAsPdf("PegCalcResultViewOnly", model)
            {
                FileName = $"TestPegCalc.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--enable-local-file-access"
            };
        }



    }
}
