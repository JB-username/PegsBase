using Microsoft.AspNetCore.Mvc;
using PegsBase.Models.Settings;
using PegsBase.Services.Settings;
using Microsoft.AspNetCore.Authorization;
using PegsBase.Models;

namespace PegsBase.Controllers
{
    public class SettingsController : Controller
    {

        private readonly IImportSettingsService _settingsService;

        public SettingsController(IImportSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [Authorize(Roles = "Master" + "," + "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Settings()
        {
            var settings = _settingsService.GetSettings();
            return View(settings);
        }

        [HttpPost]
        public IActionResult Settings(AppSettings model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Model Invalid!";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                _settingsService.SaveSettings(model);
                ViewBag.Message = "Settings saved!";
            }

            return View(model);
        }

    }
}
