// Jakar.Extensions :: Jakar.Database
// 09/10/2023  10:12 PM

namespace Jakar.Database;


public interface IUserSubscription : IUniqueID<Guid>
{
    public DateTimeOffset? SubscriptionExpires { get; }
}



public abstract record UserSubscription<TRecord>
    ( DateTimeOffset? SubscriptionExpires, RecordID<TRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default ) : OwnedTableRecord<TRecord>( ID,
                                                                                                                                                                                                                       CreatedBy,
                                                                                                                                                                                                                       OwnerUserID,
                                                                                                                                                                                                                       DateCreated,
                                                                                                                                                                                                                       LastModified ),
                                                                                                                                                                                            IUserSubscription
    where TRecord : UserSubscription<TRecord>, IDbReaderMapping<TRecord>
{
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(SubscriptionExpires), SubscriptionExpires );
        return parameters;
    }
}
