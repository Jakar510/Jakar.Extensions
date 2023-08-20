using CliWrap;
using Microsoft.Win32;



namespace Jakar.AppLogger.Common;


/// <summary> Device class to help retrieve device information. </summary>
public sealed record DeviceDescriptor( int?         AppBuild,
                                       string?      AppNamespace,
                                       AppVersion   AppVersion,
                                       string       OsName,
                                       int?         OsApiLevel,
                                       AppVersion   OsVersion,
                                       string       SdkName,
                                       string       SdkVersion,
                                       string       Locale,
                                       string       DeviceID,
                                       string?      Model,
                                       PlatformID   Platform,
                                       Architecture ProcessArchitecture,
                                       TimeSpan     TimeZoneOffset,
                                       HwInfo?      HwInfo = default
) : BaseRecord, IDevice
{
    public const string  UNKNOWN = "Unknown";
    public       HwInfo? HwInfo { get; set; } = HwInfo;


    public DeviceDescriptor( IDevice device ) : this( device.AppBuild,
                                                      device.AppNamespace,
                                                      device.AppVersion,
                                                      device.OsName,
                                                      device.OsApiLevel,
                                                      device.OsVersion,
                                                      device.SdkName,
                                                      device.SdkVersion,
                                                      device.Locale,
                                                      device.DeviceID,
                                                      device.Model,
                                                      device.Platform,
                                                      device.ProcessArchitecture,
                                                      device.TimeZoneOffset,
                                                      device.HwInfo ) { }


    public static DeviceDescriptor Create( AppVersion appVersion, string deviceID, HwInfo? hwInfo = default, int? osApiLevel = default )
    {
        // #if __IOS__
        //     deviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        // #elif __ANDROID__
        //         deviceID = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
        // #else
        //         deviceID = info.DeviceID();
        // #endif


        // string? sdkVersion = typeof(DeviceDescriptor).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        return new DeviceDescriptor( appVersion.Build ??
                                     GetFileVersion()
                                        .Build,
                                     Assembly.GetEntryAssembly()
                                            ?.EntryPoint?.DeclaringType?.Namespace,
                                     appVersion,
                                     Environment.OSVersion.VersionString,
                                     osApiLevel,
                                     OsVersion: Environment.OSVersion.Version,
                                     RuntimeInformation.FrameworkDescription,
                                     RuntimeEnvironment.GetSystemVersion(),
                                     CultureInfo.CurrentCulture.Name,
                                     deviceID,
                                     GetDeviceModel(),
                                     Environment.OSVersion.Platform,
                                     RuntimeInformation.ProcessArchitecture,
                                     TimeZoneInfo.Local.BaseUtcOffset,
                                     hwInfo );
    }


    public static AppVersion GetFileVersion()
    {
        string? fileName = Assembly.GetEntryAssembly()
                                  ?.Location;

        if ( string.IsNullOrWhiteSpace( fileName ) ) { fileName = Environment.GetCommandLineArgs()[0]; }

        return FileVersionInfo.GetVersionInfo( fileName )
                              .FileVersion ??
               UNKNOWN;
    }


    // public static string? GetSdkVersion()
    // {
    //     WpfHelper.IsRunningOnWpf
    //         ? "appcenter.wpf"
    //         : "appcenter.winforms";
    // }
    public static string? GetDeviceModel()
    {
    #if !NETSTANDARD2_1
        if ( OperatingSystem.IsAndroid() ) { }

        if ( OperatingSystem.IsIOS() ) { }

        if ( OperatingSystem.IsBrowser() ) { }

        if ( OperatingSystem.IsFreeBSD() ) { }

        if ( OperatingSystem.IsWatchOS() ) { }

        if ( OperatingSystem.IsTvOS() ) { }
    #endif


        if ( RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) )
        {
            Process process = new Process();

            ProcessStartInfo processStartInfo = new ProcessStartInfo
                                                {
                                                    WindowStyle            = ProcessWindowStyle.Hidden,
                                                    FileName               = "/bin/bash",
                                                    Arguments              = @"wmic computersystem get model",
                                                    RedirectStandardOutput = true,
                                                    RedirectStandardError  = true,
                                                    UseShellExecute        = false
                                                };

            process.StartInfo = processStartInfo;
            process.Start();

            return process.StandardOutput.ReadToEnd()
                          .Replace( "Model", "", StringComparison.OrdinalIgnoreCase )
                          .Trim();
        }


        if ( RuntimeInformation.IsOSPlatform( OSPlatform.Linux ) )
        {
            Process process = new Process();

            ProcessStartInfo processStartInfo = new ProcessStartInfo
                                                {
                                                    WindowStyle            = ProcessWindowStyle.Hidden,
                                                    FileName               = "/bin/bash",
                                                    Arguments              = @"sudo dmidecode | less | grep Version | sed -n '2p'",
                                                    RedirectStandardOutput = true,
                                                    RedirectStandardError  = true,
                                                    UseShellExecute        = false
                                                };

            process.StartInfo = processStartInfo;
            process.Start();

            return process.StandardOutput.ReadToEnd();
        }


        if ( RuntimeInformation.IsOSPlatform( OSPlatform.OSX ) )
        {
            Process process = new Process();

            ProcessStartInfo processStartInfo = new ProcessStartInfo
                                                {
                                                    WindowStyle            = ProcessWindowStyle.Hidden,
                                                    FileName               = "/bin/bash",
                                                    Arguments              = @"sysctl hw.model",
                                                    RedirectStandardOutput = true,
                                                    RedirectStandardError  = true,
                                                    UseShellExecute        = false
                                                };

            process.StartInfo = processStartInfo;
            process.Start();

            return process.StandardOutput.ReadToEnd();
        }


        return default;
    }


    public static AppVersion? GetOsBuild() // TODO: GetOsBuild other than windows
    {
        if ( RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) )
        {
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

            object? version = registryKey.GetValue( "CurrentVersion", "0.0" );
            object? build   = registryKey.GetValue( "CurrentBuild",   "0" );

            string[]? strArray = registryKey.GetValue( "BuildLabEx" )
                                           ?.ToString()
                                           ?.Split( '.' );

            string str = strArray is null || strArray.Length < 2
                             ? "0"
                             : strArray[1];

            return $"{version}.{build}.{str}";
        }


        /*

        #if !NETSTANDARD2_1
            if ( OperatingSystem.IsAndroid() ) { }

            if ( OperatingSystem.IsIOS() ) { }

            if ( OperatingSystem.IsBrowser() ) { }

            if ( OperatingSystem.IsFreeBSD() ) { }

            if ( OperatingSystem.IsWatchOS() ) { }

            if ( OperatingSystem.IsTvOS() ) { }
        #endif


        #if NETSTANDARD2_1
            if ( RuntimeInformation.IsOSPlatform( OSPlatform.Linux ) )
        #else
            if ( OperatingSystem.IsLinux() )
        #endif
            { }


        #if NETSTANDARD2_1
            if ( RuntimeInformation.IsOSPlatform( OSPlatform.OSX ) )
        #else
            if ( OperatingSystem.IsMacCatalyst() )
                if ( OperatingSystem.IsMacOS() )
        #endif
            { }

        */


        return Environment.OSVersion.Version.ToString();
    }
}
