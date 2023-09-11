// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:26 PM

namespace Jakar.Database;


[ Serializable, Table( "Codes" ) ]
public sealed record RecoveryCodeRecord
    ( [ MaxLength( 1024 ) ] string Code, RecordID<RecoveryCodeRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default ) : TableRecord<RecoveryCodeRecord>( ID,
                                                                                                                                                                                                                                      CreatedBy,
                                                                                                                                                                                                                                      OwnerUserID,
                                                                                                                                                                                                                                      DateCreated,
                                                                                                                                                                                                                                      LastModified ),
                                                                                                                                                                                                     IDbReaderMapping<RecoveryCodeRecord>
{
    private static readonly PasswordHasher<RecoveryCodeRecord> _hasher = new();


    private RecoveryCodeRecord( string code, UserRecord caller ) : this( code, RecordID<RecoveryCodeRecord>.New(), caller.ID, caller.UserID, DateTimeOffset.UtcNow ) => Code = _hasher.HashPassword( this, code );

    public static RecoveryCodeRecord Create( DbDataReader reader )
    {
        DateTimeOffset        dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset        lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                  ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>  createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<GroupRecord> id           = new RecordID<GroupRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new RecoveryCodeRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
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
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user ) => Create( user,              Guid.NewGuid() );
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
}



[ Serializable, Table( "UserRecoveryCodes" ) ]
public sealed record UserRecoveryCodeRecord : Mapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, ICreateMapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, IDbReaderMapping<UserRecoveryCodeRecord>
{
    public UserRecoveryCodeRecord( UserRecord                                           owner, RecoveryCodeRecord value ) : base( owner, value ) { }
    [ RequiresPreviewFeatures ] public static UserRecoveryCodeRecord Create( UserRecord owner, RecoveryCodeRecord value ) => new(owner, value);


    public static UserRecoveryCodeRecord Create( DbDataReader reader )
    {
        DateTimeOffset        dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset        lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                  ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>  createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<GroupRecord> id           = new RecordID<GroupRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserRecoveryCodeRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserRecoveryCodeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
