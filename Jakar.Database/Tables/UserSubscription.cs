// Jakar.Extensions :: Jakar.Database
// 09/10/2023  10:12 PM

using Jakar.Database.Resx;



namespace Jakar.Database;


public interface IUserSubscription : IUniqueID<Guid>
{
    public DateTimeOffset? SubscriptionExpires { get; }
}



public abstract record UserSubscription<TSelf>( DateTimeOffset? SubscriptionExpires, RecordID<TSelf> ID, RecordID<UserRecord>? CreatedBy, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<TSelf>(in CreatedBy, in ID, in DateCreated, in LastModified), IUserSubscription
    where TSelf : UserSubscription<TSelf>, ITableRecord<TSelf>
{
    [Pure] public override PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(SubscriptionExpires), SubscriptionExpires);
        return parameters;
    }
}
