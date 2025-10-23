// Jakar.Extensions :: Jakar.Database
// 10/22/2025  23:12

namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
public sealed record UserRecoveryCodeRecord : Mapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, ICreateMapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>
{
    public const  string                                 TABLE_NAME = "UserRecoveryCodes";
    public static string                                 TableName     => TABLE_NAME;
    public static JsonSerializerContext                  JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<UserRecoveryCodeRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.UserRecoveryCodeRecord;
    public static JsonTypeInfo<UserRecoveryCodeRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.UserRecoveryCodeRecordArray;

     
    public UserRecoveryCodeRecord( RecordID<UserRecord>               key, RecordID<RecoveryCodeRecord> value ) : base(key, value) { }
    public UserRecoveryCodeRecord( RecordID<UserRecord>               key, RecordID<RecoveryCodeRecord> value, RecordID<UserRecoveryCodeRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified = null ) : base(key, value, id, dateCreated, lastModified) { }
  
    
    public static UserRecoveryCodeRecord Create( UserRecord           key, RecoveryCodeRecord           value ) => new(key, value);
    public static UserRecoveryCodeRecord Create( RecordID<UserRecord> key, RecordID<RecoveryCodeRecord> value ) => new(key, value);
    [Pure] public static UserRecoveryCodeRecord[] Create( UserRecord key, params ReadOnlySpan<RecoveryCodeRecord> values )
    {
        UserRecoveryCodeRecord[] records = new UserRecoveryCodeRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static UserRecoveryCodeRecord[] Create( RecordID<UserRecord> key, params ReadOnlySpan<RecordID<RecoveryCodeRecord>> values )
    {
        UserRecoveryCodeRecord[] records = new UserRecoveryCodeRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static IEnumerable<UserRecoveryCodeRecord> Create( UserRecord key, IEnumerable<RecoveryCodeRecord> values )
    {
        foreach ( RecordID<RecoveryCodeRecord> value in values ) { yield return Create(key, value); }
    }
    [Pure] public static IEnumerable<UserRecoveryCodeRecord> Create( RecordID<UserRecord> key, IEnumerable<RecordID<RecoveryCodeRecord>> values )
    {
        foreach ( RecordID<RecoveryCodeRecord> value in values ) { yield return Create(key, value); }
    }


    public static UserRecoveryCodeRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>             key          = new(reader.GetFieldValue<Guid>(nameof(KeyID)));
        RecordID<RecoveryCodeRecord>     value        = new(reader.GetFieldValue<Guid>(nameof(KeyID)));
        DateTimeOffset                   dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?                  lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecoveryCodeRecord> id           = new(reader.GetFieldValue<Guid>(nameof(ID)));
        return new UserRecoveryCodeRecord(key, value, id, dateCreated, lastModified);
    }
    public static async IAsyncEnumerable<UserRecoveryCodeRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync(token) ) { yield return Create(reader); }
    }


    public static bool operator >( UserRecoveryCodeRecord  left, UserRecoveryCodeRecord right ) => Comparer<UserRecoveryCodeRecord>.Default.Compare(left, right) > 0;
    public static bool operator >=( UserRecoveryCodeRecord left, UserRecoveryCodeRecord right ) => Comparer<UserRecoveryCodeRecord>.Default.Compare(left, right) >= 0;
    public static bool operator <( UserRecoveryCodeRecord  left, UserRecoveryCodeRecord right ) => Comparer<UserRecoveryCodeRecord>.Default.Compare(left, right) < 0;
    public static bool operator <=( UserRecoveryCodeRecord left, UserRecoveryCodeRecord right ) => Comparer<UserRecoveryCodeRecord>.Default.Compare(left, right) <= 0;
}
