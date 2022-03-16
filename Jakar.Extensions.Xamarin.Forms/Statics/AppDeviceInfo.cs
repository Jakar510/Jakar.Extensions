


namespace Jakar.Extensions.Xamarin.Forms.Statics;


public static class AppDeviceInfo
{
    public static string VersionNumber  => global::Xamarin.Essentials.AppInfo.VersionString;
    public static string BuildNumber    => global::Xamarin.Essentials.AppInfo.BuildString;
    public static string PackageName    => global::Xamarin.Essentials.AppInfo.PackageName;
    public static string DeviceModel    => global::Xamarin.Essentials.DeviceInfo.Model;
    public static string Manufacturer   => global::Xamarin.Essentials.DeviceInfo.Manufacturer;
    public static string DeviceVersion  => global::Xamarin.Essentials.DeviceInfo.VersionString;
    public static string DevicePlatform => global::Xamarin.Essentials.DeviceInfo.Platform.ToString();

    public static string FullVersion => $"{VersionNumber}.{BuildNumber}";
    public static string DeviceId    => $"{Manufacturer}  {DeviceModel}: {DevicePlatform} {DeviceVersion} | {VersionNumber} [{BuildNumber}]";
}
