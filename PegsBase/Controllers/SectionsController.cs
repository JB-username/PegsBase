using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.Entities;

namespace PegsBase.Controllers
{
    [Authorize]
    public class SectionsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SectionsController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            var mainSections = _dbContext.MainSections
                .Include(ms => ms.SubSections)
                .OrderBy(ms => ms.Name)
                .ToList();
            return View(mainSections);
        }

        [HttpPost]
        public IActionResult CreateMainSection(string name, string description)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _dbContext.MainSections.Add(new MainSection
                {
                    Name = name.Trim(),
                    Description = description?.Trim()
                });
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditMainSection(int id, string name, string description)
        {
            var section = _dbContext.MainSections.Find(id);
            if (section != null && !string.IsNullOrWhiteSpace(name))
            {
                section.Name = name.Trim();
                section.Description = description?.Trim();
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteMainSection(int id)
        {
            var section = _dbContext.MainSections
                .Include(ms => ms.SubSections)
                .FirstOrDefault(ms => ms.Id == id);
            if (section != null)
            {
                _dbContext.MainSections.Remove(section);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateSubSection(int mainSectionId, string name, string description)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _dbContext.SubSections.Add(new SubSection
                {
                    Name = name.Trim(),
                    Description = description?.Trim(),
                    MainSectionId = mainSectionId
                });
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditSubSection(int id, string name, string description)
        {
            var subsection = _dbContext.SubSections.Find(id);
            if (subsection != null && !string.IsNullOrWhiteSpace(name))
            {
                subsection.Name = name.Trim();
                subsection.Description = description?.Trim();
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteSubSection(int id)
        {
            var subsection = _dbContext.SubSections.Find(id);
            if (subsection != null)
            {
                _dbContext.SubSections.Remove(subsection);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
