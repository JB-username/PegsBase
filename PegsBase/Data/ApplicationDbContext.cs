using PegsBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PegsBase.Models.Identity;

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
    }
}

//Four steps to add a table
//Create model class
//Add DB Set
//add-migration AddPegsDBTable
//update database
