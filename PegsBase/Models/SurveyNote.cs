﻿

using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models
{
    public class SurveyNote
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string Locality { get; set; }
        public string UploadedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UploadedAt { get; set; }
        public bool IsVerified { get; set; }// false = no, true =yes
        public bool IsSigned { get; set; } // true = signed, false = in progress
        public bool IsAbandoned { get; set; }
        public string? AbandonmentReason { get; set; }
        public SurveyNoteType NoteType { get; set; }
        public string? FilePath { get; set; } // relative path to the stored PDF
        public bool RequiresManualSignature => NoteType == SurveyNoteType.HolingNote;
        public string? ThumbnailPath { get; set; } // e.g. ~/uploads/thumbnails/xxx.jpg

    }
}
