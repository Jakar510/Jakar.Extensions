// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:39 PM

namespace Jakar.Database;


[ Serializable, Table( "UserGroups" ) ]
public sealed partial record UserGroupRecord : Mapping<UserGroupRecord, UserRecord, GroupRecord>, ICreateMapping<UserGroupRecord, UserRecord, GroupRecord>, IDbReaderMapping<UserGroupRecord>
{
    public UserGroupRecord( UserRecord                                           owner, GroupRecord value ) : base( owner, value ) { }
    [ RequiresPreviewFeatures ] public static UserGroupRecord Create( UserRecord owner, GroupRecord value ) => new(owner, value);

    [ DbReaderMapping ] public static partial UserGroupRecord Create( DbDataReader reader );

    /*
    public static UserGroupRecord Create( DbDataReader reader )
    {
        DateTimeOffset            dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset            lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                      ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>      createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<UserGroupRecord> id           = new RecordID<UserGroupRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserGroupRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserGroupRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
    */

}
