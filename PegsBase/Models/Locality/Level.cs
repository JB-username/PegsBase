using PegsBase.Models.Entities;
using PegsBase.Models.MinePlans;

namespace PegsBase.Models.Entities
{
    public class Level
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SubSectionId { get; set; }
        public SubSection? SubSection { get; set; }
        public ICollection<Locality> Localities { get; set; }
        public ICollection<MinePlan> MinePlans { get; set; }
    }
}
