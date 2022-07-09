// Jakar.Extensions :: Jakar.AppLogger
// 07/07/2022  8:15 AM

using System.Diagnostics;
using System.Management;
using System.Reflection;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Utils;
using Microsoft.Win32;



namespace Jakar.AppLogger;


/// <summary>
/// Implements the abstract device information helper class
/// </summary>
public class DeviceInformationHelper : AbstractDeviceInformationHelper
{
    private       IManagmentClassFactory _managmentClassFactory;
    private const string                 _defaultVersion = "Unknown";

    public DeviceInformationHelper() => _managmentClassFactory = ManagmentClassFactory.Instance;

    /// <summary>
    /// Set the specific class factory for the management class.
    /// </summary>
    /// <param name="factory">Specific management class factory.</param>
    internal void SetManagmentClassFactory( IManagmentClassFactory factory ) => _managmentClassFactory = factory;

    protected override string GetSdkName() => ( WpfHelper.IsRunningOnWpf
                                                    ? "appcenter.wpf"
                                                    : "appcenter.winforms" ) + ".netcore";

    protected override string? GetDeviceModel()
    {
        try
        {
            using ManagementObjectCollection.ManagementObjectEnumerator enumerator = _managmentClassFactory.GetComputerSystemClass().GetInstances().GetEnumerator();

            if ( enumerator.MoveNext() )
            {
                var str = (string)enumerator.Current["Model"];

                return string.IsNullOrEmpty(str) || DefaultSystemProductName == str
                           ? null
                           : str;
            }
        }
        catch ( UnauthorizedAccessException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model with error: ", ex);
            return string.Empty;
        }
        catch ( COMException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model. Make sure that WMI service is enabled.", ex);
            return string.Empty;
        }
        catch ( ManagementException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model. Make sure that WMI service is enabled.", ex);
            return string.Empty;
        }

        return string.Empty;
    }

    protected override string GetAppNamespace() => Assembly.GetEntryAssembly()?.EntryPoint.DeclaringType?.Namespace;

    protected override string GetDeviceOemName()
    {
        try
        {
            using ManagementObjectCollection.ManagementObjectEnumerator enumerator = _managmentClassFactory.GetComputerSystemClass().GetInstances().GetEnumerator();

            if ( enumerator.MoveNext() )
            {
                var str = (string)enumerator.Current["Manufacturer"];

                return string.IsNullOrEmpty(str) || DefaultSystemManufacturer == str
                           ? null
                           : str;
            }
        }
        catch ( UnauthorizedAccessException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OEM name with error: ", ex);
            return string.Empty;
        }
        catch ( COMException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OEM name. Make sure that WMI service is enabled.", ex);
            return string.Empty;
        }
        catch ( ManagementException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model. Make sure that WMI service is enabled.", ex);
            return string.Empty;
        }

        return string.Empty;
    }

    protected override string GetOsName() => "WINDOWS";

    protected override string GetOsBuild()
    {
        using RegistryKey localMachine = Registry.LocalMachine;

        using RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");

        object obj1 = registryKey.GetValue("CurrentMajorVersionNumber");

        if ( obj1 != null )
        {
            object obj2 = registryKey.GetValue("CurrentMinorVersionNumber", "0");
            object obj3 = registryKey.GetValue("CurrentBuildNumber",        "0");
            object obj4 = registryKey.GetValue("UBR",                       "0");
            return string.Format("{0}.{1}.{2}.{3}", obj1, obj2, obj3, obj4);
        }

        object   obj5     = registryKey.GetValue("CurrentVersion", "0.0");
        object   obj6     = registryKey.GetValue("CurrentBuild",   "0");
        string[] strArray = registryKey.GetValue("BuildLabEx")?.ToString().Split('.');

        string str = strArray == null || strArray.Length < 2
                         ? "0"
                         : strArray[1];

        return string.Format("{0}.{1}.{2}", obj5, obj6, str);
    }

    protected override string GetOsVersion()
    {
        try
        {
            using ManagementObjectCollection.ManagementObjectEnumerator enumerator = _managmentClassFactory.GetOperatingSystemClass().GetInstances().GetEnumerator();

            if ( enumerator.MoveNext() ) { return (string)enumerator.Current["Version"]; }
        }
        catch ( UnauthorizedAccessException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OS version with error: ", ex);
            return string.Empty;
        }
        catch ( COMException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OS version. Make sure that WMI service is enabled.", ex);
            return string.Empty;
        }
        catch ( ManagementException ex )
        {
            AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model. Make sure that WMI service is enabled.", ex);
            return string.Empty;
        }

        return string.Empty;
    }

    protected override string GetAppVersion()
    {
        string productVersion = Application.ProductVersion;
        return DeploymentVersion ?? productVersion ?? PackageVersion ?? "Unknown";
    }

    protected override string GetAppBuild() => DeploymentVersion ?? FileVersion ?? PackageVersion ?? "Unknown";

    protected override string GetScreenSize()
    {
        using ( Graphics graphics = Graphics.FromHwnd(IntPtr.Zero) )
        {
            IntPtr hdc        = graphics.GetHdc();
            int    deviceCaps = GetDeviceCaps(hdc, 117);
            return string.Format("{0}x{1}", GetDeviceCaps(hdc, 118), deviceCaps);
        }
    }

    private static string? PackageVersion => null;

    private static string? DeploymentVersion => null;

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

    /// <summary>
    /// Import GetDeviceCaps function to retreive scale-independent screen size.
    /// </summary>
    private static extern int GetDeviceCaps( IntPtr hdc, int nIndex );
}
