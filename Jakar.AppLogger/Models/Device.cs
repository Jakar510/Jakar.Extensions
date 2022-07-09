using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Reflection;
using Microsoft.AppCenter.Utils;
using Microsoft.Win32;



namespace Jakar.AppLogger;


public interface IDevice
{
    /// <summary>Gets the name of the SDK.</summary>
    /// <value>Name of the SDK. Consists of the name of the SDK and the platform, e.g. "mobilecenter.ios", "mobilecenter.android"</value>
    string SdkName { get; }

    /// <summary>Gets the SDK version.</summary>
    /// <value>Version of the SDK in semver format, e.g. "1.2.0" or "0.12.3-alpha.1".</value>
    string? SdkVersion { get; }

    /// <summary>Gets the device model.</summary>
    /// <value>Device model (example: iPad2,3).</value>
    string? Model { get; }

    /// <summary>Gets the name of the manufacturer.</summary>
    /// <value> Device manufacturer (example: HTC).</value>
    string? OemName { get; }

    /// <summary>Gets the name of the OS.</summary>
    /// <value>OS name (example: iOS).</value>
    string OsName { get; }

    /// <summary>Gets the OS version.</summary>
    /// <value>OS version (example: 9.3.0).</value>
    string? OsVersion { get; }

    /// <summary>Gets the OS build</summary>
    /// <value>OS build code (example: LMY47X).</value>
    string? OsBuild { get; }

    /// <summary>Gets the OS API level.</summary>
    /// <value>API level when applicable like in Android (example: 15).</value>
    int? OsApiLevel { get; }

    /// <summary>Gets the locale.</summary>
    /// <value>Language code (example: en_US).</value>
    string Locale { get; }

    /// <summary>Gets the time zone offset.</summary>
    /// <value>The offset in minutes from UTC for the device time zone, including daylight savings time.</value>
    double TimeZoneOffset { get; }

    /// <summary>Gets the size of the screen.</summary>
    /// <value>Screen size of the device in pixels (example: 640x480).</value>
    string? ScreenSize { get; }

    /// <summary>Gets the application version.</summary>
    /// <value>Application version name, e.g. 1.1.0</value>
    string AppVersion { get; }

    /// <summary>Gets the name of the carrier.</summary>
    /// <value>Carrier name (for mobile devices).</value>
    string? CarrierName { get; }

    /// <summary>Gets the carrier country/region.</summary>
    /// <value>Carrier country code (for mobile devices).</value>
    string? CarrierCountry { get; }

    /// <summary>Gets the app build.</summary>
    /// <value>The app's build number, e.g. 42.</value>
    string AppBuild { get; }

    /// <summary>Gets the app namespace.</summary>
    /// <value>The bundle identifier, package identifier, or namespace, depending on what the individual platforms
    /// use, e.g.com.microsoft.example.</value>
    string? AppNamespace { get; }
}



/// <summary>Device class to help retrieve device information.</summary>
public sealed class Device : IDevice
{
    public string  SdkName        { get; init; } = string.Empty;
    public string? SdkVersion     { get; init; }
    public string? Model          { get; init; }
    public string? OemName        { get; init; }
    public string  OsName         { get; init; } = string.Empty;
    public string  OsVersion      { get; init; } = string.Empty;
    public string? OsBuild        { get; init; }
    public int?    OsApiLevel     { get; set; }
    public string  Locale         { get; init; } = string.Empty;
    public double  TimeZoneOffset { get; init; }
    public string? ScreenSize     { get; init; }
    public string  AppVersion     { get; init; } = string.Empty;
    public string? CarrierName    { get; set; }
    public string? CarrierCountry { get; set; }
    public string  AppBuild       { get; init; } = string.Empty;
    public string? AppNamespace   { get; init; }


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
        OemName        = device.OemName;
        OsName         = device.OsName;
        OsVersion      = device.OsVersion;
        OsBuild        = device.OsBuild;
        OsApiLevel     = device.OsApiLevel;
        Locale         = device.Locale;
        TimeZoneOffset = device.TimeZoneOffset;
        ScreenSize     = device.ScreenSize;
        AppVersion     = device.AppVersion;
        CarrierName    = device.CarrierName;
        CarrierCountry = device.CarrierCountry;
        AppBuild       = device.AppBuild;
        AppNamespace   = device.AppNamespace;
    }

    public static Device Create( AppVersion version )
    {
        // Microsoft.AppCenter.Device


        return new Device
               {
                   SdkName        = RuntimeInformation.FrameworkDescription,
                   SdkVersion     = typeof(Device).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
                   Model          = GetDeviceModel(),
                   OemName        = GetOemName(),
                   OsName         = RuntimeInformation.OSDescription,
                   OsVersion      = RuntimeEnvironment.GetSystemVersion(),
                   OsBuild        = GetOsBuild(),
                   Locale         = CultureInfo.CurrentCulture.Name,
                   TimeZoneOffset = TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes,
                   ScreenSize     = GetScreenSize(),
                   AppVersion     = version.ToString(),
                   AppBuild       =  FileVersion ?? "Unknown",
                   AppNamespace   = Assembly.GetEntryAssembly()?.EntryPoint?.DeclaringType?.Namespace
               };
    }
    
    private static string? FileVersion
    {
        get
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if ( !( entryAssembly != null ) ) { return Application.ProductVersion; }

            string fileName = entryAssembly.Location;
            if ( string.IsNullOrWhiteSpace(fileName) ) { fileName = Environment.GetCommandLineArgs()[0]; }

            return FileVersionInfo.GetVersionInfo(fileName).FileVersion;
        }
    }

    public static ManagementClass GetComputerSystemClass() => new("Win32_ComputerSystem");
    public static ManagementClass GetOperatingSystemClass() => new("Win32_OperatingSystem");


    private static string? GetOemName()
    {
        if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
        {
            try
            {
                using ManagementObjectCollection.ManagementObjectEnumerator enumerator = GetComputerSystemClass().GetInstances().GetEnumerator();

                if ( enumerator.MoveNext() )
                {
                    var str = (string)enumerator.Current["Manufacturer"];

                    return string.IsNullOrEmpty(str) || AbstractDeviceInformationHelper.DefaultSystemManufacturer == str
                               ? default
                               : str;
                }
            }
            catch ( UnauthorizedAccessException ex )
            {
               $"Failed to get device OEM name with error: {ex}".WriteToDebug();
                return string.Empty;
            }
            catch ( COMException ex )
            {
                $"Failed to get device OEM name. Make sure that WMI service is enabled. {ex}".WriteToDebug();
                return string.Empty;
            }
            catch ( ManagementException ex )
            {
                $"Failed to get device model. Make sure that WMI service is enabled. {ex}".WriteToDebug();
                return string.Empty;
            }
        }

        return default;
    }
    private static string? GetScreenSize()
    {
        if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
        {
            using Graphics graphics   = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr         hdc        = graphics.GetHdc();
            int            deviceCaps = DeviceInformationHelper.GetDeviceCaps(hdc, 117);
            return $"{DeviceInformationHelper.GetDeviceCaps(hdc, 118)}x{deviceCaps}";
        }

        return default;
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
        if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
        {
            using RegistryKey  localMachine = Registry.LocalMachine;
            using RegistryKey? registryKey  = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
            if ( registryKey is null ) { return default; }

            object? obj1 = registryKey.GetValue("CurrentMajorVersionNumber");

            if ( obj1 is not null )
            {
                object? obj2 = registryKey.GetValue("CurrentMinorVersionNumber", "0");
                object? obj3 = registryKey.GetValue("CurrentBuildNumber",        "0");
                object? obj4 = registryKey.GetValue("UBR",                       "0");
                return $"{obj1}.{obj2}.{obj3}.{obj4}";
            }

            object?   obj5     = registryKey.GetValue("CurrentVersion", "0.0");
            object?   obj6     = registryKey.GetValue("CurrentBuild",   "0");
            string[]? strArray = registryKey.GetValue("BuildLabEx")?.ToString()?.Split('.');

            string str = strArray is null || strArray.Length < 2
                             ? "0"
                             : strArray[1];

            return $"{obj5}.{obj6}.{str}";
        }

        return default;
    }
}
