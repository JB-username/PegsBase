namespace PegsBase.Models.MinePlans
{
    public class MinePlanDeleteViewModel
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = "";

        public string? PlanTypeName { get; set; }
        public string? LevelName { get; set; }
        public string? LocalityName { get; set; }

        public decimal Scale { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
