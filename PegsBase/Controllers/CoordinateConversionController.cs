using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.Constants;
using PegsBase.Models.QuickCalcs;
using PegsBase.Services.QuickCalcs.Interfaces;
using Npgsql;
using NpgsqlTypes;
using System.Data.Common;

namespace PegsBase.Controllers
{
    [Route("coords")]
    public class CoordinateConversionController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICoordinateConversionService _svc;

        public CoordinateConversionController(
            ApplicationDbContext dbContext,
            ICoordinateConversionService svc)
        {
            _dbContext = dbContext;
            _svc = svc;
        }

        private async Task<List<SelectListItem>> LoadPegOptionsAsync()
        {
            return await _dbContext.PegRegister
                .OrderBy(p => p.PegName)
                .Select(p => new SelectListItem
                {
                    Text = p.PegName,
                    Value = p.Id.ToString()
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> LoadSridOptionsAsync()
        {
            var sridValues = Enum.GetValues(typeof(SaSrid))
                                 .Cast<int>()
                                 .ToArray();

            var list = new List<SelectListItem>();
            var conn = _dbContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                      SELECT srid, srtext
                        FROM public.spatial_ref_sys
                       WHERE srid = ANY(@srids)
                       ORDER BY srid;
                    ";

            var p = new NpgsqlParameter("srids", NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = sridValues
            };
            cmd.Parameters.Add(p);

            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                var srid = rdr.GetInt32(0);
                var wkt = rdr.GetString(1);

                var q1 = wkt.IndexOf('"');
                var q2 = q1 >= 0 ? wkt.IndexOf('"', q1 + 1) : -1;
                var name = (q1 >= 0 && q2 > q1)
                    ? wkt[(q1 + 1)..q2]
                    : $"EPSG:{srid}";

                list.Add(new SelectListItem
                {
                    Value = srid.ToString(),
                    Text = $"EPSG:{srid} — {name}"
                });
            }

            return list;
        }

        private async Task PopulateLookupData()
        {
            ViewBag.Levels = await _dbContext.Levels.OrderBy(l => l.Name).ToListAsync();
            ViewBag.Localities = await _dbContext.Localities.OrderBy(l => l.Name).ToListAsync();
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await PopulateLookupData();
            var vm = new CoordinateConversionViewModel
            {
                SridOptions = await LoadSridOptionsAsync(),
                SourceSrid = (int)SaSrid.Wgs84,
                TargetSrid = (int)SaSrid.Utm33S,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CoordinateConversionViewModel vm)
        {

            await PopulateLookupData();

            vm.SridOptions = await LoadSridOptionsAsync();

            if (vm.SelectedPegIds == null || !vm.SelectedPegIds.Any())
            {
                ModelState.AddModelError(
                    nameof(vm.SelectedPegIds),
                    "Please select at least one peg."
                );
                return View(vm);
            }

            var selectedPegs = await _dbContext.PegRegister
                .Where(p => vm.SelectedPegIds.Contains(p.Id))
                .ToListAsync();

            vm.Results = new List<CoordinateConversionResult>();

            try
            {
                foreach (var peg in selectedPegs)
                {
                    var conv = await _svc.ConvertAsync(
                        (double)peg.XCoord, 
                        (double)peg.YCoord,
                        vm.SourceSrid, 
                        vm.TargetSrid
                    );

                    vm.Results.Add(new CoordinateConversionResult
                    {
                        PegId = peg.Id,
                        PegName = peg.PegName,
                        OrigX = (double)peg.XCoord,
                        OrigY = (double)peg.YCoord,
                        OrigZ = (double?)peg.ZCoord,
                        X = conv.X,
                        Y = conv.Y,
                        Z = conv.Z
                    });
                }

                TempData["Success"] = $"{vm.Results.Count} peg(s) converted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Conversion failed: " + ex.Message;
                return View(vm);
            }

            return View(vm);
        }

        [HttpGet("GetFilteredPegs")]
        public async Task<IActionResult> GetFilteredPegs(
        string search,
        int? levelId,
        int? localityId,
        [FromQuery] List<int> selectedPegIds)
        {
            var q = _dbContext.PegRegister.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(p => p.PegName.Contains(search));

            if (levelId.HasValue)
                q = q.Where(p => p.LevelId == levelId.Value);

            if (localityId.HasValue)
                q = q.Where(p => p.LocalityId == localityId.Value);

            var pegs = await q.OrderBy(p => p.PegName)
                              .Take(1000)   // maybe page or cap it
                              .ToListAsync();

            var lookupVm = new CoordinateConversionPegLookupViewModel
            {
                Pegs = pegs,
                SelectedPegIds = selectedPegIds
            };

            return PartialView("_PegLookup", lookupVm);
        }


    };
}
