using PegsBase.Models.Entities;
using PegsBase.Models.Enums;
using PegsBase.Models.Identity;
using System.ComponentModel.DataAnnotations;

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

        // Editable metadata
        public string? SurveyorId { get; set; }        // ✅ FK to ApplicationUser
        public ApplicationUser? Surveyor { get; set; } // Optional navigation

        public int? LocalityId { get; set; }           // ✅ FK to Locality
        public Locality? Locality { get; set; }

        public int? LevelId { get; set; }              // ✅ FK to Level
        public Level? Level { get; set; }

        public DateOnly? SurveyDate { get; set; }

        public SurveyPointType? Type { get; set; }     // Peg, Control, Beacon
        public bool SaveToDatabase { get; set; } = false;
    }
}
