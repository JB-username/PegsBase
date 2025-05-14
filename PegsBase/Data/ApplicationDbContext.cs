using PegsBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PegsBase.Models.Identity;
using PegsBase.Models.MinePlans;
using PegsBase.Models.JobRequests;
using PegsBase.Models.Entities;

namespace PegsBase.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ): base(options)
        {
                
        }

        public DbSet<PegRegister> PegRegister { get; set; }
        public DbSet<RawSurveyData> RawSurveyData { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<SurveyNote> SurveyNotes { get; set; }
        public DbSet<WhitelistedEmails> WhitelistedEmails { get; set; }
        public DbSet<MinePlan> MinePlans { get; set; }
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        public DbSet<Level> Levels { get; set; }
        public DbSet<Locality> Localities { get; set; }
        public DbSet<MainSection> MainSections { get; set; }
        public DbSet<SubSection> SubSections { get; set; }

        public DbSet<AppSettings> AppSettings { get; set; }


    }
}

//Four steps to add a table
//Create model class
//Add DB Set
//add-migration AddPegsDBTable
//update database
