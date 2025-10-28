// Jakar.Extensions :: Jakar.Database
// 4/4/2024  10:22

namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
public sealed record UserAddressRecord : Mapping<UserAddressRecord, UserRecord, AddressRecord>, ICreateMapping<UserAddressRecord, UserRecord, AddressRecord>
{
    public const  string                            TABLE_NAME = "user_adreesses";
    public static JsonTypeInfo<UserAddressRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.UserAddressRecordArray;
    public static JsonSerializerContext             JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<UserAddressRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.UserAddressRecord;
    public static string                            TableName     => TABLE_NAME;


    public UserAddressRecord( UserRecord            key, AddressRecord           value ) : base(key, value) { }
    public UserAddressRecord( RecordID<UserRecord>  key, RecordID<AddressRecord> value ) : base(key, value) { }
    private UserAddressRecord( RecordID<UserRecord> key, RecordID<AddressRecord> value, RecordID<UserAddressRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base(key, value, id, dateCreated, lastModified) { }


    [Pure] public static UserAddressRecord Create( UserRecord           key, AddressRecord           value ) => new(key, value);
    [Pure] public static UserAddressRecord Create( RecordID<UserRecord> key, RecordID<AddressRecord> value ) => new(key, value);
    [Pure] public static UserAddressRecord[] Create( UserRecord key, params ReadOnlySpan<AddressRecord> values )
    {
        UserAddressRecord[] records = new UserAddressRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static UserAddressRecord[] Create( RecordID<UserRecord> key, params ReadOnlySpan<RecordID<AddressRecord>> values )
    {
        UserAddressRecord[] records = new UserAddressRecord[values.Length];
        for ( int i = 0; i < values.Length; i++ ) { records[i] = Create(key, values[i]); }

        return records;
    }
    [Pure] public static IEnumerable<UserAddressRecord> Create( UserRecord key, IEnumerable<AddressRecord> values )
    {
        foreach ( RecordID<AddressRecord> value in values ) { yield return Create(key, value); }
    }
    [Pure] public static IEnumerable<UserAddressRecord> Create( RecordID<UserRecord> key, IEnumerable<RecordID<AddressRecord>> values )
    {
        foreach ( RecordID<AddressRecord> value in values ) { yield return Create(key, value); }
    }
    [Pure] public static UserAddressRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>        key          = new(reader.GetFieldValue<Guid>(nameof(KeyID)));
        RecordID<AddressRecord>     value        = new(reader.GetFieldValue<Guid>(nameof(ValueID)));
        DateTimeOffset              dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?             lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserAddressRecord> id           = new(reader.GetFieldValue<Guid>(nameof(ID)));
        UserAddressRecord           record       = new(key, value, id, dateCreated, lastModified);
        return record.Validate();
    }


    public static bool operator >( UserAddressRecord  left, UserAddressRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( UserAddressRecord left, UserAddressRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( UserAddressRecord  left, UserAddressRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( UserAddressRecord left, UserAddressRecord right ) => left.CompareTo(right) <= 0;
}
