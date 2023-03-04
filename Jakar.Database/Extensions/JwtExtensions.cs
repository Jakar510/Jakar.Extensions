// Jakar.Extensions :: Jakar.Database
// 10/10/2022  5:01 PM

using Microsoft.AspNetCore.Authentication.JwtBearer;



namespace Jakar.Database;


public static class JwtExtensions
{
    public const string JWT = nameof(JWT);


    public static byte[] GetJWTKey( this IConfiguration configuration ) => Encoding.UTF8.GetBytes( configuration[JWT] ?? string.Empty );


    [Pure]
    public static DateTimeOffset TokenExpiration( this IConfiguration configuration )
    {
        TimeSpan offset = configuration.TokenValidation()
                                       .GetValue( nameof(TokenExpiration), TimeSpan.FromMinutes( 30 ) );

        return DateTimeOffset.UtcNow + offset;
    }
    public static IConfigurationSection TokenValidation( this IConfiguration configuration ) => configuration.GetSection( nameof(TokenValidation) );


    public static SigningCredentials GetSigningCredentials( this IConfiguration configuration ) => new(new SymmetricSecurityKey( configuration.GetJWTKey() ), SecurityAlgorithms.HmacSha256Signature);


    public static TokenValidationParameters GetTokenValidationParameters( this IConfiguration configuration, DbOptions options )
    {
        IConfigurationSection section = configuration.TokenValidation();
        var                   key     = new SymmetricSecurityKey( section.GetJWTKey() );
        return section.GetTokenValidationParameters( key, options );
    }


    public static TokenValidationParameters GetTokenValidationParameters( this IConfigurationSection section, SymmetricSecurityKey key, DbOptions options )
    {
        return new TokenValidationParameters
               {
                   AuthenticationType                        = JwtBearerDefaults.AuthenticationScheme,
                   IssuerSigningKey                          = key,
                   ClockSkew                                 = section.GetValue( nameof(TokenValidationParameters.ClockSkew),                                 TimeSpan.FromSeconds( 60 ) ),
                   IgnoreTrailingSlashWhenValidatingAudience = section.GetValue( nameof(TokenValidationParameters.IgnoreTrailingSlashWhenValidatingAudience), true ),
                   RequireAudience                           = section.GetValue( nameof(TokenValidationParameters.RequireAudience),                           true ),
                   RequireSignedTokens                       = section.GetValue( nameof(TokenValidationParameters.RequireSignedTokens),                       true ),
                   RequireExpirationTime                     = section.GetValue( nameof(TokenValidationParameters.RequireExpirationTime),                     true ),
                   ValidateActor                             = section.GetValue( nameof(TokenValidationParameters.ValidateActor),                             false ),
                   ValidateTokenReplay                       = section.GetValue( nameof(TokenValidationParameters.ValidateTokenReplay),                       false ),
                   ValidateLifetime                          = section.GetValue( nameof(TokenValidationParameters.ValidateLifetime),                          true ),
                   ValidateIssuerSigningKey                  = section.GetValue( nameof(TokenValidationParameters.ValidateIssuerSigningKey),                  true ),
                   ValidateIssuer                            = section.GetValue( nameof(TokenValidationParameters.ValidateIssuer),                            true ),
                   ValidIssuer                               = section.GetValue( nameof(TokenValidationParameters.ValidIssuer),                               options.TokenIssuer ),
                   ValidateAudience                          = section.GetValue( nameof(TokenValidationParameters.ValidateAudience),                          true ),
                   ValidAudience                             = section.GetValue( nameof(TokenValidationParameters.ValidAudience),                             options.TokenAudience ),
               };
    }
}
