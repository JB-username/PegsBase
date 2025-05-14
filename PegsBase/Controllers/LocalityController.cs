using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.Entities;

namespace PegsBase.Controllers
{
    [Authorize]
    public class LocalityController : Controller
    {
            private readonly ApplicationDbContext _dbContext;

            public LocalityController(ApplicationDbContext context)
            {
                _dbContext = context;
            }

        public IActionResult Index(int? subSectionId)
        {
            var localitiesQuery = _dbContext.Localities
                .Include(l => l.Level)
                .Include(l => l.SubSection)
                .OrderBy(l => l.Level.Name)
                .ThenBy(l => l.Name)
                .AsQueryable();

            if (subSectionId.HasValue)
            {
                localitiesQuery = localitiesQuery.Where(l => l.SubSectionId == subSectionId.Value);
            }

            var localities = localitiesQuery.ToList();

            ViewBag.Levels = new SelectList(_dbContext.Levels.OrderBy(l => l.Name), "Id", "Name");
            ViewBag.SubSections = new SelectList(_dbContext.SubSections.OrderBy(s => s.Name), "Id", "Name", subSectionId);

            return View(localities);
        }


        [HttpPost]
        public IActionResult CreateInline(string name, int levelId, int? subSectionId)
        {
            if (!string.IsNullOrWhiteSpace(name) && levelId > 0)
            {
                _dbContext.Localities.Add(new Locality
                {
                    Name = name.Trim(),
                    LevelId = levelId,
                    SubSectionId = subSectionId
                });
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult EditInline(int id, string name, int levelId, int? subSectionId)
        {
            var loc = _dbContext.Localities.Find(id);
            if (loc != null && !string.IsNullOrWhiteSpace(name))
            {
                loc.Name = name.Trim();
                loc.LevelId = levelId;
                loc.SubSectionId = subSectionId;
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var loc = _dbContext.Localities.Find(id);
            if (loc != null)
            {
                _dbContext.Localities.Remove(loc);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


    }
}
