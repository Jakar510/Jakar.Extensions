// Jakar.Extensions :: Jakar.Database
// 01/30/2023  2:41 PM

namespace Jakar.Database;


[Serializable]
[Table( "UserLoginInfo" )]
public sealed record UserLoginInfoRecord : TableRecord<UserLoginInfoRecord>
{
    private string  _loginProvider = string.Empty;
    private string  _providerKey   = string.Empty;
    private string? _providerDisplayName;
    private string? _value;


    [MaxLength( int.MaxValue )]
    public string LoginProvider
    {
        get => _loginProvider;
        set => SetProperty( ref _loginProvider, value );
    }

    [MaxLength( int.MaxValue )]
    public string? ProviderDisplayName
    {
        get => _providerDisplayName;
        set => SetProperty( ref _providerDisplayName, value );
    }

    [ProtectedPersonalData]
    [MaxLength( int.MaxValue )]
    public string ProviderKey
    {
        get => _providerKey;
        set => SetProperty( ref _providerKey, value );
    }

    [ProtectedPersonalData]
    public string? Value
    {
        get => _value;
        set => SetProperty( ref _value, value );
    }


    public UserLoginInfoRecord() { }
    public UserLoginInfoRecord( UserRecord user, UserLoginInfo info ) : this( user, info.LoginProvider, info.ProviderKey, info.ProviderDisplayName ) { }
    public UserLoginInfoRecord( UserRecord user, string loginProvider, string providerKey, string? providerDisplayName ) : base( Guid.NewGuid(), user )
    {
        LoginProvider       = loginProvider;
        ProviderKey         = providerKey;
        ProviderDisplayName = providerDisplayName;
    }


    public static DynamicParameters GetDynamicParameters( UserRecord user, UserLoginInfo info ) => GetDynamicParameters( user, info.LoginProvider, info.ProviderKey );
    public static DynamicParameters GetDynamicParameters( UserRecord user, string loginProvider, string providerKey )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserRecord.CreatedBy),   user.CreatedBy );
        parameters.Add( nameof(UserRecord.OwnerUserID), user.OwnerUserID );
        parameters.Add( nameof(ProviderKey),            providerKey );
        parameters.Add( nameof(LoginProvider),          loginProvider );
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
                                                                                                UserId        = value.OwnerUserID ?? throw new NullReferenceException( nameof(value.OwnerUserID) ),
                                                                                                LoginProvider = value.LoginProvider,
                                                                                                Name          = value.ProviderDisplayName ?? string.Empty,
                                                                                                Value         = value.ProviderKey,
                                                                                            };
}
