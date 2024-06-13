// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:26 PM

using System.Collections.ObjectModel;



namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record RecoveryCodeRecord( string Code, RecordID<RecoveryCodeRecord> ID, RecordID<UserRecord>? CreatedBy, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default ) : OwnedTableRecord<RecoveryCodeRecord>( CreatedBy, ID, DateCreated, LastModified ), IDbReaderMapping<RecoveryCodeRecord>
{
    public const            string                             TABLE_NAME = "Codes";
    private static readonly PasswordHasher<RecoveryCodeRecord> _hasher    = new();
    public static           string                             TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }


    public RecoveryCodeRecord( string code, UserRecord user ) : this( code, RecordID<RecoveryCodeRecord>.New(), user.ID, DateTimeOffset.UtcNow ) { }


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(Code), Code );
        return parameters;
    }
    [Pure]
    public static RecoveryCodeRecord Create( DbDataReader reader )
    {
        string                       code         = reader.GetFieldValue<string>( nameof(Code) );
        DateTimeOffset               dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?              lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserRecord>?        ownerUserID  = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<RecoveryCodeRecord> id           = RecordID<RecoveryCodeRecord>.ID( reader );
        RecoveryCodeRecord           record       = new RecoveryCodeRecord( code, id, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<RecoveryCodeRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public static ReadOnlyDictionary<string, RecoveryCodeRecord> Create( UserRecord user, IEnumerable<string>              recoveryCodes ) => Create( user, recoveryCodes.GetInternalArray() );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public static ReadOnlyDictionary<string, RecoveryCodeRecord> Create( UserRecord user, List<string>                     recoveryCodes ) => Create( user, CollectionsMarshal.AsSpan( recoveryCodes ) );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public static ReadOnlyDictionary<string, RecoveryCodeRecord> Create( UserRecord user, scoped in ReadOnlyMemory<string> recoveryCodes ) => Create( user, recoveryCodes.Span );

    [Pure]
    public static ReadOnlyDictionary<string, RecoveryCodeRecord> Create( UserRecord user, scoped in ReadOnlySpan<string> recoveryCodes )
    {
        RecoveryCodeMapping codes = new(recoveryCodes.Length);

        foreach ( string recoveryCode in recoveryCodes )
        {
            (string code, RecoveryCodeRecord record) = Create( user, recoveryCode );
            codes[code]                              = record;
        }

        return new ReadOnlyDictionary<string, RecoveryCodeRecord>( codes );
    }
    [Pure]
    public static async ValueTask<ReadOnlyDictionary<string, RecoveryCodeRecord>> Create( UserRecord user, IAsyncEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        RecoveryCodeMapping codes = new(10);

        await foreach ( string recoveryCode in recoveryCodes.WithCancellation( token ) )
        {
            (string code, RecoveryCodeRecord record) = Create( user, recoveryCode );
            codes[code]                              = record;
        }

        return new ReadOnlyDictionary<string, RecoveryCodeRecord>( codes );
    }
    [Pure]
    public static ReadOnlyDictionary<string, RecoveryCodeRecord> Create( UserRecord user, int count )
    {
        RecoveryCodeMapping codes = new(count);

        for ( int i = 0; i < count; i++ )
        {
            (string code, RecoveryCodeRecord record) = Create( user );
            codes[code]                              = record;
        }

        return new ReadOnlyDictionary<string, RecoveryCodeRecord>( codes );
    }
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user )              => Create( user, Guid.NewGuid() );
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, Guid   code ) => Create( user, code.ToBase64() );
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, string code ) => (code, new RecoveryCodeRecord( code, user ));

    [Pure]
    public static bool IsValid( string code, ref RecoveryCodeRecord record )
    {
        switch ( _hasher.VerifyHashedPassword( record, record.Code, code ) )
        {
            case PasswordVerificationResult.Failed:  return false;
            case PasswordVerificationResult.Success: return true;

            case PasswordVerificationResult.SuccessRehashNeeded:
                record = record with { Code = _hasher.HashPassword( record, code ) };

                return true;

            default: throw new ArgumentOutOfRangeException();
        }
    }



    public sealed class RecoveryCodeMapping( int count ) : Dictionary<string, RecoveryCodeRecord>( count, StringComparer.Ordinal );
}



[Serializable, Table( TABLE_NAME )]
public sealed record UserRecoveryCodeRecord : Mapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, ICreateMapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, IDbReaderMapping<UserRecoveryCodeRecord>
{
    public const  string TABLE_NAME = "UserRecoveryCodes";
    public static string TableName => TABLE_NAME;

    public UserRecoveryCodeRecord( UserRecord               owner, RecoveryCodeRecord           value ) : base( owner, value ) { }
    public UserRecoveryCodeRecord( RecordID<UserRecord>     key,   RecordID<RecoveryCodeRecord> value, RecordID<UserRecoveryCodeRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified = default ) : base( key, value, id, dateCreated, lastModified ) { }
    public static UserRecoveryCodeRecord Create( UserRecord owner, RecoveryCodeRecord           value ) => new(owner, value);


    public static UserRecoveryCodeRecord Create( DbDataReader reader )
    {
        RecordID<UserRecord>             key          = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        RecordID<RecoveryCodeRecord>     value        = new RecordID<RecoveryCodeRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        DateTimeOffset                   dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?                  lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        RecordID<UserRecoveryCodeRecord> id           = new RecordID<UserRecoveryCodeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserRecoveryCodeRecord( key, value, id, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserRecoveryCodeRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
