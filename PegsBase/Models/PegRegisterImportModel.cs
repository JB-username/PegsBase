using PegsBase.Models.Enums;

namespace PegsBase.Models
{
    public class PegRegisterImportModel
    {
        public string PegName { get; set; } = string.Empty;
        public decimal XCoord { get; set; }
        public decimal YCoord { get; set; }
        public decimal ZCoord { get; set; }
        public decimal? GradeElevation { get; set; }

        public string? SurveyorName { get; set; }
        public string? LocalityName { get; set; }
        public string? LevelName { get; set; }

        public DateOnly? SurveyDate { get; set; }
        public SurveyPointType PointType { get; set; }
        public bool PegFailed { get; set; } = false;
    }
}
