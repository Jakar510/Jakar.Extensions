// Jakar.Extensions :: Jakar.Database
// 10/10/2022  5:01 PM

namespace Jakar.Database;


public static class JwtExtensions
{
    [Pure] public static DateTimeOffset TokenExpiration( this IConfiguration configuration ) { return configuration.TokenExpiration( TimeSpan.FromMinutes( 30 ) ); }


    [Pure]
    public static DateTimeOffset TokenExpiration( this IConfiguration configuration, TimeSpan defaultValue )
    {
        TimeSpan offset = configuration.TokenValidation().GetValue( nameof(TokenExpiration), defaultValue );

        return DateTimeOffset.UtcNow + offset;
    }


    public static IConfigurationSection TokenValidation( this IConfiguration configuration ) { return configuration.GetSection( nameof(TokenValidation) ); }


    public static byte[]               GetJWTKey( this               IConfiguration configuration, DbOptions options ) { return Encoding.UTF8.GetBytes( configuration[options.JWTKey] ?? string.Empty ); }
    public static SymmetricSecurityKey GetSymmetricSecurityKey( this IConfiguration configuration, DbOptions options ) { return new SymmetricSecurityKey(configuration.GetJWTKey( options )); }
    public static SigningCredentials   GetSigningCredentials( this   IConfiguration configuration, DbOptions options ) { return new SigningCredentials(configuration.GetSymmetricSecurityKey( options ), options.JWTAlgorithm); }


    public static TokenValidationParameters GetTokenValidationParameters( this WebApplication        app,     DbOptions options ) { return app.Configuration.GetTokenValidationParameters( options ); }
    public static TokenValidationParameters GetTokenValidationParameters( this WebApplicationBuilder builder, DbOptions options ) { return builder.Configuration.GetTokenValidationParameters( options ); }
    public static TokenValidationParameters GetTokenValidationParameters( this IConfiguration configuration, DbOptions options )
    {
        IConfigurationSection section = configuration.TokenValidation();
        SymmetricSecurityKey  key     = configuration.GetSymmetricSecurityKey( options );
        return section.GetTokenValidationParameters( key, options );
    }
    public static TokenValidationParameters GetTokenValidationParameters( this IConfigurationSection section, SymmetricSecurityKey key, DbOptions options )
    {
        return new TokenValidationParameters
               {
                   IssuerSigningKey                          = key,
                   AuthenticationType                        = options.AuthenticationType,
                   IgnoreTrailingSlashWhenValidatingAudience = section.GetValue( nameof(TokenValidationParameters.IgnoreTrailingSlashWhenValidatingAudience), true ),
                   RequireAudience                           = section.GetValue( nameof(TokenValidationParameters.RequireAudience),                           true ),
                   RequireSignedTokens                       = section.GetValue( nameof(TokenValidationParameters.RequireSignedTokens),                       true ),
                   RequireExpirationTime                     = section.GetValue( nameof(TokenValidationParameters.RequireExpirationTime),                     true ),
                   ValidateLifetime                          = section.GetValue( nameof(TokenValidationParameters.ValidateLifetime),                          true ),
                   ValidateIssuerSigningKey                  = section.GetValue( nameof(TokenValidationParameters.ValidateIssuerSigningKey),                  true ),
                   ValidateIssuer                            = section.GetValue( nameof(TokenValidationParameters.ValidateIssuer),                            true ),
                   ValidateAudience                          = section.GetValue( nameof(TokenValidationParameters.ValidateAudience),                          true ),
                   ValidateActor                             = section.GetValue( nameof(TokenValidationParameters.ValidateActor),                             false ),
                   ValidateTokenReplay                       = section.GetValue( nameof(TokenValidationParameters.ValidateTokenReplay),                       false ),
                   ClockSkew                                 = section.GetValue( nameof(TokenValidationParameters.ClockSkew),                                 options.ClockSkew ),
                   ValidIssuer                               = section.GetValue( nameof(TokenValidationParameters.ValidIssuer),                               options.TokenIssuer ),
                   ValidAudience                             = section.GetValue( nameof(TokenValidationParameters.ValidAudience),                             options.TokenAudience )
               };
    }
}
