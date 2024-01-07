﻿// Jakar.Extensions :: Jakar.Database
// 1/6/2024  18:35

namespace Jakar.Database;


public sealed class DbTokenHandler : JsonWebTokenHandler
{
    public static DbTokenHandler Instance { get; } = new();
    public DbTokenHandler() { }


    public static ClaimsPrincipal? ValidateToken( string jsonToken, TokenValidationParameters parameters, out SecurityToken? securityToken )
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ValidateToken( jsonToken, parameters, out securityToken );
    }


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
