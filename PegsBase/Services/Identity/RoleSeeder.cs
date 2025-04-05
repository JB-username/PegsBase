using Microsoft.AspNetCore.Identity;
using PegsBase.Models.Constants;

namespace PegsBase.Services.Identity
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(Roles.Master))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Master));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User));
            }

            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Viewer))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Viewer));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Mrm))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Mrm));
            }

            if (!await roleManager.RoleExistsAsync(Roles.MineSurveyor))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.MineSurveyor));
            }

            if (!await roleManager.RoleExistsAsync(Roles.SurveyAnalyst))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SurveyAnalyst));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Surveyor))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Surveyor));
            }

            if (!await roleManager.RoleExistsAsync(Roles.MineManager))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.MineManager));
            }

            if (!await roleManager.RoleExistsAsync(Roles.MineOverseer))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.MineOverseer));
            }
        }
    }
}
