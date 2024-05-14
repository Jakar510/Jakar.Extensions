// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:39 PM

namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record UserGroupRecord : Mapping<UserGroupRecord, UserRecord, GroupRecord>, ICreateMapping<UserGroupRecord, UserRecord, GroupRecord>, IDbReaderMapping<UserGroupRecord>
{
    public const  string TABLE_NAME = "UserGroups";
    public static string TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }


    public UserGroupRecord( UserRecord            owner, GroupRecord           value ) : base( owner, value ) { }
    private UserGroupRecord( RecordID<UserRecord> key,   RecordID<GroupRecord> value, RecordID<UserGroupRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    [Pure] public static UserGroupRecord Create( UserRecord owner, GroupRecord value ) => new(owner, value);
    [Pure]
    public static UserGroupRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>      key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        RecordID<GroupRecord>     value        = new RecordID<GroupRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        DateTimeOffset            dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?           lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserGroupRecord> id           = new RecordID<UserGroupRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        UserGroupRecord                       record       = new UserGroupRecord( key, value, id, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<UserGroupRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
