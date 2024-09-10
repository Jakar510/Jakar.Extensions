// Jakar.Extensions :: Jakar.Database
// 4/4/2024  10:22

namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record UserAddressRecord : Mapping<UserAddressRecord, UserRecord, AddressRecord>, ICreateMapping<UserAddressRecord, UserRecord, AddressRecord>, IDbReaderMapping<UserAddressRecord>
{
    public const  string TABLE_NAME = "UserAdreesses";
    public static string TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }


    public UserAddressRecord( UserRecord            key, AddressRecord           value ) : base( key, value ) { }
    public UserAddressRecord( RecordID<UserRecord>  key, RecordID<AddressRecord> value ) : base( key, value ) { }
    private UserAddressRecord( RecordID<UserRecord> key, RecordID<AddressRecord> value, RecordID<UserAddressRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    [Pure] public static UserAddressRecord Create( UserRecord           key, AddressRecord           value ) => new(key, value);
    public static        UserAddressRecord Create( RecordID<UserRecord> key, RecordID<AddressRecord> value ) => new(key, value);
    [Pure]
    public static UserAddressRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>        key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        RecordID<AddressRecord>     value        = new RecordID<AddressRecord>( reader.GetFieldValue<Guid>( nameof(ValueID) ) );
        DateTimeOffset              dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?             lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserAddressRecord> id           = new RecordID<UserAddressRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        UserAddressRecord           record       = new UserAddressRecord( key, value, id, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<UserAddressRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
