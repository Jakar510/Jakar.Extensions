namespace Jakar.Database;


public interface IUserSubscription<TID> where TID : struct, IComparable<TID>, IEquatable<TID>
{
    public DateTimeOffset? SubscriptionExpires { get; }
    public TID?             SubscriptionID      { get; }
}
