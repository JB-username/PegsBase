using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PegsBase.Data;
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
        public async Task<IActionResult> UploadPegCalc(IFormFile file, [FromServices] IRawSurveyDataDatFileParser parser)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please upload a valid .dat file.";
                return RedirectToAction("UploadPegCalc");
            }

            var parsedList = await parser.ParseRawSurveyFileAsync(file);

            if (parsedList == null || parsedList.Count == 0)
            {
                TempData["Error"] = "No valid data found in the file.";
                return RedirectToAction("UploadPegCalc");
            }

            var viewModel = parsedList.First(); // Only one setup in most cases

            TempData["PegCalcViewModel"] = JsonConvert.SerializeObject(viewModel);

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
                SurveyDate = new DateOnly(2025,02,22),
                PegFailed = true
            };

            var setupPeg = await _db.PegRegister.FirstOrDefaultAsync(p => p.PegName == "861");
            var backsightPeg = await _db.PegRegister.FirstOrDefaultAsync(p => p.PegName == "860");

            var calculated = _pegCalcService.CalculatePeg(dummy, setupPeg, backsightPeg);

            return View("PegCalcResult", dummy); // reuse the real PegCalc view
        }
#endif
    }
}