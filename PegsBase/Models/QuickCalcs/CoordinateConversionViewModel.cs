using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.QuickCalcs
{
    public class CoordinateConversionViewModel
    {
        public List<PegRegister> AvailablePegs { get; set; } = new();
        
        [Display(Name = "Select Peg(s)")]
        public List<int> SelectedPegIds { get; set; } = new();

        
        [Display(Name = "Input X")]
        public double? InputX { get; set; }

        [Display(Name = "Input Y")]
        public double? InputY { get; set; }

        
        [Display(Name = "Source SRID")]
        public int SourceSrid { get; set; }

        [Display(Name = "Target SRID")]
        public int TargetSrid { get; set; }

        public List<SelectListItem> SridOptions { get; set; }


        public List<CoordinateConversionResult> Results { get; set; }
            = new List<CoordinateConversionResult>();

    }
}
