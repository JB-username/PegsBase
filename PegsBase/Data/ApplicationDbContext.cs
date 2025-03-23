using PegsBase.Models;
using Microsoft.EntityFrameworkCore;

namespace PegsBase.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ): base(options)
        {
                
        }

        public DbSet<PegRegister> PegRegister { get; set; }   
    }
}

//Four steps to add a table
//Create model class
//Add DB Set
//add-migration AddPegsDBTable
//update database
