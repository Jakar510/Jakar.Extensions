#nullable enable
using System.Globalization;



namespace Jakar.Extensions;


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
public class AppSettings : BaseHostViewModel, IAppSettings
{
    private string     _appName       = string.Empty;
    private string     _deviceVersion = string.Empty;
    private string?    _screenShotAddress;
    private AppVersion _appVersion;
    private bool       _crashDataPending;
    private Guid       _deviceID;
    private bool?      _sendCrashes;


    public virtual Guid DeviceID
    {
        get => _deviceID;
        set => SetProperty(ref _deviceID, value);
    }

    public virtual bool SendCrashes
    {
        get => _sendCrashes ??= Get_SendCrashes();
        set
        {
            SetProperty(ref _sendCrashes, value);
            Set_SendCrashes(value);
        }
    }

    public virtual string? ScreenShotAddress
    {
        get => _screenShotAddress;
        set => SetProperty(ref _screenShotAddress, value);
    }

    public virtual string AppName
    {
        get => _appName;
        set => SetProperty(ref _appName, value);
    }

    public virtual string DeviceVersion
    {
        get => _deviceVersion;
        set => SetProperty(ref _deviceVersion, value);
    }

    public virtual bool CrashDataPending
    {
        get => _crashDataPending;
        set => SetProperty(ref _crashDataPending, value);
    }

    public virtual AppVersion AppVersion
    {
        get => _appVersion;
        set => SetProperty(ref _appVersion, value);
    }


    public AppSettings( string appName, in AppVersion version, string deviceVersion, Uri  hostInfo ) : this(appName, version, deviceVersion, true, hostInfo) { }
    public AppSettings( string appName, in AppVersion version, string deviceVersion, bool sendCrashes, Uri hostInfo ) : this(appName, version, deviceVersion, sendCrashes, hostInfo, hostInfo) { }
    public AppSettings( string appName, in AppVersion version, string deviceVersion, bool sendCrashes, Uri hostInfo, Uri defaultHostInfo ) : base(hostInfo, defaultHostInfo)
    {
        SendCrashes   = sendCrashes;
        AppName       = appName;
        AppVersion    = version;
        DeviceVersion = deviceVersion;
    }


    protected virtual void Set_SendCrashes( bool value ) { }
    protected virtual bool Get_SendCrashes() => true;
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


    public AppSettings( string appName, in AppVersion version, string deviceVersion, Uri  hostInfo ) : base(appName, version, deviceVersion, hostInfo) { }
    public AppSettings( string appName, in AppVersion version, string deviceVersion, bool sendCrashes, Uri hostInfo ) : base(appName, version, deviceVersion, sendCrashes, hostInfo) { }
    public AppSettings( string appName, in AppVersion version, string deviceVersion, bool sendCrashes, Uri hostInfo, Uri defaultHostInfo ) : base(appName, version, deviceVersion, sendCrashes, hostInfo, defaultHostInfo) { }
}
