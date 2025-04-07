using Microsoft.AspNetCore.Http;
using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

public class MinePlanUploadViewModel
{
    [Required]
    public string PlanName { get; set; }

    [Required]
    public string Level { get; set; }

    [Required]
    public string Locality { get; set; }

    public int Scale { get; set; }

    public bool IsSigned { get; set; }

    [Required]
    public MinePlanType PlanType { get; set; }

    [Required]
    public IFormFile File { get; set; }
}

