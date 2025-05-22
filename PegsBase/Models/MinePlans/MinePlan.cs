using PegsBase.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.MinePlans
{
    public class MinePlan
    {
        public int Id { get; set; }
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; }

        public int LevelId { get; set; }
        public Level Level { get; set; }
        public int? LocalityId { get; set; }
        public Locality? Locality { get; set; }
        public int Scale { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UploadedAt { get; set; }
        public bool IsSigned { get; set; } // true = signed, false = in progress
        [Display(Name = "Is Superseded")]
        public bool IsSuperseded { get; set; }
        [Display(Name = "Superseded Reference")]
        public string? SupersededReference { get; set; }
        public int PlanTypeId { get; set; }
        [Display(Name = "Plan Type")]
        public PlanType? PlanType { get; set; }
        public string? FilePath { get; set; } // relative path to the stored PDF
        public string? ThumbnailPath { get; set; } // e.g. ~/uploads/thumbnails/xxx.jpg

    }
}

