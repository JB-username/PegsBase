using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models
{
    public class RawSurveyData
    {
        //Meta Data
        [Key]
        public int Id { get; set; }
        public string? Surveyor {  get; set; }
        public DateOnly SurveyDate { get; set; }
        public string? Locality { get; set; }

        //Pegs Used
        public string? StationPeg {  get; set; }
        public string? BackSightPeg { get; set; }
        public string? ForeSightPeg { get; set; }

        //Distance Measurements
        public decimal InstrumentHeight { get; set; }
        public decimal TargetHeightBacksight { get; set; }
        public decimal TargetHeightForesight { get; set; }
        public decimal SlopeDistance1Backsight { get; set; }
        public decimal SlopeDistance1Foresight { get; set; }
        public decimal SlopeDistance2Backsight { get; set; }
        public decimal SlopeDistance2Foresight { get; set; }

        //Horizontal Angle Measurements
        //Arc 1
        public decimal HorizontalAngle1Backsight { get; set; }
        public decimal HorizontalAngle1Foresight { get; set; }
        public decimal HorizontalTransit1Backsight { get; set; }
        public decimal HorizontalTransit1Foresight { get; set; }

        //Arc2
        public decimal HorizontalAngle2Backsight { get; set; }
        public decimal HorizontalAngle2Foresight { get; set; }
        public decimal HorizontalTransit2Backsight { get; set; }
        public decimal HorizontalTransit2Foresight { get; set; }
        
        //Vertical Angle Measurements
        //Arc 1
        public decimal VerticalAngle1Backsight { get; set; }
        public decimal VerticalAngle1Foresight { get; set; }
        public decimal VerticalTransit1Backsight { get; set; }
        public decimal VerticalTransit1Foresight { get; set; }

        //Arc2
        public decimal VerticalAngle2Backsight { get; set; }
        public decimal VerticalAngle2Foresight { get; set; }
        public decimal VerticalTransit2Backsight { get; set; }
        public decimal VerticalTransit2Foresight { get; set; }

        //Peg Status
        public bool PegFailed {  get; set; }
    }
}
