using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var levels = _dbContext.Levels.OrderBy(l => l.Name).ToList();
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
        public IActionResult EditInline(int id, string name)
        {
            var level = _dbContext.Levels.Find(id);
            if (level != null && !string.IsNullOrWhiteSpace(name))
            {
                level.Name = name.Trim();
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
