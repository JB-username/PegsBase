using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models;      // where PlanType lives
using PegsBase.Models.MinePlans;
using System.Threading.Tasks;

namespace PegsBase.Controllers
{
    [Authorize(Policy = "SurveyDepartment")]
    public class PlanTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PlanTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: PlanTypes
        public async Task<IActionResult> Index()
        {
            var types = await _db.PlanTypes
                                .OrderBy(pt => pt.Name)
                                .ToListAsync();
            return View(types);
        }

        // GET: PlanTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlanTypes/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.PlanTypes.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Plan type added.";
            return RedirectToAction(nameof(Index));
        }

        // GET: PlanTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var pt = await _db.PlanTypes.FindAsync(id);
            if (pt == null) return NotFound();
            return View(pt);
        }

        // POST: PlanTypes/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PlanType model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var pt = await _db.PlanTypes.FindAsync(model.Id);
            if (pt == null) return NotFound();

            pt.Name = model.Name;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Plan type updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: PlanTypes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var pt = await _db.PlanTypes.FindAsync(id);
            if (pt == null) return NotFound();
            return View(pt);
        }

        // POST: PlanTypes/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pt = await _db.PlanTypes.FindAsync(id);
            if (pt == null) return NotFound();

            _db.PlanTypes.Remove(pt);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Plan type removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}

