using Microsoft.Win32;



namespace Jakar.AppLogger.Common;


/// <summary> Device class to help retrieve device information. </summary>
public sealed record DeviceDescriptor : BaseRecord, IDevice
{
    public const string     UNKNOWN = "Unknown";
    public       AppVersion AppVersion     { get; init; } = new();
    public       double     TimeZoneOffset { get; init; }
    public       HwInfo?    HwInfo         { get; set; }
    public       int?       AppBuild       { get; init; }
    public       int?       OsApiLevel     { get; set; }
    public       string     DeviceID       { get; init; } = string.Empty;
    public       string     Locale         { get; init; } = string.Empty;
    public       string     OsName         { get; init; } = string.Empty;
    public       string     SdkName        { get; init; } = string.Empty;
    public       string     SdkVersion     { get; init; } = string.Empty;
    public       string?    AppNamespace   { get; init; }
    public       string?    Model          { get; init; }
    public       string?    OsBuild        { get; init; }
    public       string?    OsVersion      { get; init; }
    public       PlatformID Platform       { get; init; }


    public DeviceDescriptor() { }
    public DeviceDescriptor( IDevice device )
    {
        DeviceID       = device.DeviceID;
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
        AppBuild       = device.AppBuild;
        AppNamespace   = device.AppNamespace;
        HwInfo         = device.HwInfo;
    }


    public static DeviceDescriptor Create( AppVersion version, string deviceID )
    {
        // #if __IOS__
        //     deviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        // #elif __ANDROID__
        //         deviceID = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
        // #else
        //         deviceID = info.DeviceID();
        // #endif


        OperatingSystem osVersion = Environment.OSVersion;

        var device = new DeviceDescriptor
                     {
                         // DeviceID       = HardwareInfo.DeviceID,
                         SdkName = RuntimeInformation.FrameworkDescription,
                         SdkVersion = typeof(DeviceDescriptor).GetTypeInfo()
                                                              .Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                             ?.InformationalVersion ?? RuntimeEnvironment.GetSystemVersion(),
                         Model          = GetDeviceModel(),
                         OsName         = osVersion.VersionString,
                         OsVersion      = osVersion.Version.ToString(),
                         Platform       = osVersion.Platform,
                         OsBuild        = GetOsBuild(),
                         Locale         = CultureInfo.CurrentCulture.Name,
                         TimeZoneOffset = TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes,
                         AppVersion     = version,
                         AppBuild = version.Build ?? GetFileVersion()
                                       .Build,
                         AppNamespace = Assembly.GetEntryAssembly()
                                               ?.EntryPoint?.DeclaringType?.Namespace,
                         DeviceID = deviceID
                     };

        return device;
    }


    public static AppVersion GetFileVersion()
    {
        string? fileName = Assembly.GetEntryAssembly()
                                  ?.Location;

        if ( string.IsNullOrWhiteSpace( fileName ) ) { fileName = Environment.GetCommandLineArgs()[0]; }

        return FileVersionInfo.GetVersionInfo( fileName )
                              .FileVersion ?? UNKNOWN;
    }


    // public static string? GetSdkVersion()
    // {
    //     WpfHelper.IsRunningOnWpf
    //         ? "appcenter.wpf"
    //         : "appcenter.winforms";
    // }
    public static string? GetDeviceModel()
    {
        if ( RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) ) { }

        return default;
    }


    public static string? GetOsBuild()
    {
    #if TARGET_BROWSER
            return platform.Equals("BROWSER", StringComparison.OrdinalIgnoreCase);

    #elif __WINDOWS__
        using RegistryKey  localMachine = Registry.LocalMachine;
        using RegistryKey? registryKey  = localMachine.OpenSubKey( "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion" );
        if ( registryKey is null ) { return default; }

        object? obj1 = registryKey.GetValue( "CurrentMajorVersionNumber" );

        if ( obj1 is not null )
        {
            object? obj2 = registryKey.GetValue( "CurrentMinorVersionNumber", "0" );
            object? obj3 = registryKey.GetValue( "CurrentBuildNumber",        "0" );
            object? obj4 = registryKey.GetValue( "UBR",                       "0" );
            return $"{obj1}.{obj2}.{obj3}.{obj4}";
        }

        object? obj5 = registryKey.GetValue( "CurrentVersion", "0.0" );
        object? obj6 = registryKey.GetValue( "CurrentBuild",   "0" );

        string[]? strArray = registryKey.GetValue( "BuildLabEx" )
                                       ?.ToString()
                                       ?.Split( '.' );

        string str = strArray is null || strArray.Length < 2
                         ? "0"
                         : strArray[1];

        return $"{obj5}.{obj6}.{str}";

    #elif __MACOS__
            return platform.Equals("OSX", StringComparison.OrdinalIgnoreCase) || platform.Equals("MACOS", StringComparison.OrdinalIgnoreCase);

    #elif TARGET_MACCATALYST
            return platform.Equals("MACCATALYST", StringComparison.OrdinalIgnoreCase) || platform.Equals("IOS", StringComparison.OrdinalIgnoreCase);

    #elif __IOS__
            return platform.Equals("IOS", StringComparison.OrdinalIgnoreCase);

    #elif __ANDROID__
            return platform.Equals("IOS", StringComparison.OrdinalIgnoreCase);

    #elif __LINUX__
            return platform.Equals(s_osPlatformName, StringComparison.OrdinalIgnoreCase);

    #else
        return default;
    #endif
    }
}
