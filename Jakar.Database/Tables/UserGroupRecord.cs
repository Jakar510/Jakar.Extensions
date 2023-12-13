// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:39 PM

namespace Jakar.Database;


[ Serializable, Table( "UserGroups" ) ]
public sealed record UserGroupRecord : Mapping<UserGroupRecord, UserRecord, GroupRecord>, ICreateMapping<UserGroupRecord, UserRecord, GroupRecord>, IDbReaderMapping<UserGroupRecord>
{
    public static string TableName { get; } = typeof(UserGroupRecord).GetTableName();


    public UserGroupRecord( UserRecord            owner, GroupRecord           value ) : base( owner, value ) { }
    private UserGroupRecord( RecordID<UserRecord> key,   RecordID<GroupRecord> value, RecordID<UserGroupRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    [ Pure ] public static UserGroupRecord Create( UserRecord owner, GroupRecord value ) => new(owner, value);
    [ Pure ]
    public static UserGroupRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var value        = new RecordID<GroupRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<UserGroupRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        var record       = new UserGroupRecord( key, value, id, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [ Pure ]
    public static async IAsyncEnumerable<UserGroupRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
