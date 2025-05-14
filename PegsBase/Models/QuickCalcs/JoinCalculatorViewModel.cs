using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.QuickCalcs
{
    public class JoinCalculatorViewModel
    {
        [Display(Name = "Start X")]
        public decimal? StartX { get; set; }
        [Display(Name = "Start Y")]
        public decimal? StartY { get; set; }
        [Display(Name = "Start Z")]
        public decimal? StartZ { get; set; }

        [Display(Name = "End X")]
        public decimal? EndX { get; set; }
        [Display(Name = "End Y")]
        public decimal? EndY { get; set; }
        [Display(Name = "End Z")]
        public decimal? EndZ { get; set; }


        [Display(Name = "Start Peg")]
        public int FirstPegId { get; set; }
        [Display(Name = "End Peg")]
        public int SecondPegId { get; set; }


        public List<SelectListItem> PegOptions { get; set; }

        public JoinCalculatorResult Result { get; set; }


        public string FormatDMS(decimal dms)
        {
            int deg = (int)Math.Floor(dms);

            decimal totalMinutes = (dms - deg) * 60m;
            int min = (int)Math.Floor(totalMinutes);

            decimal totalSeconds = (totalMinutes - min) * 60m;
            int sec = (int)Math.Round(totalSeconds, MidpointRounding.AwayFromZero);

            if (sec == 60)
            {
                sec = 0;
                min++;
            }
            if (min == 60)
            {
                min = 0;
                deg++;
            }

            return $"{deg:D3}:{min:D2}:{sec:D2}";
        }


        public string FormatDMSWithSymbols(decimal dms)
        {
            int deg = (int)Math.Floor(dms);

            decimal totalMinutes = (dms - deg) * 60m;
            int min = (int)Math.Floor(totalMinutes);

            decimal totalSeconds = (totalMinutes - min) * 60m;
            int sec = (int)Math.Round(totalSeconds, MidpointRounding.AwayFromZero);

            if (sec == 60)
            {
                sec = 0;
                min++;
            }
            if (min == 60)
            {
                min = 0;
                deg++;
            }

            return $"{deg}°{min:D2}′{sec:D2}″";
        }

    }
}
