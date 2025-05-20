namespace PegsBase.Models
{
    public class CsvPreviewModel
    {
        public int TotalRows { get; set; }
        public int TotalGood {  get; set; }
        public List<CsvParseResult> GoodRows { get; set; } = [];
        public List<CsvParseResult> BadRows { get; set; } = [];
    }
}
