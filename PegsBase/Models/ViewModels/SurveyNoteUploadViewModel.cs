using PegsBase.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.ViewModels
{
    public class SurveyNoteUploadViewModel
    {
        [Display(Name = "Note Number")]
        public string Title { get; set; }
        public string Level { get; set; }
        public string Surveyor { get; set; }
        public string Locality { get; set; }

        [Display(Name = "Fully Signed")]
        public bool IsSigned { get; set; }

        [Required]
        public IFormFile File { get; set; }

        [Required]
        [Display(Name = "Note Type")]
        public SurveyNoteType NoteType { get; set; }

    }
}
