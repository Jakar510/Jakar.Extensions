// Jakar.Extensions :: Jakar.AppLogger
// 07/10/2022  9:00 AM

namespace Jakar.AppLogger.Common;


public interface IDevice
{
    /// <summary> Gets the application version. </summary>
    /// <value> Application version name, e.g. 1.1.0 </value>
    AppVersion? AppVersion { get; }

    /// <summary> Gets the time zone offset. </summary>
    /// <value> The offset in minutes from UTC for the device time zone, including daylight savings time. </value>
    double TimeZoneOffset { get; }

    public HwInfo? HwInfo { get; }

    /// <summary> Gets the app build. </summary>
    /// <value> The app's build number, e.g. 42. </value>
    int? AppBuild { get; }

    /// <summary> Gets the OS API level. </summary>
    /// <value> API level when applicable like in Android (example: 15). </value>
    int? OsApiLevel { get; }

    public string DeviceID { get; init; }


    /// <summary> Gets the locale. </summary>
    /// <value> Language code (example: en_US). </value>
    string Locale { get; }


    /// <summary> Gets the name of the OS. </summary>
    /// <value> OS name (example: iOS). </value>
    string OsName { get; }


    /// <summary> Gets the name of the SDK. </summary>
    /// <value> Name of the SDK. Consists of the name of the SDK and the platform, e.g. "mobilecenter.ios", "mobilecenter.android" </value>
    string SdkName { get; }

    /// <summary> Gets the SDK version. </summary>
    /// <value> Version of the SDK in semver format, e.g. "1.2.0" or "0.12.3-alpha.1". </value>
    string SdkVersion { get; }

    /// <summary> Gets the app namespace. </summary>
    /// <value>
    ///     The bundle identifier, package identifier, or namespace, depending on what the individual platforms
    ///     use, e.g.com.microsoft.example.
    /// </value>
    string? AppNamespace { get; }


    /// <summary> Gets the device model. </summary>
    /// <value> Device model (example: iPad2,3). </value>
    string? Model { get; }

    /// <summary> Gets the OS build </summary>
    /// <value> OS build code (example: LMY47X). </value>
    string? OsBuild { get; }

    /// <summary> Gets the OS version. </summary>
    /// <value> OS version (example: 9.3.0). </value>
    string? OsVersion { get; }
}
