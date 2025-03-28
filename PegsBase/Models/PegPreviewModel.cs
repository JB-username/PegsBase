using System.ComponentModel.DataAnnotations;
using PegsBase.Models.Enums;

namespace PegsBase.Models
{
    public class PegPreviewModel
    {
        [Required]
        public required string PegName { get; set; }
        public decimal XCoord { get; set; }
        public decimal YCoord { get; set; }
        public decimal ZCoord { get; set; }
        public decimal? GradeElevation { get; set; }

        // Editable by user in UI later
        public string? Surveyor { get; set; }
        public string? Locality { get; set; }
        public DateOnly? SurveyDate { get; set; }
        public int? Level { get; set; }
        public SurveyPointType? Type { get; set; }
        public bool SaveToDatabase { get; set; } = false;

    }
}
