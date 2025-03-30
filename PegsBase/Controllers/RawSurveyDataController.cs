using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using PegsBase.Data;
using System;

namespace PegsBase.Controllers
{
    public class RawSurveyDataController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RawSurveyDataController(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _db.RawSurveyData
                .OrderByDescending(r => r.SurveyDate)
                .ToListAsync();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            var allRawData = await _db.RawSurveyData.ToListAsync();
            _db.RawSurveyData.RemoveRange(allRawData);
            await _db.SaveChangesAsync();

            TempData["Message"] = "All raw survey data has been deleted.";
            return RedirectToAction("Index");
        }

    }
}
