// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:26 PM

namespace Jakar.Database;


[Serializable]
[Table( "Codes" )]
public sealed record RecoveryCodeRecord : TableRecord<RecoveryCodeRecord>
{
    private static readonly PasswordHasher<RecoveryCodeRecord> _hasher = new();
    private                 string                             _code   = string.Empty;


    [MaxLength( 1024 )]
    public string Code
    {
        get => _code;
        set => SetProperty( ref _code, value );
    }


    public RecoveryCodeRecord() { }
    private RecoveryCodeRecord( string code, UserRecord caller ) : base( Guid.NewGuid(), caller ) => Code = _hasher.HashPassword( this, code );


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


    public bool IsValid( string code )
    {
        switch ( _hasher.VerifyHashedPassword( this, Code, code ) )
        {
            case PasswordVerificationResult.Failed:  return false;
            case PasswordVerificationResult.Success: return true;

            case PasswordVerificationResult.SuccessRehashNeeded:
                Code = _hasher.HashPassword( this, code );
                return true;

            default: throw new ArgumentOutOfRangeException();
        }
    }
}
