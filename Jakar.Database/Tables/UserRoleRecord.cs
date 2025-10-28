// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:40 PM

namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
public sealed record UserRoleRecord : Mapping<UserRoleRecord, UserRecord, RoleRecord>, ICreateMapping<UserRoleRecord, UserRecord, RoleRecord>
{
    public const  string                         TABLE_NAME = "user_roles";
    public static JsonTypeInfo<UserRoleRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.UserRoleRecordArray;
    public static JsonSerializerContext          JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<UserRoleRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.UserRoleRecord;
    public static string                         TableName     => TABLE_NAME;


    public UserRoleRecord( RecordID<UserRecord>  key, RecordID<RoleRecord> value ) : base(key, value) { }
    private UserRoleRecord( RecordID<UserRecord> key, RecordID<RoleRecord> value, RecordID<UserRoleRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base(key, value, id, dateCreated, lastModified) { }


    [Pure] public static UserRoleRecord Create( UserRecord           key, RoleRecord           value ) => new(key, value);
    [Pure] public static UserRoleRecord Create( RecordID<UserRecord> key, RecordID<RoleRecord> value ) => new(key, value);
    [Pure] public static UserRoleRecord[] Create( UserRecord key, params ReadOnlySpan<RoleRecord> values )
    {
        UserRoleRecord[] records = new UserRoleRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static UserRoleRecord[] Create( RecordID<UserRecord> key, params ReadOnlySpan<RecordID<RoleRecord>> values )
    {
        UserRoleRecord[] records = new UserRoleRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static IEnumerable<UserRoleRecord> Create( UserRecord key, IEnumerable<RoleRecord> values )
    {
        foreach ( RecordID<RoleRecord> value in values ) { yield return Create(key, value); }
    }
    [Pure] public static IEnumerable<UserRoleRecord> Create( RecordID<UserRecord> key, IEnumerable<RecordID<RoleRecord>> values )
    {
        foreach ( RecordID<RoleRecord> value in values ) { yield return Create(key, value); }
    }
    [Pure] public static UserRoleRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>     key          = new(reader.GetFieldValue<Guid>(nameof(KeyID)));
        RecordID<RoleRecord>     value        = new(reader.GetFieldValue<Guid>(nameof(ValueID)));
        DateTimeOffset           dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?          lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRoleRecord> id           = new(reader.GetFieldValue<Guid>(nameof(ID)));
        UserRoleRecord           record       = new(key, value, id, dateCreated, lastModified);
        return record.Validate();
    }


    public static bool operator >( UserRoleRecord  left, UserRoleRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( UserRoleRecord left, UserRoleRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( UserRoleRecord  left, UserRoleRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( UserRoleRecord left, UserRoleRecord right ) => left.CompareTo(right) <= 0;
}
