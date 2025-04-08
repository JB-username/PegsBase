using Microsoft.AspNetCore.Mvc;
using PegsBase.Models.Settings;
using PegsBase.Services.Settings;

namespace PegsBase.Controllers
{
    public class SettingsController : Controller
    {

        private readonly IImportSettingsService _settingsService;

        public SettingsController(IImportSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var settings = _settingsService.GetSettings();
            return View(settings);
        }

        [HttpPost]
        public IActionResult Index(ImportSettings model)
        {
            if (ModelState.IsValid)
            {
                _settingsService.SaveSettings(model);
                ViewBag.Message = "Settings saved!";
            }

            return View(model);
        }
    }
}
