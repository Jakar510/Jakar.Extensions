namespace Jakar.Database;


public interface IUserSubscription<out TID> where TID : IComparable<TID>, IEquatable<TID>
{
    public DateTimeOffset? SubscriptionExpires { get; }
    public TID?            SubscriptionID      { get; }
}