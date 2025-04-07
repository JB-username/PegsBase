namespace PegsBase.Models.Identity
{
    public class AdminUserRow
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyId { get; set; }
        public string? JobTitle { get; set; }
        public string? Department {  get; set; }
        public string? Section { get; set; }
        
    }
}
