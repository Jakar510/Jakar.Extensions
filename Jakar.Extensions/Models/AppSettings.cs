#nullable enable
namespace Jakar.Extensions.Models;


public interface IAppSettings
{
    public string     AppName           { get; }
    public string     DeviceVersion     { get; }
    public bool       SendCrashes       { get; set; }
    public string?    ScreenShotAddress { get; set; }
    public bool       CrashDataPending  { get; set; }
    public AppVersion AppVersion        { get; }
    public Guid       DeviceID          { get; set; }
}



public interface IAppSettings<TViewPage> : IAppSettings
{
    public TViewPage? CurrentViewPage { get; set; }
}



[Serializable]
public class AppSettings : ObservableClass, IAppSettings
{
    private string     _appName = string.Empty;
    private AppVersion _appVersion;
    private bool       _crashDataPending;
    private Guid       _deviceID;
    private string     _deviceVersion = string.Empty;
    private string?    _screenShotAddress;
    private bool       _sendCrashes = Get_SendCrashes();


    public Guid DeviceID
    {
        get => _deviceID;
        set => SetProperty(ref _deviceID, value);
    }

    public virtual bool SendCrashes
    {
        get => _sendCrashes;
        set
        {
            SetProperty(ref _sendCrashes, value);
            Set_SendCrashes(value);
        }
    }

    public string? ScreenShotAddress
    {
        get => _screenShotAddress;
        set => SetProperty(ref _screenShotAddress, value);
    }

    public string AppName
    {
        get => _appName;
        set => SetProperty(ref _appName, value);
    }

    public string DeviceVersion
    {
        get => _deviceVersion;
        set => SetProperty(ref _deviceVersion, value);
    }

    public bool CrashDataPending
    {
        get => _crashDataPending;
        set => SetProperty(ref _crashDataPending, value);
    }

    public AppVersion AppVersion
    {
        get => _appVersion;
        set => SetProperty(ref _appVersion, value);
    }


    public AppSettings() { }
    public AppSettings( string appName, in AppVersion version, string deviceVersion )
    {
        AppName       = appName;
        AppVersion    = version;
        DeviceVersion = deviceVersion;
    }
    public AppSettings( string appName, in AppVersion version, string deviceVersion, bool sendCrashes )
    {
        SendCrashes   = sendCrashes;
        AppName       = appName;
        AppVersion    = version;
        DeviceVersion = deviceVersion;
    }


    protected static void Set_SendCrashes( bool value )
    {
        try { Preferences.Set(nameof(SendCrashes), value); }
        catch ( NotImplementedInReferenceAssemblyException ) { }
        catch ( FeatureNotSupportedException ) { }
    }
    protected static bool Get_SendCrashes()
    {
        try { return Preferences.Get(nameof(SendCrashes), true); }
        catch ( NotImplementedInReferenceAssemblyException ) { return Debugger.IsAttached; }
        catch ( FeatureNotSupportedException ) { return Debugger.IsAttached; }
    }
}



[Serializable]
public class AppSettings<TViewPage> : AppSettings, IAppSettings<TViewPage>
{
    private TViewPage? _currentViewPage;

    public virtual TViewPage? CurrentViewPage
    {
        get => _currentViewPage;
        set => SetProperty(ref _currentViewPage, value);
    }


    public AppSettings() { }
    public AppSettings( string appName, in AppVersion version, string deviceVersion ) : base(appName, version, deviceVersion) { }
    public AppSettings( string appName, in AppVersion version, string deviceVersion, bool sendCrashes ) : base(appName, version, deviceVersion, sendCrashes) { }
}
