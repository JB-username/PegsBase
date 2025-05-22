using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.Subscriptions
{
    public class Subscription
    {
        [Key]
        public Guid CompanyId { get; set; }   // matches your tenant/company GUID
        public int BaseSeats { get; set; }   // the “base” seats
        public int ExtraSeats { get; set; }   // seats they’ve bought
        public DateTime ExpiresOn { get; set; }   // end of subscription period
        public int GraceDays { get; set; }   // grace period after expiry
        public bool IsActive { get; set; }   // on/off switch
    }
}
