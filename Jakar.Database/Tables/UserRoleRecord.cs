// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:40 PM

namespace Jakar.Database;


[ Serializable, Table( "UserRoles" ) ]
public sealed record UserRoleRecord : Mapping<UserRoleRecord, UserRecord, RoleRecord>, ICreateMapping<UserRoleRecord, UserRecord, RoleRecord>, IDbReaderMapping<UserRoleRecord>, MsJsonModels.IJsonizer<UserRoleRecord>
{
    public static string TableName { get; } = typeof(UserRoleRecord).GetTableName();


    public UserRoleRecord( UserRecord            owner, RoleRecord           value ) : base( owner, value ) { }
    private UserRoleRecord( RecordID<UserRecord> key,   RecordID<RoleRecord> value, RecordID<UserRoleRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    [ Pure ] public static UserRoleRecord Create( UserRecord owner, RoleRecord value ) => new(owner, value);
    [ Pure ]
    public static UserRoleRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var value        = new RecordID<RoleRecord>( reader.GetFieldValue<Guid>( nameof(ValueID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<UserRoleRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        var record       = new UserRoleRecord( key, value, id, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [ Pure ]
    public static async IAsyncEnumerable<UserRoleRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    [ Pure ] public static UserRoleRecord FromJson( string json ) => json.FromJson( JsonTypeInfo() );
    [ Pure ]
    public static JsonSerializerOptions JsonOptions( bool formatted ) => new()
                                                                         {
                                                                             WriteIndented    = formatted,
                                                                             TypeInfoResolver = UserRoleRecordContext.Default
                                                                         };
    [ Pure ] public static JsonTypeInfo<UserRoleRecord> JsonTypeInfo() => UserRoleRecordContext.Default.UserRoleRecord;
}



[ JsonSerializable( typeof(UserRoleRecord) ) ] public partial class UserRoleRecordContext : JsonSerializerContext { }
