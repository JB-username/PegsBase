namespace PegsBase.Models.Identity
{
    public class Invite
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AssignedRole { get; set; }
    }
}
