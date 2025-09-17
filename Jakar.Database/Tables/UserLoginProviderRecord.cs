// Jakar.Extensions :: Jakar.Database
// 01/30/2023  2:41 PM

namespace Jakar.Database;


[Serializable, Table(TABLE_NAME)]
public sealed record UserLoginProviderRecord( [property: StringLength(                       int.MaxValue)] string  LoginProvider,
                                              [property: StringLength(                       int.MaxValue)] string? ProviderDisplayName,
                                              [property: ProtectedPersonalData, StringLength(int.MaxValue)] string  ProviderKey,
                                              [property: ProtectedPersonalData]                             string? Value,
                                              RecordID<UserLoginProviderRecord>                                     ID,
                                              RecordID<UserRecord>?                                                 CreatedBy,
                                              DateTimeOffset                                                        DateCreated,
                                              DateTimeOffset?                                                       LastModified = null ) : OwnedTableRecord<UserLoginProviderRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), IDbReaderMapping<UserLoginProviderRecord>
{
    public const  string TABLE_NAME = "user_login_providers";
    public static string TableName { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TABLE_NAME; }


    public UserLoginProviderRecord( UserRecord user, UserLoginInfo info ) : this(user, info.LoginProvider, info.ProviderKey, info.ProviderDisplayName) { }
    public UserLoginProviderRecord( UserRecord user, string        loginProvider, string providerKey, string? providerDisplayName ) : this(loginProvider, providerDisplayName, providerKey, string.Empty, RecordID<UserLoginProviderRecord>.New(), user.ID, DateTimeOffset.UtcNow) { }
    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(LoginProvider),       LoginProvider);
        parameters.Add(nameof(ProviderDisplayName), ProviderDisplayName);
        parameters.Add(nameof(ProviderKey),         ProviderKey);
        parameters.Add(nameof(Value),               Value);
        return parameters;
    }

    [Pure]
    public static UserLoginProviderRecord Create( DbDataReader reader )
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

    [Pure]
    public static async IAsyncEnumerable<UserLoginProviderRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync(token) ) { yield return Create(reader); }
    }

    public static DynamicParameters GetDynamicParameters( UserRecord user, string value )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(CreatedBy), user.ID.value);
        parameters.Add(nameof(Value),     value);
        return parameters;
    }
    [Pure] public static DynamicParameters GetDynamicParameters( UserRecord user, UserLoginInfo info ) => GetDynamicParameters(user, info.LoginProvider, info.ProviderKey);
    [Pure]
    public static DynamicParameters GetDynamicParameters( UserRecord user, string loginProvider, string providerKey )
    {
        DynamicParameters parameters = GetDynamicParameters(user);
        parameters.Add(nameof(ProviderKey),   providerKey);
        parameters.Add(nameof(LoginProvider), loginProvider);
        return parameters;
    }

    [Pure] public UserLoginInfo ToUserLoginInfo() => new(LoginProvider, ProviderKey, ProviderDisplayName);

    
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
                                                                                                    UserId        = value.CreatedBy?.value ?? Guid.Empty,
                                                                                                    LoginProvider = value.LoginProvider,
                                                                                                    Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                    Value         = value.ProviderKey
                                                                                                };
}
