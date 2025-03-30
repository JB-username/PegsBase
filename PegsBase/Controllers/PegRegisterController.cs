using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Models.Enums;
using PegsBase.Models.ViewModels;
using PegsBase.Services.Parsing;
using PegsBase.Services.Parsing.Interfaces;
using PegsBase.Services.PegCalc.Interfaces;
using System.Text;

namespace PegsBase.Controllers
{
    public class PegRegisterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPegFileParser _pegFileParser;
        private readonly ICoordinateDatParserService _coordinateDatParserService;
        private readonly IPegCalcService _pegCalcService;

        public PegRegisterController(
            ApplicationDbContext db, 
            IPegFileParser pegFileParser,
            ICoordinateDatParserService coordinateDatParserService,
            IPegCalcService pegCalcService)
        {
            _dbContext = db;
            _pegFileParser = pegFileParser;
            _coordinateDatParserService = coordinateDatParserService;
            _pegCalcService = pegCalcService;
        }

        public IActionResult Index(SurveyPointType? filter, string sortOrder)
        {
            var objPegsList = _dbContext.PegRegister.AsQueryable();

            if(filter.HasValue)
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

            return View(objPegsList.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PegRegister obj)
        {
            if(obj != null && obj.PegName == null)
            {
                ModelState.AddModelError("Title", "Please enter a peg name");
            }

            if (ModelState.IsValid)
            {
                _dbContext.PegRegister.Add(obj);
                _dbContext.SaveChanges();

                TempData["Success"] = "Peg created successfully.";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            PegRegister? pegEntry = _dbContext.PegRegister.Find(id);
            
            if(pegEntry == null)
            {
                return NotFound();
            }

            return View(pegEntry);
        }

        [HttpPost]
        public IActionResult Edit(PegRegister obj)
        {
            if (obj != null && obj.PegName == null)
            {
                ModelState.AddModelError("Title", "Please enter a peg name");
            }

            if (ModelState.IsValid)
            {
                _dbContext.PegRegister.Update(obj);
                _dbContext.SaveChanges();

                TempData["Success"] = "Peg updated successfully.";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        [HttpGet]
        public IActionResult Delete(int? id, SurveyPointType? filter)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            if(filter.HasValue)
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


        [HttpPost]
        public IActionResult ExportSelectedToCsv(List<int> selectedIds)
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
                csv.AppendLine(
                    $"{peg.PegName}," +
                    $"{peg.YCoord}," +
                    $"{peg.XCoord}," +
                    $"{peg.ZCoord}," +
                    $"{peg.GradeElevation},"
                    );
            }

            var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"SelectedPegs_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return File(fileBytes, "text/csv", fileName);
        }

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

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a CSV file to upload");
                return View();
            }

            using var stream = file.OpenReadStream();
            var parsedPegs = _pegFileParser.Parse(stream);

            TempData["ParsedPegs"] = JsonConvert.SerializeObject(parsedPegs);

            return RedirectToAction("Preview");
        }

        [HttpGet]
        public IActionResult Preview()
        {
            if (!TempData.TryGetValue("ParsedPegs", out var rawData) || rawData == null)
                return RedirectToAction("Upload");

            var pegs = JsonConvert.DeserializeObject<List<PegRegister>>(rawData.ToString());
            return View(pegs);
        }

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

        public IActionResult DownloadTemplate()
        {
            var csv = "PegName,XCoord,YCoord,ZCoord,GradeElevation,Locality,Level,Surveyor,SurveyDate,PointType\n";
            var bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "PegTemplate.csv");
        }

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

        [HttpGet]
        public IActionResult UploadDat()
        {
            return View(new CoordinateUploadViewModel());
        }

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

            model.PreviewRows = parsedRows.Take(2).ToList(); // Only show first 2

            if (!model.PreviewRows.Any())
            {
                ModelState.AddModelError("", "No valid rows were found in the file.");
                return View(model);
            }

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
                    Surveyor = preview.Surveyor ?? "Unknown",
                    Locality = preview.Locality ?? "Unknown",
                    SurveyDate = preview.SurveyDate ?? DateOnly.FromDateTime(DateTime.Today),
                    Level = preview.Level ?? 0,
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

        public async Task<IActionResult> ViewPeg(int id)
        {
            var peg = await _dbContext.PegRegister.FirstOrDefaultAsync(p => p.Id == id);

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

                    HAngleDirectReducedArc1 =rawData.HAngleDirectReducedArc1,
                    HAngleTransitReducedArc1 = rawData.HAngleTransitReducedArc1,
                    HAngleDirectReducedArc2 =rawData.HAngleDirectReducedArc2,
                    HAngleTransitReducedArc2 =rawData.HAngleTransitReducedArc2,

                    HAngleMeanArc1 =rawData.HAngleMeanArc1,
                    HAngleMeanArc2 =rawData.HAngleMeanArc2,
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
                    Locality = rawData.Locality,
                    Level = peg.Level,
                    PointType = peg.PointType,
                    PegFailed = rawData.PegFailed
                };

                return View("PegCalcResultViewOnly", viewModel);
            }
            else
            {
                return View("BasicPegView", peg);
            }
        }
    }
}
