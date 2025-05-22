using PegsBase.Models.Enums;

namespace PegsBase.Models.SurveyNotes
{
    public class SurveyNotesListViewModel
    {
        public List<SurveyNoteModel> Notes { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public SurveyNoteType? FilterType { get; set; }
        public string? FilterSearch { get; set; }
        public string? FilterLevel { get; set; }
        public bool? FilterIsSigned { get; set; } // true = signed, false = in progress

    }
}
