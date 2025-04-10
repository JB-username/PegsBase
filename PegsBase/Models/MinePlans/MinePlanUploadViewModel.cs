using Microsoft.AspNetCore.Http;
using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

public class MinePlanUploadViewModel
{
    [Required]
    public string PlanName { get; set; }

    public int LevelId { get; set; }

    public int? LocalityId { get; set; }

    public int Scale { get; set; }

    public bool IsSigned { get; set; }

    [Required]
    public MinePlanType PlanType { get; set; }

    [Required]
    public IFormFile File { get; set; }
}

