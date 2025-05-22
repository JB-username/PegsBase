using Microsoft.AspNetCore.Mvc.Rendering;
using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.MinePlans
{
    public class MinePlanEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; }


        [Required]
        public int PlanTypeId { get; set; }
        [Display(Name = "Plan Type")]
        public MinePlanType PlanType { get; set; }

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

        public IEnumerable<SelectListItem>? LevelOptions { get; set; }
        public IEnumerable<SelectListItem>? LocalityOptions { get; set; }
        public IEnumerable<SelectListItem>? PlanTypeOptions { get; set; }

    }
}
