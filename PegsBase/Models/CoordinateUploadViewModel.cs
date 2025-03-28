using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models
{
    public class CoordinateUploadViewModel
    {
        [Display(Name = "Coordinate File (.dat)")]
        public IFormFile? CoordinateFile { get; set; }

        public List<PegPreviewModel> PreviewRows { get; set; } = new();
        public string? RedirectAfterSaveUrl { get; set; }
    }
}
