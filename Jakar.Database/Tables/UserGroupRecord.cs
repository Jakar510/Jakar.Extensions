// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:39 PM

namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
public sealed record UserGroupRecord : Mapping<UserGroupRecord, UserRecord, GroupRecord>, ICreateMapping<UserGroupRecord, UserRecord, GroupRecord>
{
    public const  string                          TABLE_NAME = "user_groups";
    public static JsonTypeInfo<UserGroupRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.UserGroupRecordArray;
    public static JsonSerializerContext           JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<UserGroupRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.UserGroupRecord;
    public static string                          TableName     => TABLE_NAME;


    public UserGroupRecord( RecordID<UserRecord>  key, RecordID<GroupRecord> value ) : base(key, value) { }
    private UserGroupRecord( RecordID<UserRecord> key, RecordID<GroupRecord> value, RecordID<UserGroupRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base(key, value, id, dateCreated, lastModified) { }


    [Pure] public static UserGroupRecord Create( UserRecord           key, GroupRecord           value ) => new(key, value);
    public static        UserGroupRecord Create( RecordID<UserRecord> key, RecordID<GroupRecord> value ) => new(key, value);
    [Pure] public static UserGroupRecord[] Create( RecordID<UserRecord> key, params ReadOnlySpan<RecordID<GroupRecord>> values )
    {
        UserGroupRecord[] records = new UserGroupRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static UserGroupRecord[] Create( UserRecord key, params ReadOnlySpan<GroupRecord> values )
    {
        UserGroupRecord[] records = new UserGroupRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static IEnumerable<UserGroupRecord> Create( RecordID<UserRecord> key, IEnumerable<RecordID<GroupRecord>> values )
    {
        foreach ( RecordID<GroupRecord> value in values ) { yield return Create(key, value); }
    }
    [Pure] public static IEnumerable<UserGroupRecord> Create( UserRecord key, IEnumerable<GroupRecord> values )
    {
        foreach ( RecordID<GroupRecord> value in values ) { yield return Create(key, value); }
    }
    [Pure] public static UserGroupRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>      key          = new(reader.GetFieldValue<Guid>(nameof(KeyID)));
        RecordID<GroupRecord>     value        = new(reader.GetFieldValue<Guid>(nameof(KeyID)));
        DateTimeOffset            dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?           lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserGroupRecord> id           = new(reader.GetFieldValue<Guid>(nameof(ID)));
        UserGroupRecord           record       = new(key, value, id, dateCreated, lastModified);
        return record.Validate();
    }


    public static bool operator >( UserGroupRecord  left, UserGroupRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( UserGroupRecord left, UserGroupRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( UserGroupRecord  left, UserGroupRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( UserGroupRecord left, UserGroupRecord right ) => left.CompareTo(right) <= 0;
}
