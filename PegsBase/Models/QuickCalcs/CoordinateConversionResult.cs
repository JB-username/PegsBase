namespace PegsBase.Models.QuickCalcs
{
    public class CoordinateConversionResult
    {
        public int PegId { get; set; }
        public string PegName { get; set; }

        // Original coordinates
        public double OrigX { get; set; }
        public double OrigY { get; set; }
        public double? OrigZ { get; set; }

        // Converted coordinates
        public double X { get; set; }
        public double Y { get; set; }
        public double? Z { get; set; }
    }
}
