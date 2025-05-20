using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Models.Constants;
using PegsBase.Models.Enums;
using PegsBase.Models.Identity;
using PegsBase.Models.ViewModels;
using PegsBase.Services.Parsing;
using PegsBase.Services.Parsing.Interfaces;
using PegsBase.Services.PegCalc.Interfaces;
using PegsBase.Services.Settings;
using QuestPDF.Fluent;
using Rotativa.AspNetCore;
using System.Text;

namespace PegsBase.Controllers
{
    [Authorize]
    public class PegRegisterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PegRegisterController> _logger;
        private readonly IPegFileParser _pegFileParser;
        private readonly ICoordinateDatParserService _coordinateDatParserService;
        private readonly IPegCalcService _pegCalcService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapImportModelsToPegs _pegMapper;
        private readonly IImportSettingsService _importSettingsService;

        public PegRegisterController(
            ApplicationDbContext db,
            ILogger<PegRegisterController> logger,
            IPegFileParser pegFileParser,
            ICoordinateDatParserService coordinateDatParserService,
            IPegCalcService pegCalcService,
            UserManager<ApplicationUser> user,
            IMapImportModelsToPegs mapImportModelsToPeg,
            IImportSettingsService importSettingsService)
        {
            _dbContext = db;
            _logger = logger;
            _pegFileParser = pegFileParser;
            _coordinateDatParserService = coordinateDatParserService;
            _pegCalcService = pegCalcService;
            _userManager = user;
            _pegMapper = mapImportModelsToPeg;
            _importSettingsService = importSettingsService;
        }

        [Authorize(Policy = "SurveyDepartment")]
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


        [Authorize(Policy = "SurveyDepartment")]
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

        [Authorize(Policy = "SurveyDepartment")]
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

        [Authorize(Policy = "SurveyManagers")]
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


        [Authorize(Policy = "SurveyManagers")]
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


        [Authorize(Policy = "SurveyManagers")]
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

        [Authorize(Policy = "SurveyManagers")]
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

        [Authorize(Policy = "SurveyManagers")]
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

        #region PDF Exports
        [Authorize(Policy = "SurveyDepartment")]
        [HttpGet]
        public IActionResult ExportPegCalcPdf(int id)
        {
            // 1) Fetch the saved row (including any FKs you need)
            var raw = _dbContext.RawSurveyData
                         .Include(r => r.Locality)
                         //.Include(r => r.Level)                       //TODO: add level to the pegcalc
                         .FirstOrDefault(r => r.Id == id);

            if (raw == null)
                return NotFound();

            // 2) Project **every** field into your VM
            var model = new PegCalcViewModel
            {
                // metadata
                SurveyorDisplayName = raw.Surveyor ?? "(unknown)",
                SurveyDate = raw.SurveyDate,
                LocalityName = raw.Locality.Name,
                //LevelName = raw.Level.Name,

                // peg IDs
                StationPeg = raw.StationPeg,
                BackSightPeg = raw.BackSightPeg,
                ForeSightPeg = raw.ForeSightPeg,

                // distances & heights
                InstrumentHeight = raw.InstrumentHeight,
                TargetHeightBacksight = raw.TargetHeightBacksight,
                TargetHeightForesight = raw.TargetHeightForesight,
                SlopeDistanceBacksight = raw.SlopeDistanceBacksight,
                SlopeDistanceForesight = raw.SlopeDistanceForesight,

                // horizontal angles (arc1 + arc2)
                HAngleDirectArc1Backsight = raw.HAngleDirectArc1Backsight,
                HAngleTransitArc1Backsight = raw.HAngleTransitArc1Backsight,
                HAngleDirectArc1Foresight = raw.HAngleDirectArc1Foresight,
                HAngleTransitArc1Foresight = raw.HAngleTransitArc1Foresight,

                HAngleDirectArc2Backsight = raw.HAngleDirectArc2Backsight,
                HAngleTransitArc2Backsight = raw.HAngleTransitArc2Backsight,
                HAngleDirectArc2Foresight = raw.HAngleDirectArc2Foresight,
                HAngleTransitArc2Foresight = raw.HAngleTransitArc2Foresight,

                // vertical angles (arc1 + arc2)
                VAngleDirectArc1Backsight = raw.VAngleDirectArc1Backsight,
                VAngleTransitArc1Backsight = raw.VAngleTransitArc1Backsight,
                VAngleDirectArc1Foresight = raw.VAngleDirectArc1Foresight,
                VAngleTransitArc1Foresight = raw.VAngleTransitArc1Foresight,

                VAngleDirectArc2Backsight = raw.VAngleDirectArc2Backsight,
                VAngleTransitArc2Backsight = raw.VAngleTransitArc2Backsight,
                VAngleDirectArc2Foresight = raw.VAngleDirectArc2Foresight,
                VAngleTransitArc2Foresight = raw.VAngleTransitArc2Foresight,

                // reduced & means
                HAngleDirectReducedArc1 = raw.HAngleDirectReducedArc1,
                HAngleTransitReducedArc1 = raw.HAngleTransitReducedArc1,
                HAngleDirectReducedArc2 = raw.HAngleDirectReducedArc2,
                HAngleTransitReducedArc2 = raw.HAngleTransitReducedArc2,

                HAngleMeanArc1 = raw.HAngleMeanArc1,
                HAngleMeanArc2 = raw.HAngleMeanArc2,
                HAngleMeanFinal = raw.HAngleMeanFinal,
                HAngleMeanFinalReturn = raw.HAngleMeanFinalReturn,

                VAngleBacksightMeanArc1 = raw.VAngleBacksightMeanArc1,
                VAngleBacksightMeanArc2 = raw.VAngleBacksightMeanArc2,
                VAngleBacksightMeanFinal = raw.VAngleBacksightMeanFinal,

                VAngleForesightMeanArc1 = raw.VAngleForesightMeanArc1,
                VAngleForesightMeanArc2 = raw.VAngleForesightMeanArc2,
                VAngleForesightMeanFinal = raw.VAngleForesightMeanFinal,

                // bearings & back-check
                BackBearingReturn = raw.BackBearingReturn,
                ForwardBearing = raw.ForwardBearing,
                ForwardBearingReturn = raw.ForwardBearingReturn,

                HorizontalDistanceBacksight = raw.HorizontalDistanceBacksight,
                HorizontalDistanceForesight = raw.HorizontalDistanceForesight,
                VerticalDifferenceBacksight = raw.VerticalDifferenceBacksight,
                VerticalDifferenceForesight = raw.VerticalDifferenceForesight,

                BackCheckHorizontalDistance = raw.BackCheckHorizontalDistance,
                BackCheckHorizontalDifference = raw.BackCheckHorizontalDifference,
                BackCheckPegElevations = raw.BackCheckPegElevations,
                BackCheckVerticalError = raw.BackCheckVerticalError,

                // station & backsight coords
                StationPegX = raw.StationPegX,
                StationPegY = raw.StationPegY,
                StationPegZ = raw.StationPegZ,
                BacksightPegX = raw.BacksightPegX,
                BacksightPegY = raw.BacksightPegY,
                BacksightPegZ = raw.BacksightPegZ,

                // new peg & deltas
                NewPegX = raw.NewPegX,
                NewPegY = raw.NewPegY,
                NewPegZ = raw.NewPegZ,
                DeltaX = raw.DeltaX,
                DeltaY = raw.DeltaY,
                DeltaZ = raw.DeltaZ,

                PegFailed = raw.PegFailed
            };

            // 3) Generate the PDF
            var document = new PegCalcReportDocument(model);
            byte[] pdf = document.GeneratePdf();

            // 4) Return it to the browser
            return File(pdf,
                        "application/pdf",
                        $"PegCalc_{model.ForeSightPeg}.pdf");
        }

        [Authorize(Policy = "SurveyDepartment")]
        [HttpGet]
        public IActionResult ExportPreviewPdf(string pegName)
        {
            // load your peg preview – here I assume you find by PegName
            var entity = _dbContext.PegRegister
                            .FirstOrDefault(p => p.PegName == pegName);
            if (entity == null) return NotFound();

            // map to your PegPreviewModel
            var model = new PegPreviewModel
            {
                PegName = entity.PegName,
                XCoord = entity.XCoord,
                YCoord = entity.YCoord,
                ZCoord = entity.ZCoord,
                GradeElevation = entity.GradeElevation,
                LevelId = entity.LevelId,
                Level = entity.Level,
                LocalityId = entity.LocalityId,
                Locality = entity.Locality,
                SurveyDate = entity.SurveyDate,
                SurveyorId = entity.SurveyorId,
                Surveyor = entity.Surveyor,
                //Type = entity.Type,
                SaveToDatabase = true  // or some logic to reflect Pass/Fail
            };

            var document = new PegPreviewReportDocument(model);
            byte[] pdf = document.GeneratePdf();

            return File(pdf,
                        "application/pdf",
                        $"PegPreview_{model.PegName}.pdf");
        }
        #endregion


        [Authorize(Policy = "SurveyDepartment")]
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



        [Authorize(Policy = "SurveyDepartment")]
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



        [Authorize(Policy = "SurveyDepartment")]
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

        [Authorize(Policy = "SurveyDepartment")]
        [HttpGet]
        public IActionResult ImportCsvFile()
        {
            return View();
        }

        [Authorize(Policy = "SurveyDepartment")]
        [HttpPost]
        public IActionResult ImportCsvFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a CSV file to upload");
                return View();
            }

            using var stream = file.OpenReadStream();
            var parseResults = _pegFileParser.Parse(stream);

            var settings = _importSettingsService.GetSettings();

            // apply your SwapXY/InvertX/InvertY to the *Peg* inside each result
            foreach (var row in parseResults)
            {
                var peg = row.Peg;

                if (settings.SwapXY)
                {
                    var tmp = peg.XCoord;
                    peg.XCoord = peg.YCoord;
                    peg.YCoord = tmp;
                }

                if (settings.InvertX)
                {
                    peg.XCoord = -peg.XCoord;
                }

                if (settings.InvertY)
                {
                    peg.YCoord = -peg.YCoord;
                }
            }

            TempData["CsvParseResults"] = JsonConvert.SerializeObject(parseResults);

            return RedirectToAction(nameof(PreviewCsvImport));
        }



        [Authorize(Policy = "SurveyDepartment")]
        [HttpGet]
        public IActionResult PreviewCsvImport()
        {
            if (!TempData.TryGetValue("CsvParseResults", out var raw)
                || raw is not string json)
                return RedirectToAction(nameof(ImportCsvFile));

            // keep it for the POST
            TempData.Keep("CsvParseResults");

            var allResults = JsonConvert
                .DeserializeObject<List<CsvParseResult>>(json)
                ?? new List<CsvParseResult>();

            var good = allResults.Where(r => !r.Errors.Any()).ToList();
            var bad = allResults.Where(r => r.Errors.Any()).ToList();

            var vm = new CsvPreviewModel
            {
                TotalRows = allResults.Count,
                TotalGood = good.Count,
                GoodRows = good.Take(20).ToList(),
                BadRows = bad
            };

            return View(vm);
        }



        [Authorize(Policy = "SurveyDepartment")]
        [HttpPost]
        public IActionResult ConfirmCsvImport()
        {
            // 1) Retrieve the full JSON blob from TempData
            if (!TempData.TryGetValue("CsvParseResults", out var raw)
                || raw is not string json
                || string.IsNullOrWhiteSpace(json))
            {
                // Nothing to import — redirect back to upload
                return RedirectToAction(nameof(ImportCsvFile));
            }

            // 2) Deserialize the entire list of parse results
            var parseResults = JsonConvert
                .DeserializeObject<List<CsvParseResult>>(json)
                ?? new List<CsvParseResult>();

            var rowsToSave = new List<PegRegister>();
            var mappingErrors = new List<string>();

            // 3) For every row with no parse errors, attempt to map & build a PegRegister
            foreach (var row in parseResults.Where(r => !r.Errors.Any()))
            {
                var m = row.Peg;

                // Safely coalesce any null names
                var levelName = m.LevelName ?? string.Empty;
                var localityName = m.LocalityName ?? string.Empty;

                // 3.1) Find Level by case-insensitive ILIKE
                var level = _dbContext.Levels
                    .FirstOrDefault(l => EF.Functions.ILike(l.Name, levelName));
                if (level == null)
                {
                    mappingErrors.Add(
                        $"Row {row.RowNumber}: Level '{levelName}' not found.");
                    continue;
                }

                // 3.2) Find Locality scoped to that level
                var locality = _dbContext.Localities
                    .FirstOrDefault(loc =>
                        EF.Functions.ILike(loc.Name, localityName)
                        && loc.LevelId == level.Id);
                if (locality == null)
                {
                    mappingErrors.Add(
                        $"Row {row.RowNumber}: Locality '{localityName}' not found on level '{level.Name}'.");
                    continue;
                }

                // 3.3) Find Surveyor by normalized full‐name, or by initial+surname
                ApplicationUser? user = null;
                if (!string.IsNullOrWhiteSpace(m.SurveyorName))
                {
                    var rawName = m.SurveyorName.Trim();
                    var normalized = ApplicationUser.Normalize(rawName);

                    // Try exact match on normalized full name
                    user = _userManager.Users
                        .FirstOrDefault(u => u.NormalizedFullName == normalized);

                    // If no full match and looks like “X. LastName”, match by surname
                    if (user == null)
                    {
                        var tokens = rawName
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2
                            && tokens[0].Length == 2
                            && tokens[0].EndsWith('.'))
                        {
                            var normSurname = ApplicationUser.Normalize(tokens[1]);
                            user = _userManager.Users
                                .FirstOrDefault(u =>
                                    u.NormalizedFullName != null
                                    && u.NormalizedFullName.EndsWith(normSurname));
                        }
                    }
                }

                // 3.4) Build the PegRegister entity
                rowsToSave.Add(new PegRegister
                {
                    PegName = m.PegName,
                    XCoord = m.XCoord,
                    YCoord = m.YCoord,
                    ZCoord = m.ZCoord,
                    GradeElevation = m.GradeElevation,
                    SurveyDate = m.SurveyDate ?? DateOnly.FromDateTime(DateTime.Today),
                    PointType = m.PointType,
                    LevelId = level.Id,
                    LocalityId = locality.Id,
                    SurveyorId = user?.Id,
                    SurveyorNameText = user == null ? m.SurveyorName : null
                });
            }

            // 4) Persist all successfully mapped rows
            if (rowsToSave.Any())
            {
                _dbContext.PegRegister.AddRange(rowsToSave);
                _dbContext.SaveChanges();
            }

            // 5) Prepare user feedback
            TempData["Success"] = $"{rowsToSave.Count} pegs imported successfully.";
            if (mappingErrors.Any())
            {
                // Join errors with newline so your UI can render them clearly
                TempData["Error"] = string.Join("\\n", mappingErrors);
            }

            return RedirectToAction(nameof(Index));
        }



        [Authorize(Policy = "SurveyDepartment")]
        public IActionResult DownloadTemplate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("PegName,XCoord,YCoord,ZCoord,GradeElevation,Locality,Level,Surveyor,SurveyDate,PointType");
            sb.AppendLine("22033,5000.123,1500.456,250.789,50.0,MainShaft,Level1,george washington,2025-05-19,Peg");
            sb.AppendLine("CP-001,5100.000,1520.000,260.000,60.0,SurveySite,Level2,g. washington,2025-05-18,Control");
            sb.AppendLine("S563,5200.500,1550.750,270.250,70.0,BeaconArea,Level3,L. Mayberry,2025-05-17,Beacon");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "CSV_Peg_Import_Template.csv");
        }

        [Authorize(Policy = "SurveyDepartment")]
        [HttpPost]
        public IActionResult PrintSelected(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "No pegs selected to print.";
                return RedirectToAction(nameof(Index));
            }

            var pegs = _dbContext.PegRegister
                .Where(p => selectedIds.Contains(p.Id))
                .ToList();

            return View("PrintView", pegs); // Pass data to a dedicated print view
        }

        [Authorize(Policy = "SurveyDepartment")]
        [HttpGet]
        public IActionResult ImportDatFile()
        {
            return View(new CoordinateUploadViewModel());
        }

        [Authorize(Policy = "SurveyDepartment")]
        [HttpPost]
        public async Task<IActionResult> ImportDatFile(CoordinateUploadViewModel model)
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
            var rawUsers = _dbContext.Users
                .OrderBy(u => u.LastName)
                .Select(u => new {
                    u.Id,
                    u.FirstName,
                    u.LastName
                })
                .AsEnumerable();

            var surveyorItems = rawUsers
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = !string.IsNullOrEmpty(u.FirstName)
                        ? $"{u.FirstName[0]}. {u.LastName}"
                        : u.LastName
                })
                .ToList();

            ViewBag.Surveyors = surveyorItems;

            TempData["RedirectAfterSaveUrl"] = model.RedirectAfterSaveUrl;

            return View("PreviewDatImport", model);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmDatImport(CoordinateUploadViewModel model)
        {
            if (model.PreviewRows == null || !model.PreviewRows.Any())
            {
                ModelState.AddModelError("", "No pegs to save.");
                return View(nameof(ImportDatFile), model);
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

        [Authorize(Policy = "SurveyDepartment")]
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
                    Level = peg.Level?.Id ?? 0,
                    PointType = peg.PointType,
                    PegFailed = rawData.PegFailed,

                    // Display info
                    SurveyorDisplayName = peg.Surveyor?.DisplayName ?? rawData.Surveyor,
                    LocalityName = peg.Locality?.Name ?? "(none)",
                    LevelName = peg.Level?.Name ?? "(none)"
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
                search = search.Trim();
                var pattern = $"%{search}%";

                query = query.Where(p =>
                    // PegName is non-nullable
                    EF.Functions.ILike(p.PegName, pattern)

                    // Only ILIKE a non-null NormalizedFullName
                    || (p.Surveyor != null
                        && p.Surveyor.NormalizedFullName != null
                        && EF.Functions.ILike(p.Surveyor.NormalizedFullName, pattern))

                    // Only ILIKE SurveyorNameText if present
                    || (!string.IsNullOrEmpty(p.SurveyorNameText)
                        && EF.Functions.ILike(p.SurveyorNameText, pattern))

                    // Only ILIKE Locality.Name if Locality exists
                    || (p.Locality != null
                        && EF.Functions.ILike(p.Locality.Name, pattern))
                );
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



        public async Task<IActionResult> ExportPegCalcPdfRotativaTest(int id)
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
                Level = peg.Level?.Id ?? 0,
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
    }
}
