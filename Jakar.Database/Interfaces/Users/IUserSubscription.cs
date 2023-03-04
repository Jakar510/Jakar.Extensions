namespace Jakar.Database;


public interface IUserSubscription
{
    public DateTimeOffset? SubscriptionExpires { get; }
    public Guid?           SubscriptionID      { get; }
}
