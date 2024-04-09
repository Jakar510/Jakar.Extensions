// Jakar.Extensions :: Jakar.Database
// 01/30/2023  2:41 PM

namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record UserLoginInfoRecord( [property: StringLength(                        int.MaxValue )]  string  LoginProvider,
                                          [property: StringLength(                        int.MaxValue )]  string? ProviderDisplayName,
                                          [property: ProtectedPersonalData, StringLength( int.MaxValue )]  string  ProviderKey,
                                          [property: ProtectedPersonalData]                                string? Value,
                                          RecordID<UserLoginInfoRecord>                                            ID,
                                          RecordID<UserRecord>?                                                    CreatedBy,
                                          Guid?                                                                    OwnerUserID,
                                          DateTimeOffset                                                           DateCreated,
                                          DateTimeOffset?                                                          LastModified = default ) : OwnedTableRecord<UserLoginInfoRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<UserLoginInfoRecord>
{
    public const  string TABLE_NAME = "UserLoginInfo";
    public static string TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }


    public UserLoginInfoRecord( UserRecord user, UserLoginInfo info ) : this( user, info.LoginProvider, info.ProviderKey, info.ProviderDisplayName ) { }
    public UserLoginInfoRecord( UserRecord user, string        loginProvider, string providerKey, string? providerDisplayName ) : this( loginProvider, providerDisplayName, providerKey, string.Empty, RecordID<UserLoginInfoRecord>.New(), user.ID, user.UserID, DateTimeOffset.UtcNow ) { }
    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(LoginProvider),       LoginProvider );
        parameters.Add( nameof(ProviderDisplayName), ProviderDisplayName );
        parameters.Add( nameof(ProviderKey),         ProviderKey );
        parameters.Add( nameof(Value),               Value );
        return parameters;
    }

    [Pure]
    public static UserLoginInfoRecord Create( DbDataReader reader )
    {
        string                        loginProvider       = reader.GetFieldValue<string>( nameof(LoginProvider) );
        string                        providerDisplayName = reader.GetFieldValue<string>( nameof(ProviderDisplayName) );
        string                        providerKey         = reader.GetFieldValue<string>( nameof(ProviderKey) );
        string                        value               = reader.GetFieldValue<string>( nameof(Value) );
        DateTimeOffset                dateCreated         = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?               lastModified        = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        Guid                           ownerUserID         = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>?         createdBy           = RecordID<UserRecord>.CreatedBy( reader );
        RecordID<UserLoginInfoRecord> id                  = RecordID<UserLoginInfoRecord>.ID( reader );
        UserLoginInfoRecord                           record              = new UserLoginInfoRecord( loginProvider, providerDisplayName, providerKey, value, id, createdBy, ownerUserID, dateCreated, lastModified );
        record.Validate();
        return record;
    }

    [Pure]
    public static async IAsyncEnumerable<UserLoginInfoRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    public static DynamicParameters GetDynamicParameters( UserRecord user, string value )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(CreatedBy),   user.ID.Value );
        parameters.Add( nameof(OwnerUserID), user.UserID );
        parameters.Add( nameof(Value),       value );
        return parameters;
    }
    [Pure] public static DynamicParameters GetDynamicParameters( UserRecord user, UserLoginInfo info ) => GetDynamicParameters( user, info.LoginProvider, info.ProviderKey );
    [Pure]
    public static DynamicParameters GetDynamicParameters( UserRecord user, string loginProvider, string providerKey )
    {
        DynamicParameters parameters = GetDynamicParameters( user );
        parameters.Add( nameof(ProviderKey),   providerKey );
        parameters.Add( nameof(LoginProvider), loginProvider );
        return parameters;
    }

    [Pure] public UserLoginInfo ToUserLoginInfo() => new(LoginProvider, ProviderKey, ProviderDisplayName);


    public static implicit operator UserLoginInfo( UserLoginInfoRecord value ) => value.ToUserLoginInfo();
    public static implicit operator IdentityUserToken<string>( UserLoginInfoRecord value ) => new()
                                                                                              {
                                                                                                  UserId        = value.OwnerUserID?.ToString() ?? throw new NullReferenceException( nameof(value.OwnerUserID) ),
                                                                                                  LoginProvider = value.LoginProvider,
                                                                                                  Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                  Value         = value.ProviderKey
                                                                                              };
    public static implicit operator IdentityUserToken<Guid>( UserLoginInfoRecord value ) => new()
                                                                                            {
                                                                                                UserId        = value.OwnerUserID ?? Guid.Empty,
                                                                                                LoginProvider = value.LoginProvider,
                                                                                                Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                Value         = value.ProviderKey
                                                                                            };
}
