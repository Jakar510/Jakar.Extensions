#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public static class AppDeviceInfo
{
    public static string BuildNumber    => AppInfo.BuildString;
    public static string DeviceId       => $"{Manufacturer}  {DeviceModel}: {DevicePlatform} {DeviceVersion} | {VersionNumber} [{BuildNumber}]";
    public static string DeviceModel    => DeviceInfo.Model;
    public static string DevicePlatform => DeviceInfo.Platform.ToString();
    public static string DeviceVersion  => DeviceInfo.VersionString;
    public static string FullVersion    => $"{VersionNumber}.{BuildNumber}";
    public static string Manufacturer   => DeviceInfo.Manufacturer;
    public static string PackageName    => AppInfo.PackageName;
    public static string VersionNumber  => AppInfo.VersionString;
}
