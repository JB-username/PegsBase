namespace PegsBase.Models.Identity
{
    public class AdminViewModel
    {
        public List<WhitelistedEmails> WhitelistedEmails { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
