namespace PegsBase.Models.Entities
{
    public class SubSection
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., CC1E
        public string? Description { get; set; }

        public int MainSectionId { get; set; }
        public MainSection MainSection { get; set; }

        public ICollection<Level> Levels { get; set; } = new List<Level>();
        public ICollection<Locality> Localities { get; set; } = new List<Locality>();
    }
}

