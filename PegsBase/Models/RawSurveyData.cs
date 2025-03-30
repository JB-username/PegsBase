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
        public decimal SlopeDistanceBacksight { get; set; }
        public decimal SlopeDistanceForesight { get; set; }

        //Horizontal Angle Measurements
        //Arc 1
        public decimal HAngleDirectArc1Backsight { get; set; }
        public decimal HAngleDirectArc1Foresight { get; set; }
        public decimal HAngleTransitArc1Backsight { get; set; }
        public decimal HAngleTransitArc1Foresight { get; set; }

        //Arc2
        public decimal HAngleDirectArc2Backsight { get; set; }
        public decimal HAngleDirectArc2Foresight { get; set; }
        public decimal HAngleTransitArc2Backsight { get; set; }
        public decimal HAngleTransitArc2Foresight { get; set; }



        //Vertical Angle Measurements
        //Arc 1
        public decimal VAngleDirectArc1Backsight { get; set; }
        public decimal VAngleDirectArc1Foresight { get; set; }
        public decimal VAngleTransitArc1Backsight { get; set; }
        public decimal VAngleTransitArc1Foresight { get; set; }

        //Arc2
        public decimal VAngleDirectArc2Backsight { get; set; }
        public decimal VAngleDirectArc2Foresight { get; set; }
        public decimal VAngleTransitArc2Backsight { get; set; }
        public decimal VAngleTransitArc2Foresight { get; set; }

        //Peg Status
        public bool PegFailed {  get; set; }
    }
}
