// Jakar.Extensions :: Jakar.Database
// 02/26/2023  9:12 PM

namespace Jakar.Database;


[Serializable]
[Table( "UserRecoveryCodes" )]
public sealed record UserRecoveryCodeRecord : Mapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, ICreateMapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>
{
    public UserRecoveryCodeRecord() : base() { }
    public UserRecoveryCodeRecord( UserRecord                                         owner, RecoveryCodeRecord value ) : base( owner, value ) { }
    [RequiresPreviewFeatures] public static UserRecoveryCodeRecord Create( UserRecord owner, RecoveryCodeRecord value ) => new(owner, value);
}
