// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:26 PM

namespace Jakar.Database;


[Serializable]
[Table( "Codes" )]
public sealed record RecoveryCodeRecord : TableRecord<RecoveryCodeRecord>
{
    private static readonly PasswordHasher<RecoveryCodeRecord> _hasher = new();
    private                 string                             _value  = string.Empty;


    [MaxLength( 10240 )]
    public string Value
    {
        get => _value;
        set => SetProperty( ref _value, value );
    }


    public RecoveryCodeRecord() { }
    private RecoveryCodeRecord( string value, UserRecord caller ) : base( Guid.NewGuid(), caller ) => Value = _hasher.HashPassword( this, value );


    public static (RecoveryCodeRecord Record, string Code) Create( UserRecord user )
    {
        string value = Guid.NewGuid()
                           .ToBase64();

        return (Create( user, value ), value);
    }
    public static RecoveryCodeRecord Create( UserRecord user, string value ) => new(value, user);


    public bool IsValid( string value )
    {
        switch ( _hasher.VerifyHashedPassword( this, Value, value ) )
        {
            case PasswordVerificationResult.Failed:  return false;
            case PasswordVerificationResult.Success: return true;

            case PasswordVerificationResult.SuccessRehashNeeded:
                Value = _hasher.HashPassword( this, value );
                return true;

            default: throw new ArgumentOutOfRangeException();
        }
    }
}
