// Jakar.Extensions :: Jakar.Extensions
// 11/16/2026

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;



namespace Jakar.Extensions;


/// <summary> The <see cref="IWebAppSettings"/> overloads of Jakar.Users' <see cref="JwtParser"/> (Jakar.Users can't see <see cref="IWebAppSettings"/>). </summary>
public static class WebAppSettingsJwt
{
    extension( JwtParser )
    {
        public static ValueTask<JwtParser> GetOrCreateParser<TApp>( IWebAppSettings settings, string authenticationType )
            where TApp : IAppName => JwtParser.GetOrCreateParser<TApp>(settings.Configuration, settings.AppName, settings.Version, authenticationType);

        public static ValueTask<JwtParser> CreateAsync<TApp>( IWebAppSettings settings, string authenticationType )
            where TApp : IAppName => JwtParser.CreateAsync<TApp>(settings.Configuration, settings.AppName, settings.Version, authenticationType);
    }



    extension( IWebAppSettings settings )
    {
        public async ValueTask<TokenValidationParameters> GetTokenValidationParameters( string authenticationType, string? audience = null, string? issuer = null, string jwt = JWT, string fileKey = JWT_KEY, TimeSpan? clockSkew = null )
        {
            IConfiguration       configuration = settings.Configuration;
            SymmetricSecurityKey key           = await configuration.GetSymmetricSecurityKey(jwt, fileKey).ConfigureAwait(false);

            return await configuration.GetTokenValidationParameters(authenticationType, key, audience, issuer, clockSkew).ConfigureAwait(false);
        }
    }
}