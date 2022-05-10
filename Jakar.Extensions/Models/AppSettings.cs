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
public class AppSettings<TViewPage> : ObservableClass, IAppSettings<TViewPage>
{
    private string     _appName = string.Empty;
    private AppVersion _appVersion;
    private bool       _crashDataPending;
    private TViewPage? _currentViewPage;
    private Guid       _deviceID;
    private string     _deviceVersion = string.Empty;
    private string?    _screenShotAddress;
    private bool       _sendCrashes;


    public Guid DeviceID
    {
        get => _deviceID;
        set => SetProperty(ref _deviceID, value);
    }

    public bool SendCrashes
    {
        get => _sendCrashes;
        set => SetProperty(ref _sendCrashes, value);
    }

    public string? ScreenShotAddress
    {
        get => _screenShotAddress;
        set => SetProperty(ref _screenShotAddress, value);
    }

    public virtual TViewPage? CurrentViewPage
    {
        get => _currentViewPage;
        set => SetProperty(ref _currentViewPage, value);
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
}
