using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.MinePlans
{
    public class MinePlan
    {
        public int Id { get; set; }
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; }
        public string Level { get; set; }
        public string Locality { get; set; }
        public int Scale { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UploadedAt { get; set; }
        public bool IsSigned { get; set; } // true = signed, false = in progress

        [Display(Name = "Is Superseded")]
        public bool IsSuperseded { get; set; }

        [Display(Name = "Superseded Reference")]
        public string? SupersededReference { get; set; }

        [Display(Name = "Plan Type")]
        public MinePlanType PlanType { get; set; }
        public string? FilePath { get; set; } // relative path to the stored PDF
        public string? ThumbnailPath { get; set; } // e.g. ~/uploads/thumbnails/xxx.jpg

    }
}

