namespace PegsBase.Models.Entities
{
    public class Locality
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int LevelId { get; set; }
        public Level Level { get; set; }

        public int? SubSectionId { get; set; } // Nullable for flexibility
        public SubSection? SubSection { get; set; }

        public ICollection<SurveyNote> SurveyNotes { get; set; } = new List<SurveyNote>();
        public ICollection<PegRegister> Pegs { get; set; } = new List<PegRegister>();
    }
}
