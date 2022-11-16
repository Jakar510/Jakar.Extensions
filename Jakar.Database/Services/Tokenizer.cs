// TrueLogic :: TrueKeep.Cloud
// 04/22/2022  11:10 AM

using System.IdentityModel.Tokens.Jwt;



namespace Jakar.Database;


public interface ITokenService
{
    public Task<Tokens> Authenticate( VerifyRequest users,  CancellationToken token                         = default );
    public Task<string> CreateContent( string       header, UserRecord        user, CancellationToken token = default );
    public Task<string> CreateHTMLContent( string   header, UserRecord        user, CancellationToken token = default );
}



/// <summary>
///     <para>
///         <see href = "https://codepedia.info/jwt-authentication-in-aspnet-core-web-api-token" />
///     </para>
///     <para>
///         <see href = "https://stackoverflow.com/a/55740879/9530917" > How do I get current user in .NET Core Web API (from JWT Token) </see>
///     </para>
/// </summary>
public class Tokenizer<TName> : ITokenService where TName : IAppName
{
    private readonly Database             _db;
    private readonly IConfiguration       _configuration;
    internal virtual string               Audience    => _configuration.GetValue( nameof(Audience), typeof(TName).Name );
    internal virtual string               Domain      => _configuration.GetValue( nameof(Domain),   _db.Domain );
    internal virtual string               Issuer      => _configuration.GetValue( nameof(Issuer),   typeof(TName).Namespace ?? string.Empty );
    internal virtual SymmetricSecurityKey SecurityKey => new(Encoding.UTF8.GetBytes( _configuration["JWT"] ));


    public Tokenizer( IConfiguration configuration, Database dataBase )
    {
        _configuration = configuration;
        _db            = dataBase;
    }
    public virtual string CreateContent( in Tokens result, string header ) =>
        @$"{header}

{GetUrl( result )}";
    public virtual string CreateHTMLContent( in Tokens result, string header ) =>
        @$"<h1> {header} </h1>
<p>
	<a href='{GetUrl( result )}'>Click to approve</a>
</p>";


    public virtual string GenerateAccessToken( IEnumerable<Claim> claims )
    {
        string  name              = typeof(TName).Name;
        var     signinCredentials = new SigningCredentials( SecurityKey, SecurityAlgorithms.HmacSha256 );
        var     tokeOptions       = new JwtSecurityToken( name, name, claims, DateTime.Now, DateTime.Now.AddMinutes( 15 ), signinCredentials );
        string? tokenString       = new JwtSecurityTokenHandler().WriteToken( tokeOptions );
        return tokenString;
    }
    public virtual ClaimsPrincipal GetPrincipalFromExpiredToken( string token )
    {
        TokenValidationParameters tokenValidationParameters = _configuration.GetTokenValidationParameters( Issuer, Audience );
        var                       tokenHandler              = new JwtSecurityTokenHandler();
        ClaimsPrincipal?          principal                 = tokenHandler.ValidateToken( token, tokenValidationParameters, out SecurityToken securityToken );

        return securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals( SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase )
                   ? throw new SecurityTokenException( "Invalid token" )
                   : principal;
    }
    public virtual string GetUrl( in Tokens result ) => $"{Domain}/Token/{result.AccessToken}";


    public virtual async Task<Tokens> Authenticate( VerifyRequest request, CancellationToken token = default ) => await _db.Authenticate( request, token );


    public virtual async Task<string> CreateContent( string header, UserRecord user, CancellationToken token = default )
    {
        Tokens result = await _db.GetJwtToken( user, token );
        return CreateContent( result, header );
    }
    public virtual async Task<string> CreateHTMLContent( string header, UserRecord user, CancellationToken token = default )
    {
        Tokens result = await _db.GetJwtToken( user, token );
        return CreateHTMLContent( result, header );
    }
}
