using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.Entities;

namespace PegsBase.Controllers
{
    [Authorize]
    public class LevelsController : Controller
    {

        private readonly ApplicationDbContext _dbContext;

        public LevelsController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index(int? subSectionId)
        {
            var levelsQuery = _dbContext.Levels
                .Include(l => l.SubSection)
                .OrderBy(l => l.Name)
                .AsQueryable();

            if (subSectionId.HasValue)
            {
                levelsQuery = levelsQuery.Where(l => l.SubSectionId == subSectionId.Value);
            }

            var levels = levelsQuery.ToList();

            ViewBag.SubSections = new SelectList(_dbContext.SubSections
                .OrderBy(s => s.Name), "Id", "Name", subSectionId);

            return View(levels);
        }

        [HttpPost]
        public IActionResult CreateInline(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _dbContext.Levels.Add(new Level { Name = name.Trim() });
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditInline(int id, string name, int? subSectionId)
        {
            var level = _dbContext.Levels.Find(id);
            if (level != null && !string.IsNullOrWhiteSpace(name))
            {
                level.Name = name.Trim();
                level.SubSectionId = subSectionId; // Handle null or value

                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var level = _dbContext.Levels.Find(id);
            if (level != null)
            {
                _dbContext.Levels.Remove(level);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
