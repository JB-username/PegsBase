using Microsoft.AspNetCore.Mvc.Rendering;
using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.SurveyNotes
{
    public class SurveyNoteEditViewModel
    {
        public int Id { get; set; }

        [Required] public string Title { get; set; }

        [Required] public SurveyNoteType NoteType { get; set; }

        public bool IsSigned { get; set; }
        public bool IsAbandoned { get; set; }
        public string? AbandonmentReason { get; set; }

        [Required] public int LevelId { get; set; }
        [Required] public int LocalityId { get; set; }
        [Required] public string? SurveyorId { get; set; }

        public DateTime UploadedAt { get; set; }

        public IEnumerable<SelectListItem>? LevelOptions { get; set; }
        public IEnumerable<SelectListItem>? LocalityOptions { get; set; }
        public IEnumerable<SelectListItem>? SurveyorOptions { get; set; }
    }
}
