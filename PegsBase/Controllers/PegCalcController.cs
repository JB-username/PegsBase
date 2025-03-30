using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Models.Enums;
using PegsBase.Models.ViewModels;
using PegsBase.Services.Parsing.Interfaces;
using PegsBase.Services.PegCalc.Implementations;
using PegsBase.Services.PegCalc.Interfaces;
using System.Threading.Tasks;

namespace PegsBase.Controllers
{
    public class PegCalcController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPegCalcService _pegCalcService;

        public PegCalcController(ApplicationDbContext db, IPegCalcService pegCalcService)
        {
            _db = db;
            _pegCalcService = pegCalcService;
        }

        public IActionResult UploadPegCalc()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPegCalc(
                 RawSurveyDataUploadViewModel model,
                 [FromServices] IRawSurveyDataDatFileParser parser)
        {
            if (model.File == null || model.File.Length == 0)
            {
                TempData["Error"] = "Please upload a valid .dat file.";
                return RedirectToAction("UploadPegCalc");
            }

            var parsedList = await parser.ParseRawSurveyFileAsync(model.File);

            if (parsedList == null || parsedList.Count == 0)
            {
                TempData["Error"] = "No valid data found in the file.";
                return RedirectToAction("UploadPegCalc");
            }

            var viewModel = parsedList.First(); // Only one setup in most cases

            TempData["PegCalcViewModel"] = JsonConvert.SerializeObject(viewModel);
            HttpContext.Session.SetString("PegCalcViewModel", JsonConvert.SerializeObject(viewModel));

            return RedirectToAction("Calculate");
        }



        [HttpGet]
        public async Task<IActionResult> Calculate()
        {
            var modelJson = TempData["PegCalcViewModel"] as string;
            if (string.IsNullOrEmpty(modelJson))
            {
                TempData["Error"] = "Peg calculation data lost. Please re-upload.";
                return RedirectToAction("UploadPegCalc");
            }

            var viewModel = JsonConvert.DeserializeObject<PegCalcViewModel>(modelJson);

            var setupPeg = await _db.PegRegister.FirstOrDefaultAsync(p => p.PegName == viewModel.StationPeg);
            var backsightPeg = await _db.PegRegister.FirstOrDefaultAsync(p => p.PegName == viewModel.BackSightPeg);

            if (setupPeg == null || backsightPeg == null)
            {
                TempData["PegCalcViewModel"] = modelJson; // keep it alive for after coordinate upload
                TempData["Error"] = "Setup or Backsight peg not found. Please upload the peg coordinates.";
                return RedirectToAction("UploadCoordinates", "PegRegister");
            }

            var calculated = _pegCalcService.CalculatePeg(viewModel, setupPeg, backsightPeg);

            return View("PegCalcResult", calculated);
        }


        [HttpGet]
        public IActionResult PrintView()
        {
            var modelJson = HttpContext.Session.GetString("PegCalcViewModel");
            if (string.IsNullOrEmpty(modelJson))
            {
                TempData["Error"] = "Peg calculation data lost.";
                return RedirectToAction("UploadPegCalc");
            }

            var viewModel = JsonConvert.DeserializeObject<PegCalcViewModel>(modelJson);

            return View("PegCalcPrint", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SavePegCalc(PegCalcViewModel rawData)
        {

            if (!ModelState.IsValid)
            {
                return View("PegCalcResult", rawData);
            }

            var pegRegister = await _db.PegRegister.FirstOrDefaultAsync(p => p.PegName == rawData.ForeSightPeg);

            if (!Enum.IsDefined(typeof(SurveyPointType), rawData.PointType))
            {
                rawData.PointType = SurveyPointType.Peg;
            }

            if (pegRegister == null)
            {
                pegRegister = new PegRegister
                {
                    Level = rawData.Level,
                    Locality = rawData.Locality,
                    GradeElevation = rawData.GradeElevation,
                    Surveyor = rawData.Surveyor,

                    PointType = rawData.PointType,

                    PegName = rawData.ForeSightPeg,
                    YCoord = rawData.NewPegY,
                    XCoord = rawData.NewPegX,
                    ZCoord = rawData.NewPegZ,
                    SurveyDate = rawData.SurveyDate,
                    PegFailed = rawData.PegFailed,
                    HasPegCalc = true,
                    FromPeg = rawData.StationPeg,
                };

                _db.PegRegister.Add(pegRegister);
            }

            var raw = new RawSurveyData
            {
                Surveyor = rawData.Surveyor,
                SurveyDate = rawData.SurveyDate,
                Locality = rawData.Locality,

                StationPeg = rawData.StationPeg,
                BackSightPeg = rawData.BackSightPeg,
                ForeSightPeg = rawData.ForeSightPeg,

                InstrumentHeight = rawData.InstrumentHeight,
                TargetHeightBacksight = rawData.TargetHeightBacksight,
                TargetHeightForesight = rawData.TargetHeightForesight,
                SlopeDistanceBacksight = rawData.SlopeDistanceBacksight,
                SlopeDistanceForesight = rawData.SlopeDistanceForesight,

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

                PegFailed = rawData.PegFailed,

                HorizontalDistanceBacksight = rawData.HorizontalDistanceBacksight,
                HorizontalDistanceForesight = rawData.HorizontalDistanceForesight,
                VerticalDifferenceBacksight = rawData.VerticalDifferenceBacksight,
                VerticalDifferenceForesight = rawData.VerticalDifferenceForesight,
                BackCheckHorizontalDistance = rawData.BackCheckHorizontalDistance,
                BackCheckHorizontalDifference = rawData.BackCheckHorizontalDifference,
                BackCheckPegElevations = rawData.BackCheckPegElevations,
                BackCheckVerticalError = rawData.BackCheckVerticalError,

                HAngleDirectReducedArc1 = rawData.HAngleDirectReducedArc1,
                HAngleDirectReducedArc2 = rawData.HAngleDirectReducedArc2,
                HAngleTransitReducedArc1 = rawData.HAngleTransitReducedArc1,
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

                StationPegX = rawData.StationPegX,
                StationPegY = rawData.StationPegY,
                StationPegZ = rawData.StationPegZ,

                BacksightPegX = rawData.BacksightPegX,
                BacksightPegY = rawData.BacksightPegY,
                BacksightPegZ = rawData.BacksightPegZ,

                NewPegX = rawData.NewPegX,
                NewPegY = rawData.NewPegY,
                NewPegZ = rawData.NewPegZ,

                DeltaX = rawData.DeltaX,
                DeltaY = rawData.DeltaY,
                DeltaZ = rawData.DeltaZ

            };

            var existingRaw = await _db.RawSurveyData
                    .FirstOrDefaultAsync(
                r => r.ForeSightPeg ==
                rawData.ForeSightPeg);

            if (existingRaw == null)
            {
                _db.RawSurveyData.Add(raw);
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "PegRegister");
        }


#if DEBUG
        [Route("pegcalc/dev")]
        public async Task<IActionResult> DevTestPegCalc()
        {
            var dummy = new PegCalcViewModel
            {
                StationPeg = "861",
                BackSightPeg = "860",
                ForeSightPeg = "862",

                HAngleDirectArc1Backsight = 15.1441m,
                HAngleDirectArc1Foresight = 293.0739m,
                HAngleTransitArc1Backsight = 262.0740m,
                HAngleTransitArc1Foresight = 179.5948m,

                VAngleDirectArc1Backsight = 78.3434m,
                VAngleTransitArc1Backsight = 281.2544m,
                VAngleDirectArc1Foresight = 83.3943m,
                VAngleTransitArc1Foresight = 276.2127m,

                SlopeDistanceBacksight = 1.939m,
                InstrumentHeight = -1.721m,
                TargetHeightForesight = 0m,
                TargetHeightBacksight = 1.232m,
                SlopeDistanceForesight = 3.230m,

                Locality = "Test Pit 1",
                Surveyor = "T. Surveyor",
                SurveyDate = new DateOnly(2025, 02, 22),
                PegFailed = false
            };

            var setupPeg = await _db.PegRegister.FirstOrDefaultAsync(p => p.PegName == "861");
            var backsightPeg = await _db.PegRegister.FirstOrDefaultAsync(p => p.PegName == "860");

            var calculated = _pegCalcService.CalculatePeg(dummy, setupPeg, backsightPeg);

            return View("PegCalcResult", dummy); // reuse the real PegCalc view
        }
#endif
    }
}