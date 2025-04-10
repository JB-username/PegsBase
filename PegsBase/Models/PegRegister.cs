using PegsBase.Models.Enums;
using PegsBase.Models.Entities;
using System.ComponentModel.DataAnnotations;
using PegsBase.Models.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PegsBase.Models
{
    public class PegRegister
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a peg name")]
        public required string PegName { get; set; }

        [Required(ErrorMessage = "Please select a level")]
        public int? LevelId {  get; set; }
        public Level? Level { get; set; }

        [Required(ErrorMessage = "Please select a locality")]
        public int? LocalityId { get; set; }
        public Locality? Locality { get; set; }
        public decimal YCoord { get; set; }
        public decimal XCoord { get; set; }
        public decimal ZCoord { get; set; }
        public decimal? GradeElevation { get; set; }

        public string? SurveyorId { get; set; }
        [ValidateNever]
        public ApplicationUser? Surveyor { get; set; }
        public string? SurveyorNameText { get; set; } //fallback if not in system
        public DateOnly SurveyDate { get; set; }

        [Required (ErrorMessage = "Must be of type: Peg, Control or Beacon")]
        public SurveyPointType PointType { get; set; }
        public bool PegFailed { get; set; }
        public bool HasPegCalc { get; set; }
        public string? FromPeg { get; set; }        
    }
}
