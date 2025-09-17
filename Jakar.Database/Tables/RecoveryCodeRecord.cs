// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:26 PM

using Jakar.Database.Resx;



namespace Jakar.Database;


[Serializable, Table(TABLE_NAME)]
public sealed record RecoveryCodeRecord( string Code, RecordID<RecoveryCodeRecord> ID, RecordID<UserRecord>? CreatedBy, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<RecoveryCodeRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), IDbReaderMapping<RecoveryCodeRecord>
{
    public const            string                             TABLE_NAME = "recovery_codes";
    private static readonly PasswordHasher<RecoveryCodeRecord> __hasher   = new();
    public static           string                             TableName { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TABLE_NAME; }


    public RecoveryCodeRecord( string code, UserRecord user ) : this(code, RecordID<RecoveryCodeRecord>.New(), user.ID, DateTimeOffset.UtcNow) { }


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(Code), Code);
        return parameters;
    }
    [Pure]
    public static RecoveryCodeRecord Create( DbDataReader reader )
    {
        string                       code         = reader.GetFieldValue<string>(nameof(Code));
        DateTimeOffset               dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?              lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>?        ownerUserID  = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<RecoveryCodeRecord> id           = RecordID<RecoveryCodeRecord>.ID(reader);
        RecoveryCodeRecord           record       = new(code, id, ownerUserID, dateCreated, lastModified);
        return record.Validate();
    }
    [Pure]
    public static async IAsyncEnumerable<RecoveryCodeRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync(token) ) { yield return Create(reader); }
    }


    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)] public static Codes Create( UserRecord user, IEnumerable<string>              recoveryCodes ) => Create(user, recoveryCodes.GetInternalArray());
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)] public static Codes Create( UserRecord user, List<string>                     recoveryCodes ) => Create(user, CollectionsMarshal.AsSpan(recoveryCodes));
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)] public static Codes Create( UserRecord user, scoped in ReadOnlyMemory<string> recoveryCodes ) => Create(user, recoveryCodes.Span);

    [Pure]
    public static Codes Create( UserRecord user, scoped in ReadOnlySpan<string> recoveryCodes )
    {
        Codes codes = new(recoveryCodes.Length);

        foreach ( string recoveryCode in recoveryCodes )
        {
            ( string code, RecoveryCodeRecord record ) = Create(user, recoveryCode);
            codes[code]                                = record;
        }

        return codes;
    }
    [Pure]
    public static async ValueTask<Codes> Create( UserRecord user, IAsyncEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        Codes codes = new(10);

        await foreach ( string recoveryCode in recoveryCodes.WithCancellation(token) )
        {
            ( string code, RecoveryCodeRecord record ) = Create(user, recoveryCode);
            codes[code]                                = record;
        }

        return codes;
    }
    [Pure]
    public static Codes Create( UserRecord user, int count )
    {
        Codes codes = new(count);

        for ( int i = 0; i < count; i++ )
        {
            ( string code, RecoveryCodeRecord record ) = Create(user);
            codes[code]                                = record;
        }

        return codes;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user )              => Create(user, Guid.CreateVersion7());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, Guid   code ) => Create(user, code.ToBase64());
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, string code ) => ( code, new RecoveryCodeRecord(code, user) );

    [Pure]
    public static bool IsValid( string code, ref RecoveryCodeRecord record )
    {
        switch ( __hasher.VerifyHashedPassword(record, record.Code, code) )
        {
            case PasswordVerificationResult.Failed:
                return false;

            case PasswordVerificationResult.Success:
                return true;

            case PasswordVerificationResult.SuccessRehashNeeded:
                record = record with { Code = __hasher.HashPassword(record, code) };

                return true;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public override bool Equals( RecoveryCodeRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && string.Equals(Code, other.Code, StringComparison.InvariantCultureIgnoreCase);
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Code);


    public static bool operator >( RecoveryCodeRecord  left, RecoveryCodeRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( RecoveryCodeRecord left, RecoveryCodeRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( RecoveryCodeRecord  left, RecoveryCodeRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( RecoveryCodeRecord left, RecoveryCodeRecord right ) => left.CompareTo(right) <= 0;



    public sealed class Codes( int count ) : IReadOnlyDictionary<string, RecoveryCodeRecord>
    {
        private readonly Dictionary<string, RecoveryCodeRecord> __codes = new(count, StringComparer.Ordinal);
        public           int                                    Count => __codes.Count;
        public RecoveryCodeRecord this[ string key ] { get => __codes[key]; internal set => __codes[key] = value; }
        public IEnumerable<string>                                   Keys                                                                           => __codes.Keys;
        public IEnumerable<RecoveryCodeRecord>                       Values                                                                         => __codes.Values;
        public IEnumerator<KeyValuePair<string, RecoveryCodeRecord>> GetEnumerator()                                                                => __codes.GetEnumerator();
        IEnumerator IEnumerable.                                     GetEnumerator()                                                                => ( (IEnumerable)__codes ).GetEnumerator();
        public bool                                                  ContainsKey( string key )                                                      => __codes.ContainsKey(key);
        public bool                                                  TryGetValue( string key, [MaybeNullWhen(false)] out RecoveryCodeRecord value ) => __codes.TryGetValue(key, out value);
    }
}



[Serializable, Table(TABLE_NAME)]
public sealed record UserRecoveryCodeRecord : Mapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, ICreateMapping<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>, IDbReaderMapping<UserRecoveryCodeRecord>
{
    public const  string TABLE_NAME = "UserRecoveryCodes";
    public static string TableName => TABLE_NAME;

    public UserRecoveryCodeRecord( UserRecord                         key, RecoveryCodeRecord           value ) : base(key, value) { }
    public UserRecoveryCodeRecord( RecordID<UserRecord>               key, RecordID<RecoveryCodeRecord> value ) : base(key, value) { }
    public UserRecoveryCodeRecord( RecordID<UserRecord>               key, RecordID<RecoveryCodeRecord> value, RecordID<UserRecoveryCodeRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified = null ) : base(key, value, id, dateCreated, lastModified) { }
    public static UserRecoveryCodeRecord Create( UserRecord           key, RecoveryCodeRecord           value ) => new(key, value);
    public static UserRecoveryCodeRecord Create( RecordID<UserRecord> key, RecordID<RecoveryCodeRecord> value ) => new(key, value);


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
