// Jakar.Extensions :: Jakar.Extensions
// 4/5/2024  11:1

using System.Security.Claims;
using Jakar.Extensions.UserGuid;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;



namespace Jakar.Extensions;


public sealed class JwtParser( SigningCredentials credentials, TokenValidationParameters parameters, string appName, string issuer, AppVersion version )
{
    public const            string                                  SESSION       = "Session";
    public const            string                                  VERIFY_DEVICE = "VerifyDevice";
    public const            string                                  VERIFY_EMAIL  = "VerifyEmail";
    private static readonly ConcurrentDictionary<string, JwtParser> _parsers      = new(StringComparer.Ordinal);
    private readonly        AppVersion                              _version      = version;
    private readonly        JsonWebTokenHandler                     _handler      = new();
    private readonly        SigningCredentials                      _credentials  = credentials;
    private readonly        string                                  _appName      = appName;
    private readonly        string                                  _issuer       = issuer;
    private readonly        TokenValidationParameters               _parameters   = parameters;


    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    public static async ValueTask<JwtParser> GetOrCreateParser<TApp>( IWebAppSettings settings, string authenticationType )
        where TApp : IAppName
    {
        if ( _parsers.TryGetValue( authenticationType, out JwtParser? parser ) is false ) { _parsers[authenticationType] = parser = await CreateAsync<TApp>( settings, authenticationType ); }

        return parser;
    }


    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    public static async ValueTask<JwtParser> CreateAsync<TApp>( IWebAppSettings settings, string authenticationType )
        where TApp : IAppName
    {
        SigningCredentials        credentials = await settings.Configuration.GetSigningCredentials();
        TokenValidationParameters parameters  = await settings.GetTokenValidationParameters( authenticationType );

        return new JwtParser( credentials, parameters, settings.AppName, TApp.AppName, settings.Version );
    }


    public Tokens<Guid> CreateToken( UserModel          user, ISessionID<Guid> request, string authenticationType ) => CreateToken<UserModel, UserAddress, GroupModel, RoleModel, Guid>( user, request, authenticationType );
    public Tokens<long> CreateToken( UserLong.UserModel user, ISessionID<long> request, string authenticationType ) => CreateToken<UserLong.UserModel, UserLong.UserAddress, UserLong.GroupModel, UserLong.RoleModel, long>( user, request, authenticationType );
    public Tokens<TID> CreateToken<TUser, TID>( TUser user, ISessionID<TID> request, string authenticationType )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
        where TUser : UserModel<TUser, TID>, ICreateUserModel<TUser, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>, new()
    {
        ClaimsIdentity identity = new(user.GetClaims(), authenticationType);
        return CreateToken( user, request, identity );
    }
    public Tokens<TID> CreateToken<TUser, TAddress, TGroupModel, TRoleModel, TID>( TUser user, ISessionID<TID> request, string authenticationType )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
        where TUser : IUserData<TID, TAddress, TGroupModel, TRoleModel>
        where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
        where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
        where TAddress : IAddress<TID>, IEquatable<TAddress>
    {
        ClaimsIdentity identity = new(user.GetClaims(), authenticationType);
        return CreateToken( user, request, identity );
    }
    public Tokens<TID> CreateToken<TUser, TID>( in TUser user, in ISessionID<TID> request, in ClaimsIdentity identity )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
        where TUser : IUserData<TID> => CreateToken( user, request.DeviceID, request.SessionID, identity );
    public Tokens<TID> CreateToken<TUser, TID>( in TUser user, in Guid deviceID, in TID sessionID, in ClaimsIdentity identity )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
        where TUser : IUserData<TID>
    {
        DateTimeOffset now            = DateTimeOffset.UtcNow;
        DateTimeOffset notBefore      = now - TimeSpan.FromMinutes( 2 );
        DateTimeOffset expires        = user.GetExpires( now, TimeSpan.FromHours( 1 ) );
        DateTimeOffset refreshExpires = user.GetExpires( now, TimeSpan.FromDays( 90 ) );
        string         accessToken    = CreateToken( in expires,        in notBefore, identity );
        string         refreshToken   = CreateToken( in refreshExpires, in notBefore, identity );
        return new Tokens<TID>( user.UserID, user.FullName, _version, accessToken, refreshToken, deviceID, sessionID );
    }
    public string CreateToken( ref readonly DateTimeOffset expires, ref readonly DateTimeOffset notBefore, in ClaimsIdentity identity ) =>
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
    public const string JWT            = "JWT";
    public const string JWT_KEY        = "JWT.key";
    public const string VALID_AUDIENCE = "TokenValidationParameters:ValidAudience";
    public const string VALID_ISSUER   = "TokenValidationParameters:ValidIssuer";


    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    private static async ValueTask<byte[]> GetJWTKey( this IConfiguration configuration, string jwt = JWT, string fileKey = JWT_KEY, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan  = TelemetrySpan.Create();
        string?             value = configuration.GetValue<string>( fileKey );

        if ( string.IsNullOrWhiteSpace( value ) is false )
        {
            LocalFile file = value;

            return file.Exists
                       ? await file.ReadAsync().AsBytes(  token )
                       : throw new FileNotFoundException( file.FullPath );
        }

        value = configuration.GetValue<string>( jwt );

        return string.IsNullOrWhiteSpace( value ) is false
                   ? Encoding.UTF8.GetBytes( value )
                   : throw new KeyNotFoundException( $"Keys not found: [ {nameof(jwt)}: {jwt}, {nameof(fileKey)}: {fileKey} ]" );
    }


    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    public static async ValueTask<SigningCredentials> GetSigningCredentials( this IConfiguration configuration, string algorithm = SecurityAlgorithms.HmacSha512Signature, string jwt = JWT, string fileKey = JWT_KEY ) => new(await configuration.GetSymmetricSecurityKey( jwt, fileKey ), algorithm);

    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    public static async ValueTask<SymmetricSecurityKey> GetSymmetricSecurityKey( this IConfiguration configuration, string jwt = JWT, string fileKey = JWT_KEY ) => new(await configuration.GetJWTKey( jwt, fileKey ));

    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    public static async ValueTask<TokenValidationParameters> GetTokenValidationParameters( this IWebAppSettings settings, string authenticationType, string? audience = null, string? issuer = null, string jwt = JWT, string fileKey = JWT_KEY, TimeSpan? clockSkew = null )
    {
        IConfiguration       configuration = settings.Configuration;
        SymmetricSecurityKey key           = await configuration.GetSymmetricSecurityKey( jwt, fileKey );
        return await configuration.GetTokenValidationParameters( authenticationType, key, audience, issuer, clockSkew );
    }

    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
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
