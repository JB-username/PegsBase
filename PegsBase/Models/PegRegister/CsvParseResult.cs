namespace PegsBase.Models
{
    public class CsvParseResult
    {
        public int RowNumber { get; set; }
        public PegRegisterImportModel Peg { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
