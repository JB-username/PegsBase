using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Models.Entities;
using PegsBase.Models.Identity;
using PegsBase.Services.Parsing.Interfaces;

namespace PegsBase.Services.Parsing
{
    public class MapImportModelsToPegs : IMapImportModelsToPegs
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MapImportModelsToPegs(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<List<PegRegister>> MapAsync(List<PegRegisterImportModel> importModels)
        {
            var levels = await _dbContext.Levels.ToListAsync();
            var localities = await _dbContext.Localities.ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            var result = new List<PegRegister>();

            foreach (var row in importModels)
            {
                var peg = new PegRegister
                {
                    PegName = row.PegName,
                    XCoord = row.XCoord,
                    YCoord = row.YCoord,
                    ZCoord = row.ZCoord,
                    GradeElevation = row.GradeElevation,
                    SurveyDate = row.SurveyDate.HasValue
                            ? row.SurveyDate.Value
                            : DateOnly.FromDateTime(DateTime.Today),
                    PointType = row.PointType,
                    PegFailed = false,
                    HasPegCalc = false
                };

                var level = levels.FirstOrDefault(l => string.Equals(l.Name, row.LevelName, StringComparison.OrdinalIgnoreCase));
                if (level == null && !string.IsNullOrWhiteSpace(row.LevelName))
                {
                    level = new Level { Name = row.LevelName.Trim() };
                    _dbContext.Levels.Add(level);
                    levels.Add(level); // Add to local list to prevent duplicates
                }

                var locality = localities.FirstOrDefault(l => string.Equals(l.Name, row.LocalityName, StringComparison.OrdinalIgnoreCase));
                if (locality == null && !string.IsNullOrWhiteSpace(row.LocalityName))
                {
                    locality = new Locality
                    {
                        Name = row.LocalityName.Trim(),
                        Level = level // optional: assign level
                    };
                    _dbContext.Localities.Add(locality);
                    localities.Add(locality); // Prevent duplicate additions
                }

                peg.LevelId = level?.Id;
                peg.LocalityId = locality?.Id;

                var normalizedInput = Normalize(row.SurveyorName);
                var surveyor = users.FirstOrDefault(u => u.NormalizedFullName == normalizedInput);
                peg.SurveyorId = surveyor?.Id;

                result.Add(peg);
            }
            await _dbContext.SaveChangesAsync();

            return result;
        }

        private static string Normalize(string input)
        {
            return input?
                .Replace(".", "")
                .Replace(" ", "")
                .ToUpperInvariant()
                .Trim();
        }

    }
}
