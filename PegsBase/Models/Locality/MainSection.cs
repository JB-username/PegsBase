namespace PegsBase.Models.Entities
{
    public class MainSection
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., 3# Shaft
        public string? Description { get; set; }

        public ICollection<SubSection> SubSections { get; set; } = new List<SubSection>();
    }
}
