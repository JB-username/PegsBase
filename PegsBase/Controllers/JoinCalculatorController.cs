using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.QuickCalcs;
using PegsBase.Services.QuickCalcs.Interfaces;


namespace PegsBase.Controllers
{
    public class JoinCalculatorController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IJoinCalculatorService _calculator;

        public JoinCalculatorController(
            ApplicationDbContext ctx,
            IJoinCalculatorService calculator)
        {
            _dbContext = ctx;
            _calculator = calculator;
        }

        // GET: show form
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pegs = await _dbContext.PegRegister
                                 .OrderBy(p => p.PegName)
                                 .Select(p => new {
                                     p.Id,
                                     p.PegName
                                 }).ToListAsync();

            var vm = new JoinCalculatorViewModel
            {
                PegOptions = pegs
                  .Select(p => new SelectListItem(p.PegName, p.Id.ToString()))
                  .ToList()
            };
            return View(vm);
        }

        // POST: process selection
        [HttpPost]
        public async Task<IActionResult> Index(JoinCalculatorViewModel vm)
        {
            // repopulate dropdown
            vm.PegOptions = await _dbContext.PegRegister
                .OrderBy(p => p.PegName)
                .Select(p => new SelectListItem(p.PegName, p.Id.ToString()))
                .ToListAsync();

            if (vm.FirstPegId == vm.SecondPegId)
            {
                ModelState.AddModelError("", "Please select two different pegs.");
                return View(vm);
            }

            var points = await _dbContext.PegRegister
                .Where(p => p.Id == vm.FirstPegId || p.Id == vm.SecondPegId)
                .ToListAsync();

            if (points.Count != 2)
            {
                ModelState.AddModelError("", "One or both pegs not found.");
                return View(vm);
            }

            var a = points.Single(p => p.Id == vm.FirstPegId);
            var b = points.Single(p => p.Id == vm.SecondPegId);

            vm.StartX = a.XCoord;
            vm.StartY = a.YCoord;
            vm.StartZ = a.ZCoord;

            vm.EndX = b.XCoord;
            vm.EndY = b.YCoord;
            vm.EndZ = b.ZCoord;

            vm.Result = _calculator.Calculate(
                a.XCoord, a.YCoord, a.ZCoord,
                b.XCoord, b.YCoord, b.ZCoord
            );

            return View(vm);
        }
    }
}
