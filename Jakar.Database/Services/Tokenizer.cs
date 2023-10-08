// Jakar :: Jakar.Database
// 04/22/2022  11:10 AM

namespace Jakar.Database;


public interface ITokenService
{
    public ValueTask<string>  CreateContent( string       header, UserRecord user,  ClaimType         types, CancellationToken token = default );
    public ValueTask<string>  CreateHTMLContent( string   header, UserRecord user,  ClaimType         types, CancellationToken token = default );
    public ValueTask<Tokens?> Authenticate( VerifyRequest users,  ClaimType  types, CancellationToken token = default );
}



/// <summary>
///     <para>
///         <see href="https://codepedia.info/jwt-authentication-in-aspnet-core-web-api-token"/>
///     </para>
///     <para>
///         <see href="https://stackoverflow.com/a/55740879/9530917"> How do I get current user in .NET Core Web API (from JWT Token) </see>
///     </para>
/// </summary>
public class Tokenizer : ITokenService // TODO: update Tokenizer
{
    private readonly Database _db;
    internal virtual Uri      Domain => _db.Options.Domain;


    public Tokenizer( Database dataBase ) => _db = dataBase;

    public virtual string GetUrl( Tokens result ) => $"{Domain.OriginalString}/Token/{result.AccessToken}";

    public virtual async ValueTask<string> GenerateAccessToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        JwtSecurityToken security    = await _db.GetEmailJwtSecurityToken( claims, token );
        string           tokenString = new JwtSecurityTokenHandler().WriteToken( security );
        return tokenString;
    }
    public virtual async ValueTask<ClaimsPrincipal> GetPrincipalFromExpiredToken( string token, CancellationToken cancellationToken )
    {
        TokenValidationParameters tokenValidationParameters = await _db.GetTokenValidationParameters( cancellationToken );
        var                       tokenHandler              = new JwtSecurityTokenHandler();
        ClaimsPrincipal?          principal                 = tokenHandler.ValidateToken( token, tokenValidationParameters, out SecurityToken securityToken );

        return securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals( SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase )
                   ? throw new SecurityTokenException( "Invalid token" )
                   : principal;
    }


    public virtual string CreateContent( Tokens result, string header ) =>
        @$"{header}

{GetUrl( result )}";
    public virtual string CreateHTMLContent( Tokens result, string header ) =>
        @$"<h1> {header} </h1>
<p>
	<a href='{GetUrl( result )}'>Click to approve</a>
</p>";


    public virtual ValueTask<Tokens?> Authenticate( VerifyRequest request, ClaimType types, CancellationToken token = default ) => _db.Authenticate( request, types, token );


    public virtual async ValueTask<string> CreateContent( string header, UserRecord user, ClaimType types, CancellationToken token = default )
    {
        Tokens result = await _db.GetToken( user, types, token );
        return CreateContent( result, header );
    }
    public virtual async ValueTask<string> CreateHTMLContent( string header, UserRecord user, ClaimType types, CancellationToken token = default )
    {
        Tokens result = await _db.GetToken( user, types, token );
        return CreateHTMLContent( result, header );
    }
}
