namespace PegsBase.Models
{
    public class TestCoordinateUploadViewModel
    {
        public List<TestCoordinateRow> PreviewRows { get; set; } = new();
    }

    public class TestCoordinateRow
    {
        public string PegName { get; set; }
        public bool SaveToDatabase { get; set; }
    }
}
