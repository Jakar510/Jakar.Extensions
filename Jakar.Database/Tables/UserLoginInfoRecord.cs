// Jakar.Extensions :: Jakar.Database
// 01/30/2023  2:41 PM

namespace Jakar.Database;


[Serializable,Table( "UserLoginInfo" )]
public sealed record UserLoginInfoRecord( [property: MaxLength(                        int.MaxValue )] string  LoginProvider,
                                          [property: MaxLength(                        int.MaxValue )] string? ProviderDisplayName,
                                          [property: ProtectedPersonalData, MaxLength( int.MaxValue )] string  ProviderKey,
                                          [property: ProtectedPersonalData]                            string? Value,
                                          RecordID<UserLoginInfoRecord>                                        ID,
                                          RecordID<UserRecord>?                                                CreatedBy,
                                          Guid?                                                                OwnerUserID,
                                          DateTimeOffset                                                       DateCreated,
                                          DateTimeOffset?                                                      LastModified = default
) : TableRecord<UserLoginInfoRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<UserLoginInfoRecord>
{
    public UserLoginInfoRecord( UserRecord user, UserLoginInfo info ) : this( user, info.LoginProvider, info.ProviderKey, info.ProviderDisplayName ) { }
    public UserLoginInfoRecord( UserRecord user, string loginProvider, string providerKey, string? providerDisplayName ) : this( loginProvider,
                                                                                                                                 providerDisplayName,
                                                                                                                                 providerKey,
                                                                                                                                 string.Empty,
                                                                                                                                 RecordID<UserLoginInfoRecord>.New(),
                                                                                                                                 user.ID,
                                                                                                                                 user.UserID,
                                                                                                                                 DateTimeOffset.UtcNow ) { }


    public static UserLoginInfoRecord Create( DbDataReader reader )
    {
        var loginProvider       = reader.GetString( nameof(LoginProvider) );
        var providerDisplayName = reader.GetString( nameof(ProviderDisplayName) );
        var providerKey         = reader.GetString( nameof(ProviderKey) );
        var value               = reader.GetString( nameof(Value) );
        var dateCreated         = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified        = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        var ownerUserID         = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var createdBy           = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var id                  = new RecordID<UserLoginInfoRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );

        return new UserLoginInfoRecord( loginProvider, providerDisplayName, providerKey, value, id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserLoginInfoRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static DynamicParameters GetDynamicParameters( UserRecord user, UserLoginInfo info ) => GetDynamicParameters( user, info.LoginProvider, info.ProviderKey );
    public static DynamicParameters GetDynamicParameters( UserRecord user, string loginProvider, string providerKey )
    {
        DynamicParameters parameters = GetDynamicParameters( user );
        parameters.Add( nameof(ProviderKey),   providerKey );
        parameters.Add( nameof(LoginProvider), loginProvider );
        return parameters;
    }


    public UserLoginInfo ToUserLoginInfo() => new(LoginProvider, ProviderKey, ProviderDisplayName);


    public static implicit operator UserLoginInfo( UserLoginInfoRecord value ) => value.ToUserLoginInfo();
    public static implicit operator IdentityUserToken<string>( UserLoginInfoRecord value ) => new()
                                                                                              {
                                                                                                  UserId        = value.OwnerUserID?.ToString() ?? throw new NullReferenceException( nameof(value.OwnerUserID) ),
                                                                                                  LoginProvider = value.LoginProvider,
                                                                                                  Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                  Value         = value.ProviderKey,
                                                                                              };
    public static implicit operator IdentityUserToken<Guid>( UserLoginInfoRecord value ) => new()
                                                                                            {
                                                                                                UserId        = value.OwnerUserID ?? Guid.Empty,
                                                                                                LoginProvider = value.LoginProvider,
                                                                                                Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                Value         = value.ProviderKey,
                                                                                            };
}
