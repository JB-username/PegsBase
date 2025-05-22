using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PegsBase.Models.Entities;
using PegsBase.Models.Enums;
using PegsBase.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.SurveyNotes
{
    public class SurveyNoteUploadViewModel
    {
        [Display(Name = "Note Number")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Level")]
        [Required(ErrorMessage = "Please select a level")]
        public int LevelId { get; set; }
        public Level? Level { get; set; }

        [Display(Name = "Locality")]
        [Required(ErrorMessage = "Please select a locality")]
        public int LocalityId { get; set; }
        public Locality? Locality { get; set; }
        
        [Display(Name = "Signed Off")]
        public bool IsSigned { get; set; }

        [Required]
        public IFormFile? File { get; set; }

        [Display(Name = "Surveyor")]
        [Required(ErrorMessage = "Please select the surveyor")]
        public string? SurveyorId { get; set; }

        [Required]
        [Display(Name = "Note Type")]
        public SurveyNoteType NoteType { get; set; }

        // Optional: for populating dropdowns in the view
        public IEnumerable<SelectListItem>? Levels { get; set; }
        public IEnumerable<SelectListItem>? Localities { get; set; }
        public IEnumerable<SelectListItem>? Surveyors { get; set; }
    }
}
