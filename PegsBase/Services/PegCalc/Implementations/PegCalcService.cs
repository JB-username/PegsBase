using PegsBase.Models;
using PegsBase.Models.ViewModels;
using PegsBase.Services.PegCalc.Implementations;
using PegsBase.Services.PegCalc.Interfaces;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace PegsBase.Services.PegCalc.Implementations
{
    public class PegCalcService : IPegCalcService
    {
        public PegCalcViewModel CalculatePeg(PegCalcViewModel vm, PegRegister setup, PegRegister backsight)
        {
            // Arc 1
            var arc1 = CalcArc(
                vm.HAngleDirectArc1Backsight, 
                vm.HAngleDirectArc1Foresight, 
                vm.HAngleTransitArc1Backsight, 
                vm.HAngleTransitArc1Foresight);


            vm.HAngleDirectReducedArc1 = arc1.Direct;

            vm.HAngleTransitReducedArc1 = arc1.Transit;

            decimal meanHorizontalArc1 = arc1.Mean;
            decimal meanHorizontalFinal = arc1.Mean;
            
            decimal meanHorizontalArc2 = 0;
                        
            // Arc 2 (if present)
            if (vm.HAngleDirectArc2Backsight != 0 && vm.HAngleDirectArc2Foresight != 0)
            {
                var arc2 = CalcArc(
                    vm.HAngleDirectArc2Backsight, 
                    vm.HAngleDirectArc2Foresight, 
                    vm.HAngleTransitArc2Backsight, 
                    vm.HAngleTransitArc2Foresight);

                meanHorizontalArc2 = arc2.Mean;
                vm.HAngleDirectReducedArc2 = arc2.Direct;
                vm.HAngleTransitReducedArc2 = arc2.Transit;

                meanHorizontalFinal = DegToDMS((DMSToDec(arc1.Mean) + DMSToDec(arc2.Mean)) / 2);
            }

            vm.HAngleMeanArc1 = meanHorizontalArc1;
            vm.HAngleMeanArc2 = meanHorizontalArc2;

            vm.HAngleMeanFinal = meanHorizontalFinal;
            vm.HAngleMeanFinalReturn = DegToDMS(360 - DMSToDec(meanHorizontalFinal));

            // Bearings
            decimal backBearing = vm.HAngleDirectArc1Backsight;
            
            decimal backBearingReturn = (backBearing > 180) 
                ? DegToDMS(DMSToDec(backBearing) - 180) 
                : DegToDMS(DMSToDec(backBearing) + 180);
            vm.BackBearingReturn = Math.Round(backBearingReturn,6);

            decimal forwardBearing = DMSToDec(backBearing) + DMSToDec(meanHorizontalFinal);

            if (forwardBearing > 360) forwardBearing -= 360;

            vm.ForwardBearing = DecToDMS(Math.Round(forwardBearing, 6));

            decimal forwardBearingReturn = (forwardBearing > 180)
                ? DegToDMS(forwardBearing) - 180
                : DegToDMS(forwardBearing) + 180;
            vm.ForwardBearingReturn = Math.Round(forwardBearingReturn, 6);

            // Vertical angles & reductions
            // Backsight VA
            decimal backsightVAReducedArc1 = 90 - DMSToDec(vm.VAngleDirectArc1Backsight);
            decimal backsightVATransitReducedArc1 = DMSToDec(vm.VAngleTransitArc1Backsight) - 270;
            
            decimal vABacksightMeanArc1 = (backsightVAReducedArc1 + backsightVATransitReducedArc1) / 2;
            vm.VAngleBacksightMeanArc1 = DegToDMS(vABacksightMeanArc1);

            decimal vABacksightMeanFinal = vABacksightMeanArc1;

            if (vm.VAngleDirectArc2Backsight != 0 && vm.VAngleTransitArc2Backsight != 0)
            {
                decimal backsightVAReducedArc2 = 90 - DMSToDec(vm.VAngleDirectArc2Backsight);
                decimal backsightVATransitReducedArc2 = DMSToDec(vm.VAngleTransitArc2Backsight) - 270;

                decimal vABacksightMeanArc2 = (backsightVAReducedArc2 + backsightVATransitReducedArc2) / 2;
                vm.VAngleBacksightMeanArc2 = DegToDMS(vABacksightMeanArc2);

                vABacksightMeanFinal = (vABacksightMeanArc1 + vABacksightMeanArc2) / 2;
            }

            vm.VAngleBacksightMeanFinal = DegToDMS(vABacksightMeanFinal);

            //Foresight VA
            decimal foresightVAReducedArc1 = 90 - DMSToDec(vm.VAngleDirectArc1Foresight);
            decimal foresightVATransitReducedArc1 = DMSToDec(vm.VAngleTransitArc1Foresight) - 270;

            decimal vAForesightMeanArc1 = (foresightVAReducedArc1 + foresightVATransitReducedArc1) / 2;
            vm.VAngleForesightMeanArc1 = DegToDMS(vAForesightMeanArc1);

            decimal vAForesightMeanFinal = vAForesightMeanArc1;

            if (vm.VAngleDirectArc2Foresight != 0 && vm.VAngleTransitArc2Foresight != 0)
            {
                decimal foresightVAReducedArc2 = 90 - DMSToDec(vm.VAngleDirectArc2Foresight);
                decimal foresightVATransitReducedArc2 = DMSToDec(vm.VAngleTransitArc2Foresight) - 270;

                decimal vAforesightMeanArc2 = (foresightVAReducedArc2 + foresightVATransitReducedArc2) / 2;
                vm.VAngleForesightMeanArc2 = DegToDMS(vAforesightMeanArc2);

                vAForesightMeanFinal = (vAForesightMeanArc1 + vAforesightMeanArc2) / 2;
            }

            vm.VAngleForesightMeanFinal = DegToDMS(vAForesightMeanFinal);


            // Distances
            decimal slopeBS = vm.SlopeDistanceBacksight;

            decimal slopeFS = vm.SlopeDistanceForesight;

            decimal radBS = vABacksightMeanFinal * (decimal)Math.PI / 180;
            decimal radFS = vAForesightMeanFinal * (decimal)Math.PI / 180;

            decimal vdBS = slopeBS * (decimal)Math.Sin((double)radBS);
            decimal hdBS = slopeBS * (decimal)Math.Cos((double)radBS);

            decimal vdFS = slopeFS * (decimal)Math.Sin((double)radFS);
            decimal hdFS = slopeFS * (decimal)Math.Cos((double)radFS);

            vm.HorizontalDistanceBacksight = Math.Round(hdBS, 5);
            vm.HorizontalDistanceForesight = Math.Round(hdFS, 5);

            vm.VerticalDifferenceForesight = Math.Round(vdFS, 5);

            // Back check HD
            decimal backCheckHD = HD(setup, backsight);
            vm.BackCheckHorizontalDistance = Math.Round(backCheckHD, 5);
            vm.BackCheckHorizontalDifference = Math.Round(hdBS - backCheckHD, 5);

            // Elevations
            decimal elevDiff = backsight.ZCoord - setup.ZCoord;
            decimal verticalToBack = vm.InstrumentHeight + vm.TargetHeightBacksight + vdBS;
            decimal verticalToFront = vm.InstrumentHeight + vm.TargetHeightForesight + vdFS;

            vm.BackCheckPegElevations = Math.Round(elevDiff, 5);
            vm.VerticalDifferenceBacksight = Math.Round(verticalToBack, 5);
            vm.BackCheckVerticalError = Math.Round(verticalToBack - elevDiff, 5);
            vm.DeltaZ = Math.Round(verticalToFront, 5);

            // Final Coords
            decimal forwardRad = forwardBearing * (decimal)Math.PI / 180;
            decimal dY = hdFS * (decimal)Math.Sin((double)forwardRad);
            decimal dX = hdFS * (decimal)Math.Cos((double)forwardRad);

            vm.DeltaY = Math.Round(dY, 5);
            vm.DeltaX = Math.Round(dX, 5);

            vm.NewPegX = Math.Round(setup.XCoord + dX, 5);
            vm.NewPegY = Math.Round(setup.YCoord + dY, 5);
            vm.NewPegZ = Math.Round(setup.ZCoord + verticalToFront, 5);

            vm.StationPegX = setup.XCoord;
            vm.StationPegY = setup.YCoord;
            vm.StationPegZ = setup.ZCoord;

            vm.BacksightPegX = backsight.XCoord;
            vm.BacksightPegY = backsight.YCoord;
            vm.BacksightPegZ = backsight.ZCoord;

            return vm;
        }

        #region Helpers
        private (
            decimal BacksightDir, 
            decimal ForesightDir, 
            decimal BacksightTrn, 
            decimal ForesightTrn, 
            decimal Direct, 
            decimal Transit, 
            decimal Mean) 
            CalcArc(params decimal?[] angles)
        {
            decimal bsd = DMSToDec(angles[0] ?? 0);
            decimal fsd = DMSToDec(angles[1] ?? 0);
            decimal bst = DMSToDec(angles[2] ?? 0);
            decimal fst = DMSToDec(angles[3] ?? 0);

            decimal direct = fsd - bsd < 0 ? fsd - bsd + 360 :fsd - bsd;
            decimal transit = fst - bst < 0 ? fst - bst + 360 :fst - bst;
            decimal mean = (direct + transit) / 2;

            return (bsd, fsd, bst, fst, DegToDMS(direct), DegToDMS(transit), DegToDMS(mean));
        }

        private decimal VerticalMean(decimal? va, decimal? trnVa)
        {
            decimal a = 90 - DMSToDec(va ?? 0);
            decimal b = (DMSToDec(trnVa ?? 0) - 270);
            return ((a + b) / 2);
        }

        private decimal DMSToDec(decimal dms)
        {
            int degrees = (int)Math.Floor(dms);
            int minutes = (int)Math.Floor((dms - degrees) * 100);
            decimal seconds = (dms - degrees - (minutes / 100m)) * 10000m;
            return degrees + (minutes / 60m) + (seconds / 3600m);
        }

        private decimal DegToDMS(decimal dec, int secondPrecision = 6)
        {
            decimal deg = Math.Floor(dec);
            decimal minutesDecimal = (dec - deg) * 60m;
            decimal min = Math.Floor(minutesDecimal);
            decimal sec = Math.Round((minutesDecimal - min) * 60m, secondPrecision);

            // Ensure seconds don't round to 60
            if (sec >= 60m)
            {
                sec = 0;
                min += 1;
            }

            // Ensure minutes don't round to 60
            if (min >= 60m)
            {
                min = 0;
                deg += 1;
            }

            // Format as DDD.MMSSssss (flattened into string, then parsed)
            string minStr = ((int)min).ToString("D2"); // Always 2 digits
            string secStr = sec.ToString($"00.{new string('0', secondPrecision)}", CultureInfo.InvariantCulture);

            // Remove the decimal point from secStr
            secStr = secStr.Replace(".", "");

            string combined = $"{deg}.{minStr}{secStr}";
            return decimal.Parse(combined, CultureInfo.InvariantCulture);
        }




        //private decimal DegToDMS(decimal dec)
        //{
        //    var deg = Math.Floor(dec);
        //    var min = Math.Floor((dec - deg) * 60);
        //    var sec = ((dec - deg - (min / 60m)) * 3600m);
        //    return deg + (min / 100m) + (sec / 10000m);
        //}



        private decimal Normalize360(decimal angle)
        {
            return angle >= 360 ? angle - 360 : angle;
        }

        private decimal GetBackBearing(PegRegister from, PegRegister to)
        {
            decimal dy = to.YCoord - from.YCoord;
            decimal dx = to.XCoord - from.XCoord;
            double rad = Math.Atan2((double)dy, (double)dx);
            if (rad < 0) rad += 2 * Math.PI;
            return DegToDMS((decimal)(rad * 180 / Math.PI));
        }

        private decimal HD(PegRegister a, PegRegister b)
        {
            decimal dy = b.YCoord - a.YCoord;
            decimal dx = b.XCoord - a.XCoord;
            return (decimal)Math.Sqrt((double)(dy * dy + dx * dx));
        }

        public decimal DMStoDecimal(decimal dmsToConvert)
        {
            decimal deg = Math.Floor(dmsToConvert);

            decimal minW = dmsToConvert - deg;
            decimal min = Math.Floor(minW * 100);

            decimal secW = dmsToConvert * 100;
            decimal secW1 = Math.Floor(secW);
            decimal sec = (secW - secW1) * 100;

            return deg + (min / 60) + (sec / 3600);
        }
        public decimal DecToDMS(decimal decToConvert)
        {
            decimal deg = Math.Floor(decToConvert);
            decimal minW = decToConvert - deg;
            decimal min = Math.Floor(minW * 60);
            decimal secW = minW - (min / 60);
            decimal sec = secW * 3600;
            return deg + (min / 100) + (sec / 10000);
        }
        #endregion
    }
}