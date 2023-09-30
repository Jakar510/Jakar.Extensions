// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:40 PM

namespace Jakar.Database;


[ Serializable, Table( "UserRoles" ) ]
public sealed record UserRoleRecord : Mapping<UserRoleRecord, UserRecord, RoleRecord>, ICreateMapping<UserRoleRecord, UserRecord, RoleRecord>, IDbReaderMapping<UserRoleRecord>
{
    public static string TableName { get; } = typeof(UserRoleRecord).GetTableName();


    public UserRoleRecord( UserRecord            owner, RoleRecord           value ) : base( owner, value ) { }
    private UserRoleRecord( RecordID<UserRecord> key,   RecordID<RoleRecord> value, RecordID<UserRoleRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    public static UserRoleRecord Create( UserRecord owner, RoleRecord value ) => new(owner, value);
    public static UserRoleRecord Create( DbDataReader reader )
    {
        var             key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var             value        = new RecordID<RoleRecord>( reader.GetFieldValue<Guid>( nameof(ValueID) ) );
        DateTimeOffset  dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset? lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var             id           = new RecordID<UserRoleRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserRoleRecord( key, value, id, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserRoleRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
