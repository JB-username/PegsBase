using PegsBase.Models.Enums;
using PegsBase.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.JobRequests
{
    public class CreateJobRequestViewModel
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        public string? AssignedToUserId { get; set; } // single user (optional)
        public string? TargetDepartment { get; set; } // department (optional)
        public string? TargetSection { get; set; } // section (optional)
    }
}
