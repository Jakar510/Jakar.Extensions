// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:26 PM

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Database;


[ Serializable, Table( "Codes" ) ]
public sealed record RecoveryCodeRecord
    ( [ MaxLength( 10240 ) ] string Code, RecordID<RecoveryCodeRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default ) : OwnedTableRecord<RecoveryCodeRecord>( ID,
                                                                                                                                                                                                                                            CreatedBy,
                                                                                                                                                                                                                                            OwnerUserID,
                                                                                                                                                                                                                                            DateCreated,
                                                                                                                                                                                                                                            LastModified ),
                                                                                                                                                                                                      IDbReaderMapping<RecoveryCodeRecord>,
                                                                                                                                                                                                      IMsJsonContext<RecoveryCodeRecord>
{
    private static readonly PasswordHasher<RecoveryCodeRecord> _hasher = new();

    public static string TableName { get; } = typeof(RecoveryCodeRecord).GetTableName();


    public RecoveryCodeRecord( string code, UserRecord user ) : this( code, RecordID<RecoveryCodeRecord>.New(), user.ID, user.UserID, DateTimeOffset.UtcNow ) { }

    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(Code), Code );
        return parameters;
    }

    public static RecoveryCodeRecord Create( DbDataReader reader )
    {
        string                       code         = reader.GetFieldValue<string>( nameof(Code) );
        var                          dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                          lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var                          ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>?        createdBy    = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<RecoveryCodeRecord> id           = RecordID<RecoveryCodeRecord>.ID( reader );
        var                          record       = new RecoveryCodeRecord( code, id, createdBy, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    public static async IAsyncEnumerable<RecoveryCodeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static IReadOnlyDictionary<string, RecoveryCodeRecord> Create( UserRecord user, IEnumerable<string> recoveryCodes )
    {
        var codes = new Dictionary<string, RecoveryCodeRecord>( StringComparer.Ordinal );

        foreach ( string recoveryCode in recoveryCodes )
        {
            (string code, RecoveryCodeRecord record) = Create( user, recoveryCode );
            codes[code]                              = record;
        }

        return codes;
    }
    public static async ValueTask<IReadOnlyDictionary<string, RecoveryCodeRecord>> Create( UserRecord user, IAsyncEnumerable<string> recoveryCodes )
    {
        var codes = new Dictionary<string, RecoveryCodeRecord>( StringComparer.Ordinal );

        await foreach ( string recoveryCode in recoveryCodes )
        {
            (string code, RecoveryCodeRecord record) = Create( user, recoveryCode );
            codes[code]                              = record;
        }

        return codes;
    }
    public static IReadOnlyDictionary<string, RecoveryCodeRecord> Create( UserRecord user, int count )
    {
        var codes = new Dictionary<string, RecoveryCodeRecord>( count, StringComparer.Ordinal );

        for ( int i = 0; i < count; i++ )
        {
            (string code, RecoveryCodeRecord record) = Create( user );
            codes[code]                              = record;
        }

        return codes;
    }
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user )              => Create( user, Guid.NewGuid() );
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, Guid   code ) => Create( user, code.ToBase64() );
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, string code ) => (code, new RecoveryCodeRecord( code, user ));


    public static bool IsValid( string code, ref RecoveryCodeRecord record )
    {
        switch ( _hasher.VerifyHashedPassword( record, record.Code, code ) )
        {
            case PasswordVerificationResult.Failed:  return false;
            case PasswordVerificationResult.Success: return true;

            case PasswordVerificationResult.SuccessRehashNeeded:
                record = record with
                         {
                             Code = _hasher.HashPassword( record, code )
                         };

                return true;

            default: throw new ArgumentOutOfRangeException();
        }
    }
    public static JsonSerializerOptions JsonOptions( bool formatted ) => new()
                                                                         {
                                                                             WriteIndented    = formatted,
                                                                             TypeInfoResolver = RecoveryCodeRecordContext.Default
                                                                         };
    public static JsonTypeInfo<RecoveryCodeRecord> JsonTypeInfo() => RecoveryCodeRecordContext.Default.RecoveryCodeRecord;
}



[ JsonSerializable( typeof(RecoveryCodeRecord) ) ] public partial class RecoveryCodeRecordContext : JsonSerializerContext { }



[ Serializable, Table( "UserRecoveryCodes" ) ]
public sealed record UserRecoveryCodeRecord : Mapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>,
                                              ICreateMapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>,
                                              IDbReaderMapping<UserRecoveryCodeRecord>,
                                              IMsJsonContext<UserRecoveryCodeRecord>
{
    public static string TableName { get; } = typeof(UserRecoveryCodeRecord).GetTableName();

    public UserRecoveryCodeRecord( UserRecord owner, RecoveryCodeRecord value ) : base( owner, value ) { }
    public UserRecoveryCodeRecord( RecordID<UserRecord> key, RecordID<RecoveryCodeRecord> value, RecordID<UserRecoveryCodeRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified = default ) :
        base( key, value, id, dateCreated, lastModified ) { }
    public static UserRecoveryCodeRecord Create( UserRecord owner, RecoveryCodeRecord value ) => new(owner, value);


    public static UserRecoveryCodeRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var value        = new RecordID<RecoveryCodeRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<UserRecoveryCodeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserRecoveryCodeRecord( key, value, id, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserRecoveryCodeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
    public static JsonSerializerOptions JsonOptions( bool formatted ) => new()
                                                                         {
                                                                             WriteIndented    = formatted,
                                                                             TypeInfoResolver = UserRecoveryCodeRecordContext.Default
                                                                         };
    public static JsonTypeInfo<UserRecoveryCodeRecord> JsonTypeInfo() => UserRecoveryCodeRecordContext.Default.UserRecoveryCodeRecord;
}



[ JsonSerializable( typeof(UserRecoveryCodeRecord) ) ] public partial class UserRecoveryCodeRecordContext : JsonSerializerContext { }
