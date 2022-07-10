using System.Diagnostics;
using System.Globalization;
using System.Reflection;



namespace Jakar.AppLogger;


/// <summary>Device class to help retrieve device information.</summary>
public sealed class Device : IDevice
{
    public const string UNKNOWN = "Unknown";


    public string  SdkName        { get; init; } = string.Empty;
    public string  SdkVersion     { get; init; } = string.Empty;
    public string? Model          { get; init; }
    public string  OsName         { get; init; } = string.Empty;
    public string? OsVersion      { get; init; }
    public string? OsBuild        { get; init; }
    public int?    OsApiLevel     { get; set; }
    public string  Locale         { get; init; } = string.Empty;
    public double  TimeZoneOffset { get; init; }
    public string  AppVersion     { get; init; } = string.Empty;
    public string  AppBuild       { get; init; } = string.Empty;
    public string? AppNamespace   { get; init; }
    public string? CarrierName    { get; set; }
    public string? CarrierCountry { get; set; }


    public Device() { }

    /// <summary>
    /// Creates a public device model from an ingestion device model.
    /// </summary>
    /// <param name="device">The ingestion device model.</param>
    public Device( IDevice device )
    {
        SdkName        = device.SdkName;
        SdkVersion     = device.SdkVersion;
        Model          = device.Model;
        OsName         = device.OsName;
        OsVersion      = device.OsVersion;
        OsBuild        = device.OsBuild;
        OsApiLevel     = device.OsApiLevel;
        Locale         = device.Locale;
        TimeZoneOffset = device.TimeZoneOffset;
        AppVersion     = device.AppVersion;
        CarrierName    = device.CarrierName;
        CarrierCountry = device.CarrierCountry;
        AppBuild       = device.AppBuild;
        AppNamespace   = device.AppNamespace;
    }

    public static Device Create( in AppVersion version ) => Create(version.ToString(), version.Build?.ToString());
    public static Device Create( string appVersion, string? build = default )
    {
        // Microsoft.AppCenter.Device

        var device = new Device
                     {
                         SdkName        = RuntimeInformation.FrameworkDescription,
                         SdkVersion     = typeof(Device).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? RuntimeEnvironment.GetSystemVersion(),
                         Model          = GetDeviceModel(),
                         OsName         = Environment.OSVersion.VersionString,
                         OsVersion      = Environment.OSVersion.Version.ToString(),
                         OsBuild        = GetOsBuild(),
                         Locale         = CultureInfo.CurrentCulture.Name,
                         TimeZoneOffset = TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes,
                         AppVersion     = appVersion,
                         AppBuild       = build ?? GetFileVersion(),
                         AppNamespace   = Assembly.GetEntryAssembly()?.EntryPoint?.DeclaringType?.Namespace
                     };

        return device;
    }

    private static string GetFileVersion()
    {
        string? fileName = Assembly.GetEntryAssembly()?.Location;
        if ( string.IsNullOrWhiteSpace(fileName) ) { fileName = Environment.GetCommandLineArgs()[0]; }

        return FileVersionInfo.GetVersionInfo(fileName).FileVersion ?? UNKNOWN;
    }


    // private static string? GetSdkVersion()
    // {
    //     WpfHelper.IsRunningOnWpf
    //         ? "appcenter.wpf"
    //         : "appcenter.winforms";
    // }
    private static string? GetDeviceModel()
    {
        if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) { }

        return default;
    }

    private static string? GetOsBuild()
    {
    #if TARGET_BROWSER
            return platform.Equals("BROWSER", StringComparison.OrdinalIgnoreCase);

    #elif TARGET_WINDOWS
        using Microsoft.Win32.RegistryKey  localMachine = Microsoft.Win32.Registry.LocalMachine;
        using Microsoft.Win32.RegistryKey? registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
        if ( registryKey is null ) { return default; }
        
        object? obj1 = registryKey.GetValue("CurrentMajorVersionNumber");
        
        if ( obj1 is not null )
        {
            object? obj2 = registryKey.GetValue("CurrentMinorVersionNumber", "0");
            object? obj3 = registryKey.GetValue("CurrentBuildNumber",        "0");
            object? obj4 = registryKey.GetValue("UBR",                       "0");
            return $"{obj1}.{obj2}.{obj3}.{obj4}";
        }
        
        object?   obj5 = registryKey.GetValue("CurrentVersion", "0.0");
        object?   obj6 = registryKey.GetValue("CurrentBuild",   "0");
        string[]? strArray = registryKey.GetValue("BuildLabEx")?.ToString()?.Split('.');
        
        string str = strArray is null || strArray.Length < 2
                         ? "0"
                         : strArray[1];
        
        return $"{obj5}.{obj6}.{str}";

    #elif TARGET_OSX
            return platform.Equals("OSX", StringComparison.OrdinalIgnoreCase) || platform.Equals("MACOS", StringComparison.OrdinalIgnoreCase);

    #elif TARGET_MACCATALYST
            return platform.Equals("MACCATALYST", StringComparison.OrdinalIgnoreCase) || platform.Equals("IOS", StringComparison.OrdinalIgnoreCase);

    #elif TARGET_IOS
            return platform.Equals("IOS", StringComparison.OrdinalIgnoreCase);

    #elif TARGET_ANDROID
            return platform.Equals("IOS", StringComparison.OrdinalIgnoreCase);

    #elif TARGET_UNIX
            return platform.Equals(s_osPlatformName, StringComparison.OrdinalIgnoreCase);

    #else
        return default;
    #endif
    }
}
