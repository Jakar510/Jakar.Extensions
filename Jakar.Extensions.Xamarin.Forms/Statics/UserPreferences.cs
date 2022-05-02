using Xamarin.Essentials;



namespace Jakar.Extensions.Xamarin.Forms.Statics;


public static class UserPreferences
{
    // 
    public static string GetKey( this IAppSettings settings, object value,  string caller, string propertyName ) => settings.AppName.GetKey(value, caller, propertyName) ?? throw new NullReferenceException(nameof(IAppSettings.AppName));
    public static string GetKey( this IAppSettings settings, Type   type,   string caller, string propertyName ) => settings.AppName.GetKey(type,  caller, propertyName) ?? throw new NullReferenceException(nameof(IAppSettings.AppName));
    public static string GetKey( this string       appName,  object item,   string caller, string propertyName ) => appName.GetKey(item.GetType(), caller, propertyName);
    public static string GetKey( this string       appName,  Type   type,   string caller, string propertyName ) => $"{appName}.{type.GetKey(caller, propertyName)}";
    public static string GetKey( this object       item,     string caller, string propertyName ) => item.GetType().GetKey(caller, propertyName);
    public static string GetKey( this Type         type,     string caller, string propertyName ) => $"{type.FullName}.{caller}.{propertyName}";

    public static string GetKey( this object item, [CallerMemberName] string propertyName = "" ) => item.GetType().GetKey(propertyName);
    public static string GetKey( this Type   type, [CallerMemberName] string propertyName = "" ) => $"{type.FullName}.{propertyName}";


    public static string GetPassword( this string key ) => GetPasswordAsync(key).Result;

    public static async Task<string> GetPasswordAsync( this string key )
    {
        try { return await SecureStorage.GetAsync(key).ConfigureAwait(false); }
        catch
        {
            // Possible that device doesn't support secure storage on device.		
            return string.Empty;
        }
    }

    public static void SetPassword( this string key, string value ) => MainThread.InvokeOnMainThreadAsync(async () => await SetPasswordAsync(key, value)).Wait();

    public static async Task SetPasswordAsync( this string key, string value ) => await SecureStorage.SetAsync(key, value).ConfigureAwait(false);
}
