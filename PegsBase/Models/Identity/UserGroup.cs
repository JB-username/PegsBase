namespace PegsBase.Models.Identity
{
    public class UserGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
    }
}
