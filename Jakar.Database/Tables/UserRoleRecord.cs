// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:40 PM

namespace Jakar.Database;


[ Serializable, Table( "UserRoles" ) ]
public sealed record UserRoleRecord : Mapping<UserRoleRecord, UserRecord, RoleRecord>, ICreateMapping<UserRoleRecord, UserRecord, RoleRecord>, IDbReaderMapping<UserRoleRecord>
{
    public UserRoleRecord( UserRecord                                           owner, RoleRecord value ) : base( owner, value ) { }
    [ RequiresPreviewFeatures ] public static UserRoleRecord Create( UserRecord owner, RoleRecord value ) => new(owner, value);


    public static UserRoleRecord Create( DbDataReader reader )
    {
        DateTimeOffset           dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset           lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                     ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>     createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<UserRoleRecord> id           = new RecordID<UserRoleRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserRoleRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserRoleRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
