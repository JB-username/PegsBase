using PegsBase.Models.Enums;
using PegsBase.Models.Identity;

namespace PegsBase.Models.JobRequests
{
    public class JobRequest
    {
        public int Id { get; set; }

        // Sender
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        // Receiver (nullable if it's department/section-based only)
        public string? AssignedToUserId { get; set; }
        public ApplicationUser? AssignedTo { get; set; }

        // Optional targeting by section/department
        public string? TargetDepartment { get; set; }
        public string? TargetSection { get; set; }

        public string Subject { get; set; }
        public string Description { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Pending;
        public string? DeclineReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}
