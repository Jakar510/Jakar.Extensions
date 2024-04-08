// Jakar.Extensions :: Jakar.Extensions
// 4/5/2024  11:1

using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;



namespace Jakar.Extensions;


public sealed class JwtParser( SigningCredentials credentials, TokenValidationParameters parameters, string appName, string issuer, AppVersion version )
{
    public const            string                                  VERIFY_EMAIL  = "VerifyEmail";
    public const            string                                  VERIFY_DEVICE = "VerifyDevice";
    public const            string                                  SESSION       = "Session";
    private static readonly ConcurrentDictionary<string, JwtParser> _parsers      = new(StringComparer.Ordinal);
    private readonly        AppVersion                              _version      = version;
    private readonly        JsonWebTokenHandler                     _handler      = new();
    private readonly        SigningCredentials                      _credentials  = credentials;
    private readonly        string                                  _appName      = appName;
    private readonly        string                                  _issuer       = issuer;
    private readonly        TokenValidationParameters               _parameters   = parameters;


    public static async ValueTask<JwtParser> GetOrCreateParser<T>( IWebAppSettings settings, string authenticationType )
        where T : IAppName
    {
        if ( _parsers.TryGetValue( authenticationType, out JwtParser? parser ) is false ) { _parsers[authenticationType] = parser = await CreateAsync<T>( settings, authenticationType ); }

        return parser;
    }
    public static async ValueTask<JwtParser> CreateAsync<T>( IWebAppSettings settings, string authenticationType )
        where T : IAppName
    {
        SigningCredentials        credentials = await settings.Configuration.GetSigningCredentials();
        TokenValidationParameters parameters  = await settings.GetTokenValidationParameters( authenticationType );
        return new JwtParser( credentials, parameters, settings.AppName, typeof(T).Name, settings.Version );
    }


    public Tokens<Guid> CreateToken( UserGuid.UserModel user, ISessionID<Guid> request, string authenticationType ) => CreateToken<UserGuid.UserModel, UserGuid.UserAddress, UserGuid.GroupModel, UserGuid.RoleModel, Guid>( user, request, authenticationType );
    public Tokens<long> CreateToken( UserLong.UserModel user, ISessionID<long> request, string authenticationType ) => CreateToken<UserLong.UserModel, UserLong.UserAddress, UserLong.GroupModel, UserLong.RoleModel, long>( user, request, authenticationType );
    public Tokens<TID> CreateToken<TUser, TID>( TUser user, ISessionID<TID> request, string authenticationType )
#if NET8_0_OR_GREATER
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
    #elif NET6_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
    #else
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    #endif
    #if NET8_0_OR_GREATER
        where TUser : UserModel<TUser, TID>, ICreateUserModel<TUser, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>, new()
    #else
        where TUser : UserModel<TUser, TID>, new()
#endif
    {
        ClaimsIdentity identity = new(user.GetClaims(), authenticationType);
        return CreateToken( user, request, identity );
    }
    public Tokens<TID> CreateToken<TUser, TAddress, TGroupModel, TRoleModel, TID>( TUser user, ISessionID<TID> request, string authenticationType )
#if NET8_0_OR_GREATER
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
    #elif NET6_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
    #else
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    #endif
        where TUser : IUserData<TID, TAddress, TGroupModel, TRoleModel>
        where TGroupModel : IGroupModel<TID>
        where TRoleModel : IRoleModel<TID>
        where TAddress : IAddress<TID>
    {
        ClaimsIdentity identity = new(user.GetClaims(), authenticationType);
        return CreateToken( user, request, identity );
    }
    public Tokens<TID> CreateToken<TUser, TID>( in TUser user, in ISessionID<TID> request, in ClaimsIdentity identity )
#if NET8_0_OR_GREATER
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
    #elif NET6_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
    #else
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    #endif
        where TUser : IUserData<TID> => CreateToken( user, request.DeviceID, request.SessionID, identity );
    public Tokens<TID> CreateToken<TUser, TID>( in TUser user, in Guid deviceID, in TID sessionID, in ClaimsIdentity identity )
#if NET8_0_OR_GREATER
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
    #elif NET6_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
    #else
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    #endif
        where TUser : IUserData<TID>
    {
        DateTimeOffset now          = DateTimeOffset.UtcNow;
        DateTimeOffset notBefore    = now - TimeSpan.FromMinutes( 2 );
        string         accessToken  = CreateToken( user.GetExpires( now, TimeSpan.FromHours( 1 ) ), notBefore, identity );
        string         refreshToken = CreateToken( user.GetExpires( now, TimeSpan.FromDays( 90 ) ), notBefore, identity );
        return new Tokens<TID>( user.UserID, user.FullName, _version, accessToken, refreshToken, deviceID, sessionID );
    }
    public string CreateToken( in DateTimeOffset expires, in DateTimeOffset notBefore, in ClaimsIdentity identity ) =>
        _handler.CreateToken( new SecurityTokenDescriptor
                              {
                                  Audience           = _appName,
                                  Issuer             = _issuer,
                                  Subject            = identity,
                                  Expires            = expires.LocalDateTime,
                                  NotBefore          = notBefore.LocalDateTime,
                                  SigningCredentials = _credentials
                              } );


    public Task<TokenValidationResult> ValidateTokenAsync( string       jwt ) => _handler.ValidateTokenAsync( jwt, _parameters );
    public Task<TokenValidationResult> ValidateTokenAsync( JsonWebToken jwt ) => _handler.ValidateTokenAsync( jwt, _parameters );
}



public static class JwtParserExtensions
{
    public const string VALID_ISSUER   = "TokenValidationParameters:ValidIssuer";
    public const string VALID_AUDIENCE = "TokenValidationParameters:ValidAudience";
    public const string JWT            = "JWT";
    public const string JWT_KEY        = "JWT.key";


    private static async ValueTask<byte[]> GetJWTKey( this IConfiguration configuration, string jwt = JWT, string fileKey = JWT_KEY, CancellationToken token = default )
    {
        string? value = configuration.GetValue<string>( fileKey );

        if ( string.IsNullOrWhiteSpace( value ) is false )
        {
            LocalFile file = value;

            return file.Exists
                       ? await file.ReadAsync().AsBytes( token )
                       : throw new FileNotFoundException( file.FullPath );
        }

        value = configuration.GetValue<string>( jwt );

        return string.IsNullOrWhiteSpace( value ) is false
                   ? Encoding.UTF8.GetBytes( value )
                   : throw new KeyNotFoundException( $"Keys not found: [ {nameof(jwt)}: {jwt}, {nameof(fileKey)}: {fileKey} ]" );
    }


    public static async ValueTask<SigningCredentials>   GetSigningCredentials( this   IConfiguration configuration, string algorithm = SecurityAlgorithms.HmacSha512Signature, string jwt     = JWT, string fileKey = JWT_KEY ) => new(await configuration.GetSymmetricSecurityKey( jwt, fileKey ), algorithm);
    public static async ValueTask<SymmetricSecurityKey> GetSymmetricSecurityKey( this IConfiguration configuration, string jwt       = JWT,                                    string fileKey = JWT_KEY ) => new(await configuration.GetJWTKey( jwt, fileKey ));
    public static async ValueTask<TokenValidationParameters> GetTokenValidationParameters( this IWebAppSettings settings, string authenticationType, string? audience = null, string? issuer = null, string jwt = JWT, string fileKey = JWT_KEY, TimeSpan? clockSkew = null )
    {
        IConfiguration       configuration = settings.Configuration;
        SymmetricSecurityKey key           = await configuration.GetSymmetricSecurityKey( jwt, fileKey );
        return await configuration.GetTokenValidationParameters( authenticationType, key, audience, issuer, clockSkew );
    }
    public static async ValueTask<TokenValidationParameters> GetTokenValidationParameters( this IConfiguration configuration, string authenticationType, SymmetricSecurityKey? key = null, string? audience = null, string? issuer = null, TimeSpan? clockSkew = null )
    {
        key      ??= await configuration.GetSymmetricSecurityKey();
        issuer   ??= configuration[VALID_ISSUER]   ?? throw new KeyNotFoundException();
        audience ??= configuration[VALID_AUDIENCE] ?? throw new KeyNotFoundException();
        return key.GetTokenValidationParameters( authenticationType, issuer, audience, clockSkew );
    }


    public static TokenValidationParameters GetTokenValidationParameters( this SymmetricSecurityKey key, string authenticationType, string issuer, string audience, TimeSpan? clockSkew = null ) =>
        new()
        {
            ClockSkew                = clockSkew ?? TimeSpan.FromSeconds( 60 ),
            IssuerSigningKey         = key,
            RequireExpirationTime    = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidIssuer              = issuer,
            ValidAudience            = audience,
            AuthenticationType       = authenticationType
        };
}
