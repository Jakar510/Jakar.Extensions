// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:17 PM

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;



namespace Jakar.Database;


/// <summary> The SecurityToken created by JwtSecurityTokenHandler.CreateToken </summary>
[Serializable]
[SuppressMessage( "ReSharper", "InconsistentNaming" )]
[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
public readonly struct Tokens : IValidator
{
    public AppVersion Version      { get; init; } = default;
    public Guid       UserID       { get; init; } = Guid.Empty;
    public string     AccessToken  { get; init; } = string.Empty;
    public string?    RefreshToken { get; init; } = default;
    public string?    FullName     { get; init; } = default;
    public bool       IsValid      => !string.IsNullOrWhiteSpace( AccessToken );


    [JsonConstructor]
    public Tokens( string accessToken, string? refreshToken, AppVersion version, Guid userID, string? fullName )
    {
        Version      = version;
        AccessToken  = accessToken;
        RefreshToken = refreshToken;
        UserID       = userID;
        FullName     = fullName;
    }


    public static ValueTask<Tokens> CreateAsync( Database db, UserRecord user, CancellationToken token                            = default ) => db.TryCall( CreateAsync, db, user, db.Configuration.TokenExpireTime(), token );
    public static ValueTask<Tokens> CreateAsync( Database db, UserRecord user, DateTimeOffset    expires, CancellationToken token = default ) => db.TryCall( CreateAsync, db, user, expires,                            token );
    public static async ValueTask<Tokens> CreateAsync( DbConnection connection, DbTransaction transaction, Database db, UserRecord user, DateTimeOffset expires, CancellationToken token = default )
    {
        List<Claim> claims = await user.GetUserClaims( connection, transaction, db, token );
        Tokens      result = Create( db.Configuration, claims, user, db.Version, expires );
        await db.Users.Update( connection, transaction, user, token );
        return result;
    }


    public static Tokens Create( IConfiguration configuration, IEnumerable<Claim> claims, in AppVersion version, UserRecord user ) =>
        Create( configuration, claims, user, version, configuration.TokenExpireTime() );
    public static Tokens Create( IConfiguration configuration, IEnumerable<Claim> claims, UserRecord user, in AppVersion version, DateTimeOffset expires )
    {
        if (user.SubscriptionExpires < expires) { expires = user.SubscriptionExpires.Value; }

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
                                             {
                                                 Subject            = new ClaimsIdentity( claims ),
                                                 Expires            = expires.LocalDateTime,
                                                 SigningCredentials = configuration.GetSigningCredentials()
                                             };

        string token   = handler.WriteToken( handler.CreateToken( descriptor ) );
        string refresh = handler.WriteToken( handler.CreateToken( descriptor ) );

        user.SetRefreshToken( refresh, expires );
        return Create( token, refresh, version, user );
    }
    public static Tokens Create( string accessToken, string? refreshToken, AppVersion version, UserRecord user ) => new(accessToken, refreshToken, version, user.UserID, user.FullName);


    public bool VerifyVersion( in AppVersion version ) => Version.FuzzyEquals( version );
}
