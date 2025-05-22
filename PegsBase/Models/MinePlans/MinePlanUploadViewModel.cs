using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PegsBase.Models.Enums;
using PegsBase.Models.MinePlans;
using System.ComponentModel.DataAnnotations;

public class MinePlanUploadViewModel
{
    [Required]
    [Display(Name = "Plan Name")]
    public string PlanName { get; set; } = string.Empty;

    [Required]
    public IFormFile File { get; set; }

    [Required(ErrorMessage = "Please select a mine plan type")]
    public int? PlanTypeId { get; set; }

    public PlanType? PlanType { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Scale { get; set; }

    [Display(Name = "Signed?")]
    public bool IsSigned { get; set; }

    [Required]
    public int LevelId { get; set; }

    [Required]
    public int LocalityId { get; set; }


    public IEnumerable<SelectListItem>? LevelOptions { get; set; }
    public IEnumerable<SelectListItem>? LocalityOptions { get; set; }
    public IEnumerable<SelectListItem>? PlanTypeOptions { get; set; }
}

