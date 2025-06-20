// Jakar.Extensions :: Jakar.Database
// 09/10/2023  10:12 PM

namespace Jakar.Database;


public interface IUserSubscription : IUniqueID<Guid>
{
    public DateTimeOffset? SubscriptionExpires { get; }
}



public abstract record UserSubscription<TClass>( DateTimeOffset? SubscriptionExpires, RecordID<TClass> ID, RecordID<UserRecord>? CreatedBy, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<TClass>( in CreatedBy, in ID, in DateCreated, in LastModified ), IUserSubscription
    where TClass : UserSubscription<TClass>, IDbReaderMapping<TClass>
{
    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(SubscriptionExpires), SubscriptionExpires );
        return parameters;
    }
}
