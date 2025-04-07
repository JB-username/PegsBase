using Microsoft.AspNetCore.Identity;

namespace PegsBase.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? CompanyId { get; set; }
        public string? JobTitle { get; set; }
        public string? Department {  get; set; }
        public string? Section { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? ProfilePictureUrl { get; set; } // store image URL or relative path
        public DateTime? LastLoginAt { get; set; }
        public int LoginCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
