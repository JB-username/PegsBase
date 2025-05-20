namespace PegsBase.Models.QuickCalcs
{
    public class CoordinateConversionPegLookupViewModel
    {
        public List<PegRegister> Pegs { get; set; }
        public List<int> SelectedPegIds { get; set; } = new();
    }
}
