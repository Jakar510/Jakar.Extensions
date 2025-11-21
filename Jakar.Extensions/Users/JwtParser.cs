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
    private static readonly ConcurrentDictionary<string, JwtParser> __parsers     = new(StringComparer.Ordinal);
    private readonly        AppVersion                              __version     = version;
    private readonly        JsonWebTokenHandler                     __handler     = new();
    private readonly        SigningCredentials                      __credentials = credentials;
    private readonly        string                                  __appName     = appName;
    private readonly        string                                  __issuer      = issuer;
    private readonly        TokenValidationParameters               __parameters  = parameters;


    public static async ValueTask<JwtParser> GetOrCreateParser<TApp>( IWebAppSettings settings, string authenticationType )
        where TApp : IAppName
    {
        if ( !__parsers.TryGetValue(authenticationType, out JwtParser? parser) ) { __parsers[authenticationType] = parser = await CreateAsync<TApp>(settings, authenticationType).ConfigureAwait(false); }

        return parser;
    }


    public static async ValueTask<JwtParser> CreateAsync<TApp>( IWebAppSettings settings, string authenticationType )
        where TApp : IAppName
    {
        SigningCredentials        credentials = await settings.Configuration.GetSigningCredentials().ConfigureAwait(false);
        TokenValidationParameters parameters  = await settings.GetTokenValidationParameters(authenticationType).ConfigureAwait(false);

        return new JwtParser(credentials, parameters, settings.AppName, TApp.AppName, settings.Version);
    }


    public SessionToken CreateToken<TRequest>( UserModel user, TRequest request, string authenticationType )
        where TRequest : ISessionID<Guid> => CreateToken<TRequest, UserModel, Guid>(user, request, authenticationType);
    public SessionToken CreateToken<TRequest>( UserLong.UserModel user, TRequest request, string authenticationType )
        where TRequest : ISessionID<long> => CreateToken<TRequest, UserLong.UserModel, long>(user, request, authenticationType);


    public SessionToken CreateToken<TRequest, TUser, TID>( TUser user, TRequest request, string authenticationType )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
        where TRequest : ISessionID<TID>
        where TUser : IUserData
    {
        ClaimsIdentity identity = new(user.GetClaims(), authenticationType);

        return request switch
               {
                   ISessionID<long> idLong => CreateToken(in user, in idLong,       in identity),
                   ISessionID<Guid> idGuid => CreateToken(in user, idGuid.DeviceID, idGuid.SessionID, in identity),
                   _                       => throw new ExpectedValueTypeException(request, typeof(ISessionID<Guid>), typeof(ISessionID<long>))
               };
    }


    public SessionToken CreateToken<TRequest, TUser>( in TUser user, in TRequest request, in ClaimsIdentity identity )
        where TRequest : ISessionID<long>
        where TUser : IUserData => CreateToken(user, request.DeviceID, request.SessionID.AsGuid(), identity);
    public SessionToken CreateToken<TUser>( in TUser user, in string deviceID, in Guid sessionID, in ClaimsIdentity identity )
        where TUser : IUserData
    {
        DateTimeOffset now            = DateTimeOffset.UtcNow;
        DateTimeOffset notBefore      = now - TimeSpan.FromMinutes(2);
        DateTimeOffset expires        = user.GetExpires(now, TimeSpan.FromHours(1));
        DateTimeOffset refreshExpires = user.GetExpires(now, TimeSpan.FromDays(90));
        string         accessToken    = CreateToken(in expires,        in notBefore, identity);
        string         refreshToken   = CreateToken(in refreshExpires, in notBefore, identity);

        return new SessionToken
               {
                   UserID       = user.UserID,
                   AccessToken  = accessToken,
                   RefreshToken = refreshToken,
                   DeviceID     = deviceID,
                   SessionID    = sessionID,
                   FullName     = user.FullName,
                   Version      = __version
               };
    }
    public string CreateToken( ref readonly DateTimeOffset expires, ref readonly DateTimeOffset notBefore, in ClaimsIdentity identity ) =>
        __handler.CreateToken(new SecurityTokenDescriptor
                              {
                                  Audience           = __appName,
                                  Issuer             = __issuer,
                                  Subject            = identity,
                                  Expires            = expires.LocalDateTime,
                                  NotBefore          = notBefore.LocalDateTime,
                                  SigningCredentials = __credentials
                              });


    public Task<TokenValidationResult> ValidateTokenAsync( string       jwt ) => __handler.ValidateTokenAsync(jwt, __parameters);
    public Task<TokenValidationResult> ValidateTokenAsync( JsonWebToken jwt ) => __handler.ValidateTokenAsync(jwt, __parameters);
}



public static class JwtParserExtensions
{
    extension( IConfiguration configuration )
    {
        private async ValueTask<byte[]> GetJWTKey( string jwt = JWT, string fileKey = JWT_KEY, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            string?             value         = configuration[fileKey];

            if ( !string.IsNullOrWhiteSpace(value) )
            {
                LocalFile file = value;

                return file.Exists
                           ? await file.ReadAsync()
                                       .AsBytes(token).ConfigureAwait(false)
                           : throw new FileNotFoundException(file.FullPath);
            }

            value = configuration[jwt];

            return !string.IsNullOrWhiteSpace(value)
                       ? Encoding.UTF8.GetBytes(value)
                       : throw new KeyNotFoundException($"Keys not found: [ {nameof(jwt)}: {jwt}, {nameof(fileKey)}: {fileKey} ]");
        }
        public async ValueTask<SigningCredentials>   GetSigningCredentials( string   algorithm = SecurityAlgorithms.HmacSha512Signature, string jwt     = JWT, string fileKey = JWT_KEY ) => new(await configuration.GetSymmetricSecurityKey(jwt, fileKey).ConfigureAwait(false), algorithm);
        public async ValueTask<SymmetricSecurityKey> GetSymmetricSecurityKey( string jwt       = JWT,                                    string fileKey = JWT_KEY ) => new(await configuration.GetJWTKey(jwt, fileKey).ConfigureAwait(false));
    }



    public static async ValueTask<TokenValidationParameters> GetTokenValidationParameters( this IWebAppSettings settings, string authenticationType, string? audience = null, string? issuer = null, string jwt = JWT, string fileKey = JWT_KEY, TimeSpan? clockSkew = null )
    {
        IConfiguration       configuration = settings.Configuration;
        SymmetricSecurityKey key           = await configuration.GetSymmetricSecurityKey(jwt, fileKey).ConfigureAwait(false);
        return await configuration.GetTokenValidationParameters(authenticationType, key, audience, issuer, clockSkew).ConfigureAwait(false);
    }

    public static async ValueTask<TokenValidationParameters> GetTokenValidationParameters( this IConfiguration configuration, string authenticationType, SymmetricSecurityKey? key = null, string? audience = null, string? issuer = null, TimeSpan? clockSkew = null )
    {
        key      ??= await configuration.GetSymmetricSecurityKey().ConfigureAwait(false);
        issuer   ??= configuration[VALID_ISSUER]   ?? throw new KeyNotFoundException();
        audience ??= configuration[VALID_AUDIENCE] ?? throw new KeyNotFoundException();
        return key.GetTokenValidationParameters(authenticationType, issuer, audience, clockSkew);
    }


    public static TokenValidationParameters GetTokenValidationParameters( this SymmetricSecurityKey key, string authenticationType, string issuer, string audience, TimeSpan? clockSkew = null ) =>
        new()
        {
            ClockSkew                = clockSkew ?? TimeSpan.FromSeconds(60),
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
