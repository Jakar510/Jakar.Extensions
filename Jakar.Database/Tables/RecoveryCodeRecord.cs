// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:26 PM

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;



namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
public sealed record RecoveryCodeRecord( [property: StringLength(1024)] string Code, RecordID<RecoveryCodeRecord> ID, RecordID<UserRecord>? CreatedBy, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : OwnedTableRecord<RecoveryCodeRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), ITableRecord<RecoveryCodeRecord>
{
    public const            string                             TABLE_NAME = "recovery_codes";
    private static readonly PasswordHasher<RecoveryCodeRecord> __hasher   = new();
    public static           string                             TableName     { get => TABLE_NAME; }
    public static           JsonSerializerContext              JsonContext   => JakarDatabaseContext.Default;
    public static           JsonTypeInfo<RecoveryCodeRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.RecoveryCodeRecord;
    public static           JsonTypeInfo<RecoveryCodeRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.RecoveryCodeRecordArray;


    public static FrozenDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<RecoveryCodeRecord>.Default.WithColumn<string>(nameof(Code), length: 1024)
                                                                                                                   .With_CreatedBy()
                                                                                                                   .Build();


    public RecoveryCodeRecord( string code, UserRecord user ) : this(code, RecordID<RecoveryCodeRecord>.New(), user.ID, DateTimeOffset.UtcNow) { }


    [Pure] public override PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(Code), Code);
        return parameters;
    }
    [Pure] public static RecoveryCodeRecord Create( DbDataReader reader )
    {
        string                       code         = reader.GetFieldValue<string>(nameof(Code));
        DateTimeOffset               dateCreated  = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?              lastModified = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>?        ownerUserID  = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<RecoveryCodeRecord> id           = RecordID<RecoveryCodeRecord>.ID(reader);
        RecoveryCodeRecord           record       = new(code, id, ownerUserID, dateCreated, lastModified);
        return record.Validate();
    }


    public static MigrationRecord CreateTable( ulong migrationID )
    {
        return MigrationRecord.Create<UserRecord>(migrationID,
                                                  $"create {TABLE_NAME} table",
                                                  $"""
                                                   CREATE TABLE IF NOT EXISTS {TABLE_NAME}
                                                   (  
                                                   {nameof(Code).SqlColumnName()}           VARCHAR(1024)  NOT NULL, 
                                                   {nameof(AdditionalData).SqlColumnName()} json           NULL,
                                                   {nameof(ID).SqlColumnName()}             uuid           PRIMARY KEY,
                                                   {nameof(CreatedBy).SqlColumnName()}      uuid           NULL,
                                                   {nameof(DateCreated).SqlColumnName()}    timestamptz    NOT NULL,
                                                   {nameof(LastModified).SqlColumnName()}   timestamptz    NULL,
                                                   FOREIGN KEY({nameof(CreatedBy).SqlColumnName()}) REFERENCES {UserRecord.TABLE_NAME.SqlColumnName()}(id) ON DELETE SET NULL 
                                                   );

                                                   CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                                   BEFORE INSERT OR UPDATE ON {TABLE_NAME}
                                                   FOR EACH ROW
                                                   EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();
                                                   """);
    }


    [Pure] public static Codes Create( UserRecord user, IEnumerable<string>              recoveryCodes ) => Create(user, recoveryCodes.GetInternalArray());
    [Pure] public static Codes Create( UserRecord user, List<string>                     recoveryCodes ) => Create(user, CollectionsMarshal.AsSpan(recoveryCodes));
    [Pure] public static Codes Create( UserRecord user, scoped in ReadOnlyMemory<string> recoveryCodes ) => Create(user, recoveryCodes.Span);
    [Pure] public static Codes Create( UserRecord user, params ReadOnlySpan<string> recoveryCodes )
    {
        Codes codes = new(recoveryCodes.Length);

        foreach ( string recoveryCode in recoveryCodes )
        {
            ( string code, RecoveryCodeRecord record ) = Create(user, recoveryCode);
            codes[code]                                = record;
        }

        return codes;
    }
    [Pure] public static async ValueTask<Codes> Create( UserRecord user, IAsyncEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        Codes codes = new(10);

        await foreach ( string recoveryCode in recoveryCodes.WithCancellation(token) )
        {
            ( string code, RecoveryCodeRecord record ) = Create(user, recoveryCode);
            codes[code]                                = record;
        }

        return codes;
    }
    [Pure] public static Codes Create( UserRecord user, int count )
    {
        Codes codes = new(count);

        for ( int i = 0; i < count; i++ )
        {
            ( string code, RecoveryCodeRecord record ) = Create(user);
            codes[code]                                = record;
        }

        return codes;
    }
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user )              => Create(user, Guid.CreateVersion7());
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, Guid   code ) => Create(user, code.ToHex());
    public static (string Code, RecoveryCodeRecord Record) Create( UserRecord user, string code ) => ( code, new RecoveryCodeRecord(code, user) );


    [Pure] public static bool IsValid( string code, ref RecoveryCodeRecord record )
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
        public           int                                    Count  => __codes.Count;
        public           IEnumerable<string>                    Keys   => __codes.Keys;
        public           IEnumerable<RecoveryCodeRecord>        Values => __codes.Values;
        public RecoveryCodeRecord this[ string key ] { get => __codes[key]; internal set => __codes[key] = value; }
        public IEnumerator<KeyValuePair<string, RecoveryCodeRecord>> GetEnumerator()                                                                => __codes.GetEnumerator();
        IEnumerator IEnumerable.                                     GetEnumerator()                                                                => ( (IEnumerable)__codes ).GetEnumerator();
        public bool                                                  ContainsKey( string key )                                                      => __codes.ContainsKey(key);
        public bool                                                  TryGetValue( string key, [MaybeNullWhen(false)] out RecoveryCodeRecord value ) => __codes.TryGetValue(key, out value);
    }
}
