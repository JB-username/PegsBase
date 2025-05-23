﻿using PegsBase.Models.Entities;
using PegsBase.Models.Enums;
using PegsBase.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.ViewModels
{
    public class PegCalcViewModel
    {
        //Meta Data
        [Key]
        public int Id { get; set; }
        public string? SurveyorId { get; set; }
        public string? Surveyor { get; set; }
        public string? SurveyorNameText { get; set; }
        public DateOnly SurveyDate { get; set; }
        public int LocalityId { get; set; }
        public string? Locality { get; set; }
        public int LevelId { get; set; }
        public int Level { get; set; }
        public decimal GradeElevation { get; set; }
        public SurveyPointType PointType { get; set; } = SurveyPointType.Peg;

        public string? SurveyorDisplayName { get; set; }
        public string? LocalityName { get; set; }
        public string? LevelName { get; set; }


        public List<Level> Levels { get; set; } = new List<Level>();
        public List<Locality> Localities { get; set; } = new List<Locality>();
        public List<ApplicationUser> Surveyors { get; set; } = new List<ApplicationUser>();


        //Pegs Used
        public string? StationPeg { get; set; }
        public string? BackSightPeg { get; set; }
        public string? ForeSightPeg { get; set; }

        //Distance Measurements
        public decimal InstrumentHeight { get; set; }
        public decimal TargetHeightBacksight { get; set; }
        public decimal TargetHeightForesight { get; set; }

        public decimal SlopeDistanceBacksight { get; set; }
        public decimal SlopeDistanceForesight { get; set; }

        public decimal HorizontalDistanceBacksight { get; set; }
        public decimal HorizontalDistanceForesight { get; set; }

        public decimal VerticalDifferenceBacksight { get; set; }
        public decimal VerticalDifferenceForesight { get; set; }

        public decimal BackCheckHorizontalDistance { get; set; }
        public decimal BackCheckHorizontalDifference { get; set; }

        public decimal BackCheckPegElevations { get; set; }
        public decimal BackCheckVerticalError { get; set; }
        
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

        //Reduced Angles
        public decimal HAngleDirectReducedArc1 { get; set;}
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

        //Bearing Values
        public decimal BackBearingReturn { get; set; }
        public decimal ForwardBearing { get; set; }
        public decimal ForwardBearingReturn { get; set; }

        //Peg Status
        public bool PegFailed { get; set; }
       
        //Coordinates
        public decimal StationPegX {  get; set; }
        public decimal StationPegY { get; set; }
        public decimal StationPegZ { get; set; }

        public decimal BacksightPegX { get; set; }
        public decimal BacksightPegY { get; set; }
        public decimal BacksightPegZ { get; set; }

        public decimal NewPegX { get; set; }
        public decimal NewPegY { get; set; }
        public decimal NewPegZ { get; set; }

        //Deltas
        public decimal DeltaZ { get; set; }
        public decimal DeltaX { get; set; }
        public decimal DeltaY { get; set; }

        public string FormatOrDefault(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "000:00:00" : value;
        }

        public string FormatDMS(decimal dms)
        {
            int deg = (int)Math.Floor(dms);
            int min = (int)((dms - deg) * 100);

            // Calculate seconds and round to nearest whole number
            decimal secDecimal = ((dms - deg) * 100 - min) * 100;
            int sec = (int)Math.Round(secDecimal, MidpointRounding.AwayFromZero);

            // Handle rounding overflow (e.g., 59.9 → 60)
            if (sec == 60)
            {
                sec = 0;
                min++;

                if (min == 60)
                {
                    min = 0;
                    deg++;
                }
            }

            return $"{deg:D3}:{min:D2}:{sec:D2}";
        }

    }
}

