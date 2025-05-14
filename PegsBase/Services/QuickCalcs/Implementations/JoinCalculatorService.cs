using PegsBase.Models.QuickCalcs;
using PegsBase.Services.QuickCalcs.Interfaces;

namespace PegsBase.Services.QuickCalcs.Implementations
{
    public class JoinCalculatorService : IJoinCalculatorService

    {
        public JoinCalculatorResult Calculate(
            decimal x1, 
            decimal y1, 
            decimal z1, 
            decimal x2, 
            decimal y2, 
            decimal z2
            )
        {
            var dx = (double)(x2 - x1);
            var dy = (double)(y2 - y1);
            var dz = (double)(z2 - z1);

            var horiz = Math.Sqrt(dx * dx + dy * dy);

            var slope = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            var angleRad = Math.Atan2(dx, dy);
            var bearing = (angleRad * 180 / Math.PI + 360) % 360;

            var dip = Math.Atan2(dz, horiz) * 180 / Math.PI;

            var vd = (double)(z1 - z2);

            return new JoinCalculatorResult
            {
                HorizontalDistance = horiz,
                SlopeDistance = slope,
                BearingDegrees = bearing,
                DipDegrees = dip,
                VerticalDifference = vd
            };
        }
    }
}
