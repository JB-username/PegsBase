using Microsoft.EntityFrameworkCore;
using PegsBase.Models.MinePlans;

namespace PegsBase.Data
{
    public static class MinePlanTypeSeeder
    {
        private static readonly List<string> DefaultPlanTypes = new()
        {
            "Index Key Plan",
            "Surface Plan",
            "Surface Contour Plan",
            "Mine Vent And Rescue Plan",
            "Mine Plan",
            "Rehabilitation Plan",
            "Residue Deposit",
            "Geological",
            "Workings",
            "Vertical Section",
            "Level Plan",
            "General Plan",
            "Cross Sections",
            "MO Plan"
        };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // ensure the database is created/migrated
            await db.Database.MigrateAsync();

            // only insert if empty
            if (await db.PlanTypes.AnyAsync())
                return;

            foreach (var name in DefaultPlanTypes)
            {
                if (!await db.PlanTypes.AnyAsync(pt => pt.Name == name))
                {
                    db.PlanTypes.Add(new PlanType { Name = name });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
