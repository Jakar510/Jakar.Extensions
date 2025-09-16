// Jakar.Extensions :: Jakar.Database
// 1/6/2024  18:35

namespace Jakar.Database;


public class DbTokenHandler : JsonWebTokenHandler
{
    private readonly JwtSecurityTokenHandler __handler = new();
    public static    DbTokenHandler          Instance { get; set; } = new();


    public virtual ClaimsPrincipal? ValidateToken( string jsonToken, TokenValidationParameters parameters, out SecurityToken? securityToken ) => __handler.ValidateToken( jsonToken, parameters, out securityToken );


    public override async Task<TokenValidationResult> ValidateTokenAsync( string token, TokenValidationParameters validationParameters )
    {
        TokenValidationResult result = await base.ValidateTokenAsync( token, validationParameters );
        return result;
    }
    public override async Task<TokenValidationResult> ValidateTokenAsync( SecurityToken token, TokenValidationParameters validationParameters )
    {
        TokenValidationResult result = await base.ValidateTokenAsync( token, validationParameters );
        return result;
    }
}
