using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.MinePlans
{
    public class MinePlanEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; } = string.Empty;
      
        [Required]
        public int PlanTypeId { get; set; }
        [Display(Name = "Plan Type")]
        public List<SelectListItem> PlanTypeOptions { get; set; } = [];

        [Required]
        [Range(1, int.MaxValue)]
        public int Scale { get; set; }

        [Display(Name = "Signed?")]
        public bool IsSigned { get; set; }

        [Display(Name = "Superseded?")]
        public bool IsSuperseded { get; set; }

        [Display(Name = "Superseded Reference")]
        public string? SupersededReference { get; set; }

        [Required]
        public int LevelId { get; set; }

        [Required]
        public int LocalityId { get; set; }

        public DateTime UploadedAt { get; set; }

        public List<SelectListItem>? LevelOptions { get; set; } = [];
        public List<SelectListItem>? LocalityOptions { get; set; } = [];

    }
}
