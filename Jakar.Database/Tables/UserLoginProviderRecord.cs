// Jakar.Extensions :: Jakar.Database
// 01/30/2023  2:41 PM


using Microsoft.AspNetCore.DataProtection.KeyManagement;



namespace Jakar.Database;


[Serializable]
[Table(TABLE_NAME)]
public sealed record UserLoginProviderRecord( [property: StringLength(                                  UNICODE_CAPACITY)] string  LoginProvider,
                                              [property: StringLength(                                  UNICODE_CAPACITY)] string? ProviderDisplayName,
                                              [property: ProtectedPersonalData] [property: StringLength(UNICODE_CAPACITY)] string  ProviderKey,
                                              [property: ProtectedPersonalData] [property: StringLength(UNICODE_CAPACITY)] string? Value,
                                              RecordID<UserLoginProviderRecord>                                                    ID,
                                              RecordID<UserRecord>?                                                                CreatedBy,
                                              DateTimeOffset                                                                       DateCreated,
                                              DateTimeOffset?                                                                      LastModified = null ) : OwnedTableRecord<UserLoginProviderRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), ITableRecord<UserLoginProviderRecord>
{
    public const  string                                  TABLE_NAME = "user_login_providers";
    public static string                                  TableName     {  get => TABLE_NAME; }
    public static JsonSerializerContext                   JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<UserLoginProviderRecord>   JsonTypeInfo  => JakarDatabaseContext.Default.UserLoginProviderRecord;
    public static JsonTypeInfo<UserLoginProviderRecord[]> JsonArrayInfo => JakarDatabaseContext.Default.UserLoginProviderRecordArray;


    public static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { get; } = SqlTable<RoleRecord>.Create()
                                                                                                              .WithColumn<string>(nameof(LoginProvider),       length: UNICODE_CAPACITY)
                                                                                                              .WithColumn<string>(nameof(ProviderDisplayName), ColumnOptions.Nullable, length: UNICODE_CAPACITY)
                                                                                                              .WithColumn<string>(nameof(ProviderKey),         length: UNICODE_CAPACITY)
                                                                                                              .WithColumn<string>(nameof(Value),               length: UNICODE_CAPACITY)
                                                                                                              .With_CreatedBy()
                                                                                                              .Build();


    public UserLoginProviderRecord( UserRecord user, UserLoginInfo info ) : this(user, info.LoginProvider, info.ProviderKey, info.ProviderDisplayName) { }
    public UserLoginProviderRecord( UserRecord user, string        loginProvider, string providerKey, string? providerDisplayName ) : this(loginProvider, providerDisplayName, providerKey, string.Empty, RecordID<UserLoginProviderRecord>.New(), user.ID, DateTimeOffset.UtcNow) { }
    [Pure] public override PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(LoginProvider),       LoginProvider);
        parameters.Add(nameof(ProviderDisplayName), ProviderDisplayName);
        parameters.Add(nameof(ProviderKey),         ProviderKey);
        parameters.Add(nameof(Value),               Value);
        return parameters;
    }


    public static MigrationRecord CreateTable( ulong migrationID )
    {
        string tableID = TABLE_NAME.SqlColumnName();

        return MigrationRecord.Create<UserLoginProviderRecord>(migrationID,
                                                               $"create {tableID} table",
                                                               $"""
                                                                CREATE TABLE {tableID}
                                                                (
                                                                {nameof(LoginProvider).SqlColumnName()}       varchar({UNICODE_CAPACITY}) NOT NULL,
                                                                {nameof(ProviderDisplayName).SqlColumnName()} varchar({UNICODE_CAPACITY}) NOT NULL,
                                                                {nameof(ProviderKey).SqlColumnName()}         varchar({UNICODE_CAPACITY}) NOT NULL,
                                                                {nameof(Value).SqlColumnName()}               varchar({UNICODE_CAPACITY}) NOT NULL,
                                                                {nameof(ID).SqlColumnName()}                  uuid                        PRIMARY KEY,
                                                                {nameof(DateCreated).SqlColumnName()}         timestamptz                 NOT NULL DEFAULT SYSUTCDATETIME(),
                                                                {nameof(LastModified).SqlColumnName()}        timestamptz                 
                                                                );

                                                                CREATE TRIGGER {nameof(MigrationRecord.SetLastModified).SqlColumnName()}
                                                                BEFORE INSERT OR UPDATE ON {tableID}
                                                                FOR EACH ROW
                                                                EXECUTE FUNCTION {nameof(MigrationRecord.SetLastModified).SqlColumnName()}();
                                                                """);
    }
    [Pure] public static UserLoginProviderRecord Create( DbDataReader reader )
    {
        string                            loginProvider       = reader.GetFieldValue<string>(nameof(LoginProvider));
        string                            providerDisplayName = reader.GetFieldValue<string>(nameof(ProviderDisplayName));
        string                            providerKey         = reader.GetFieldValue<string>(nameof(ProviderKey));
        string                            value               = reader.GetFieldValue<string>(nameof(Value));
        DateTimeOffset                    dateCreated         = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?                   lastModified        = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));
        RecordID<UserRecord>?             ownerUserID         = RecordID<UserRecord>.CreatedBy(reader);
        RecordID<UserLoginProviderRecord> id                  = RecordID<UserLoginProviderRecord>.ID(reader);
        UserLoginProviderRecord           record              = new(loginProvider, providerDisplayName, providerKey, value, id, ownerUserID, dateCreated, lastModified);
        return record.Validate();
    }


    public static PostgresParameters GetDynamicParameters( UserRecord user, string value )
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(CreatedBy), user.ID.Value);
        parameters.Add(nameof(Value),     value);
        return parameters;
    }
    [Pure] public static PostgresParameters GetDynamicParameters( UserRecord user, UserLoginInfo info ) => GetDynamicParameters(user, info.LoginProvider, info.ProviderKey);
    [Pure] public static PostgresParameters GetDynamicParameters( UserRecord user, string loginProvider, string providerKey )
    {
        PostgresParameters parameters = GetDynamicParameters(user);
        parameters.Add(nameof(ProviderKey),   providerKey);
        parameters.Add(nameof(LoginProvider), loginProvider);
        return parameters;
    }

    [Pure] public UserLoginInfo ToUserLoginInfo() => new(LoginProvider, ProviderKey, ProviderDisplayName);


    public override bool Equals( UserLoginProviderRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other)                                                                                         &&
               string.Equals(LoginProvider,       other.LoginProvider,       StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(ProviderDisplayName, other.ProviderDisplayName, StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(ProviderKey,         other.ProviderKey,         StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(Value,               other.Value,               StringComparison.InvariantCultureIgnoreCase);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(LoginProvider,       StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(ProviderDisplayName, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(ProviderKey,         StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(Value,               StringComparer.InvariantCultureIgnoreCase);
        return hashCode.ToHashCode();
    }
    public static bool operator >( UserLoginProviderRecord  left, UserLoginProviderRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( UserLoginProviderRecord left, UserLoginProviderRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( UserLoginProviderRecord  left, UserLoginProviderRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( UserLoginProviderRecord left, UserLoginProviderRecord right ) => left.CompareTo(right) <= 0;


    public static implicit operator UserLoginInfo( UserLoginProviderRecord value ) => value.ToUserLoginInfo();
    public static implicit operator IdentityUserToken<string>( UserLoginProviderRecord value ) => new()
                                                                                                  {
                                                                                                      UserId        = value.CreatedBy?.ToString() ?? throw new NullReferenceException(nameof(value.CreatedBy)),
                                                                                                      LoginProvider = value.LoginProvider,
                                                                                                      Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                      Value         = value.ProviderKey
                                                                                                  };
    public static implicit operator IdentityUserToken<Guid>( UserLoginProviderRecord value ) => new()
                                                                                                {
                                                                                                    UserId        = value.CreatedBy?.Value ?? Guid.Empty,
                                                                                                    LoginProvider = value.LoginProvider,
                                                                                                    Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                    Value         = value.ProviderKey
                                                                                                };
}
