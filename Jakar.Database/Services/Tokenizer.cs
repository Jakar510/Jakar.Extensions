// Jakar :: Jakar.Database
// 04/22/2022  11:10 AM

namespace Jakar.Database;


public interface ITokenService
{
    public ValueTask<string>          CreateContent( string       header, UserRecord user,  ClaimType         types, CancellationToken token = default );
    public ValueTask<string>          CreateHTMLContent( string   header, UserRecord user,  ClaimType         types, CancellationToken token = default );
    public ValueTask<ErrorOrResult<Tokens>> Authenticate( VerifyRequest users,  ClaimType  types, CancellationToken token = default );
}



/// <summary>
///     <para>
///         <see href="https://codepedia.info/jwt-authentication-in-aspnet-core-web-api-token"/>
///     </para>
///     <para>
///         <see href="https://stackoverflow.com/a/55740879/9530917"> How do I get current user in .NET Core Web API (from JWT Token) </see>
///     </para>
/// </summary>
public class Tokenizer( Database dataBase ) : ITokenService // TODO: update Tokenizer
{
    private readonly Database _dataBase = dataBase;
    internal virtual Uri      Domain => _dataBase.Settings.Domain;


    public virtual string GetUrl( in Tokens result ) => $"{Domain.OriginalString}/Token/{result.AccessToken}";


    public virtual async ValueTask<string> GenerateAccessToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        JwtSecurityToken security    = await _dataBase.GetJwtSecurityToken( claims, token );
        string           tokenString = new JwtSecurityTokenHandler().WriteToken( security );
        return tokenString;
    }


    public virtual string CreateContent( in Tokens result, in string header ) =>
        $"""
         {header}

         {GetUrl( result )}
         """;
    public virtual string CreateHTMLContent( in Tokens result, in string header ) =>
        $"""
         <h1> {header} </h1>
         <p>
         	<a href='{GetUrl( result )}'>Click to approve</a>
         </p>
         """;


    public virtual ValueTask<ErrorOrResult<Tokens>> Authenticate( VerifyRequest request, ClaimType types, CancellationToken token = default ) => _dataBase.Authenticate( request, types, token );


    public virtual async ValueTask<string> CreateContent( string header, UserRecord user, ClaimType types, CancellationToken token = default )
    {
        Tokens result = await _dataBase.GetToken( user, types, token );
        return CreateContent( result, header );
    }
    public virtual async ValueTask<string> CreateHTMLContent( string header, UserRecord user, ClaimType types, CancellationToken token = default )
    {
        Tokens result = await _dataBase.GetToken( user, types, token );
        return CreateHTMLContent( result, header );
    }
}
