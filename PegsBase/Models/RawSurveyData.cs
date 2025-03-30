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

        //Calculation Results
        public decimal HorizontalDistanceBacksight { get; set; }
        public decimal HorizontalDistanceForesight { get; set; }

        public decimal VerticalDifferenceBacksight { get; set; }
        public decimal VerticalDifferenceForesight { get; set; }

        public decimal BackCheckHorizontalDistance { get; set; }
        public decimal BackCheckHorizontalDifference { get; set; }

        public decimal BackCheckPegElevations { get; set; }
        public decimal BackCheckVerticalError { get; set; }

        public decimal HAngleDirectReducedArc1 { get; set; }
        public decimal HAngleTransitReducedArc1 { get; set; }
        public decimal HAngleDirectReducedArc2 { get; set; }
        public decimal HAngleTransitReducedArc2 { get; set; }

        public decimal HAngleMeanArc1 { get; set; }
        public decimal HAngleMeanArc2 { get; set; }
        public decimal HAngleMeanFinal { get; set; }
        public decimal HAngleMeanFinalReturn { get; set; }

        public decimal VAngleBacksightMeanArc1 { get; set; }
        public decimal VAngleBacksightMeanArc2 { get; set; }
        public decimal VAngleBacksightMeanFinal { get; set; }

        public decimal VAngleForesightMeanArc1 { get; set; }
        public decimal VAngleForesightMeanArc2 { get; set; }
        public decimal VAngleForesightMeanFinal { get; set; }

        public decimal BackBearingReturn { get; set; }
        public decimal ForwardBearing { get; set; }
        public decimal ForwardBearingReturn { get; set; }

        public decimal StationPegX { get; set; }
        public decimal StationPegY { get; set; }
        public decimal StationPegZ { get; set; }

        public decimal BacksightPegX { get; set; }
        public decimal BacksightPegY { get; set; }
        public decimal BacksightPegZ { get; set; }

        public decimal NewPegX { get; set; }
        public decimal NewPegY { get; set; }
        public decimal NewPegZ { get; set; }

        public decimal DeltaZ { get; set; }
        public decimal DeltaX { get; set; }
        public decimal DeltaY { get; set; }
    }
}
