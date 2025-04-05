using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PegsBase.Models;

namespace PegsBase.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Upload()
        {
            var model = new TestCoordinateUploadViewModel
            {
                PreviewRows = new List<TestCoordinateRow>
            {
                new TestCoordinateRow { PegName = "A001", SaveToDatabase = true },
                new TestCoordinateRow { PegName = "A002", SaveToDatabase = false },
                new TestCoordinateRow { PegName = "A003", SaveToDatabase = false }
            }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Upload(TestCoordinateUploadViewModel model)
        {
            for (int i = 0; i < model.PreviewRows.Count; i++)
            {
                var row = model.PreviewRows[i];
                System.Diagnostics.Debug.WriteLine($"Row {i}: PegName={row.PegName}, SaveToDatabase={row.SaveToDatabase}");
            }

            TempData["Message"] = "Form submitted!";
            return View(model);
        }
    }
}
