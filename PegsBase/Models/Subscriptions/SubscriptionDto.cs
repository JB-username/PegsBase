namespace PegsBase.Models.Subscriptions
{
    public record SubscriptionDto(
        Guid CompanyId,
        int BaseSeats,
        int ExtraSeats,
        DateTime ExpiresOn,
        int GraceDays,
        bool IsActive)
    {
        public SubscriptionDto() : this(Guid.Empty, 0, 0, DateTime.MinValue, 0, false) { }

        public SubscriptionDto(Subscription s)
          : this(s.CompanyId, s.BaseSeats, s.ExtraSeats, s.ExpiresOn, s.GraceDays, s.IsActive) { }
    }
}
