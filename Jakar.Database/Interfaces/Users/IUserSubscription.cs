namespace Jakar.Database;


public interface IUserSubscription
{
    public DateTimeOffset? SubscriptionExpires { get; }
    public long?           SubscriptionID      { get; }
}
