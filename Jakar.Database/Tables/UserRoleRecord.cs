// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:40 PM

namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record UserRoleRecord : Mapping<UserRoleRecord, UserRecord, RoleRecord>, ICreateMapping<UserRoleRecord, UserRecord, RoleRecord>, IDbReaderMapping<UserRoleRecord>
{
    public const  string TABLE_NAME = "user_roles";
    public static string TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }


    public UserRoleRecord( UserRecord            key, RoleRecord           value ) : base( key, value ) { }
    public UserRoleRecord( RecordID<UserRecord>  key, RecordID<RoleRecord> value ) : base( key, value ) { }
    private UserRoleRecord( RecordID<UserRecord> key, RecordID<RoleRecord> value, RecordID<UserRoleRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    [Pure] public static UserRoleRecord Create( UserRecord           key, RoleRecord           value ) => new(key, value);
    public static        UserRoleRecord Create( RecordID<UserRecord> key, RecordID<RoleRecord> value ) => new(key, value);
    [Pure]
    public static UserRoleRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>     key          = new( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        RecordID<RoleRecord>     value        = new( reader.GetFieldValue<Guid>( nameof(ValueID) ) );
        DateTimeOffset           dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?          lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserRoleRecord> id           = new( reader.GetFieldValue<Guid>( nameof(ID) ) );
        UserRoleRecord           record       = new( key, value, id, dateCreated, lastModified );
        return record.Validate();
    }
    [Pure]
    public static async IAsyncEnumerable<UserRoleRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
