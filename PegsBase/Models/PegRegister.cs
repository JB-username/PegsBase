using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models
{
    public class PegRegister
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a peg name")]
        public required string PegName { get; set; }
        public int Level { get; set; }
        [Required]
        [StringLength(200,MinimumLength = 2, ErrorMessage = "Locality must be between 2 and 200 characters!")]
        public required string Locality { get; set; }
        public decimal YCoord { get; set; }
        public decimal XCoord { get; set; }
        public decimal ZCoord { get; set; }
        public decimal? GradeElevation { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 5 and 100 characters!")]
        public string Surveyor { get; set; } = string.Empty;
        public DateOnly SurveyDate { get; set; }
        [Required (ErrorMessage = "Must be of type: Peg, Control or Beacon")]
        public SurveyPointType PointType { get; set; }
        public bool PegFailed { get; set; }
        public bool HasPegCalc { get; set; }
        public string? FromPeg { get; set; }
    }
}
