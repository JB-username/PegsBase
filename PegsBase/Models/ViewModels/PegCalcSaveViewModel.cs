using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace PegsBase.Models.ViewModels
{
    public class PegCalcSaveViewModel
    {
        [Required]
        public required string Surveyor { get; set; }
        [Required]
        public required string Locality { get; set; }

        public int Level { get; set; }
        [Required]
        public SurveyPointType PointType { get; set; }
        [Required]
        public decimal GradeElevation { get; set; }
    }
}