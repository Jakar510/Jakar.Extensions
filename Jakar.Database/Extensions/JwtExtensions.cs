// Jakar.Extensions :: Jakar.Database
// 10/10/2022  5:01 PM

using Microsoft.AspNetCore.Authentication.JwtBearer;



namespace Jakar.Database;


public static class JwtExtensions
{
    public static byte[] GetJWTKey( this IConfiguration configuration ) => Encoding.UTF8.GetBytes( configuration["JWT"] );


    public static SigningCredentials GetSigningCredentials( this IConfiguration configuration ) => new(new SymmetricSecurityKey( configuration.GetJWTKey() ), SecurityAlgorithms.HmacSha256Signature);


    public static TokenValidationParameters GetTokenValidationParameters<T>( this IConfiguration configuration, string issuer ) where T : IAppName => configuration.GetTokenValidationParameters( issuer, typeof(T).Name );
    public static TokenValidationParameters GetTokenValidationParameters( this IConfiguration configuration, string issuer, string audience )
    {
        IConfigurationSection section = configuration.TokenValidation();

        return new TokenValidationParameters
               {
                   AuthenticationType                        = JwtBearerDefaults.AuthenticationScheme,
                   IssuerSigningKey                          = new SymmetricSecurityKey( section.GetJWTKey() ),
                   ClockSkew                                 = section.GetValue( nameof(TokenValidationParameters.ClockSkew),                                 TimeSpan.FromSeconds( 300 ) ),
                   IgnoreTrailingSlashWhenValidatingAudience = section.GetValue( nameof(TokenValidationParameters.IgnoreTrailingSlashWhenValidatingAudience), true ),
                   RequireAudience                           = section.GetValue( nameof(TokenValidationParameters.RequireAudience),                           true ),
                   RequireSignedTokens                       = section.GetValue( nameof(TokenValidationParameters.RequireSignedTokens),                       true ),
                   RequireExpirationTime                     = section.GetValue( nameof(TokenValidationParameters.RequireExpirationTime),                     true ),
                   ValidateActor                             = section.GetValue( nameof(TokenValidationParameters.ValidateActor),                             false ),
                   ValidateTokenReplay                       = section.GetValue( nameof(TokenValidationParameters.ValidateTokenReplay),                       false ),
                   ValidateLifetime                          = section.GetValue( nameof(TokenValidationParameters.ValidateLifetime),                          true ),
                   ValidateIssuerSigningKey                  = section.GetValue( nameof(TokenValidationParameters.ValidateIssuerSigningKey),                  true ),
                   ValidateIssuer                            = section.GetValue( nameof(TokenValidationParameters.ValidateIssuer),                            true ),
                   ValidIssuer                               = section.GetValue( nameof(TokenValidationParameters.ValidIssuer),                               issuer ),
                   ValidateAudience                          = section.GetValue( nameof(TokenValidationParameters.ValidateAudience),                          true ),
                   ValidAudience                             = section.GetValue( nameof(TokenValidationParameters.ValidAudience),                             audience )
               };
    }


    [Pure]
    public static DateTimeOffset TokenExpiration( this IConfiguration configuration )
    {
        TimeSpan offset = configuration.TokenValidation()
                                       .GetValue( nameof(TokenExpiration), TimeSpan.FromMinutes( 30 ) );

        return DateTimeOffset.UtcNow + offset;
    }
    public static IConfigurationSection TokenValidation( this IConfiguration configuration ) => configuration.GetSection( nameof(TokenValidation) );
}
