// Jakar.Extensions :: Jakar.AppLogger.Portal
// 08/21/2023  9:52 AM

namespace Jakar.AppLogger.Portal.Data;


public readonly record struct AppLoggerSecret( Guid UserID, Guid AppID )
{
    public static string Create( IDataProtectorProvider provider, UserRecord user, AppRecord app )
    {
        using IDataProtector protector = provider.GetProtector();
        return Create( protector, user, app );
    }
    public static async ValueTask<string> CreateAsync( IDataProtectorProvider provider, UserRecord user, AppRecord app )
    {
        using IDataProtector protector = await provider.GetProtectorAsync();
        return Create( protector, user, app );
    }
    public static string Create( IDataProtector protector, UserRecord user, AppRecord app )
    {
        string json = new AppLoggerSecret( user.ID.Value, app.ID.Value ).ToJson();
        return protector.Encrypt( json );
    }


    public static OneOf<AppLoggerSecret, Error> Parse( IDataProtectorProvider provider, string value )
    {
        using IDataProtector protector = provider.GetProtector();
        return Parse( protector, value );
    }
    public static async ValueTask<OneOf<AppLoggerSecret, Error>> ParseAsync( IDataProtectorProvider provider, string value )
    {
        using IDataProtector protector = await provider.GetProtectorAsync();
        return Parse( protector, value );
    }
    public static OneOf<AppLoggerSecret, Error> Parse( IDataProtector protector, string value )
    {
        try
        {
            string json = protector.Decrypt( value );
            return json.FromJson<AppLoggerSecret>();
        }
        catch ( Exception ) { return new Error( Status.Unauthorized ); }
    }
}
