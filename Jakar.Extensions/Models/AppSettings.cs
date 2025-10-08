namespace Jakar.Extensions;


public interface IScreenshot
{
    public LocalFile? Screenshot { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; }
}



public interface IAppSettings : IDeviceID
{
    public string     AppName       { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
    public AppVersion AppVersion    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
    public string     DeviceVersion { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }


    public void SetDeviceID( string id );
}



public interface IAppSettings<TViewPage> : IAppSettings, IScreenshot
{
    public TViewPage? CurrentViewPage { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; }
}



[Serializable]
public class AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : BaseHostViewModel(hostInfo, defaultHostInfo), IAppSettings, IScreenshot
{
    private string     __deviceID = EMPTY;
    private LocalFile? __screenshot;


    public string     AppName    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = appName;
    public AppVersion AppVersion { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = version;
    public string DeviceID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __deviceID;
        set
        {
            if ( SetProperty(ref __deviceID, value) ) { OnSetDeviceID(value); }
        }
    }
    public         string     DeviceVersion { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = deviceVersion;
    public virtual LocalFile? Screenshot    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __screenshot; set => SetProperty(ref __screenshot, value); }


    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : this(appName, version, deviceVersion, hostInfo, hostInfo) { }


    public            void SetDeviceID( string      id )       => DeviceID = id;
    protected virtual void OnSetDeviceID( in string deviceID ) { }
}



[Serializable]
public class AppSettings<TViewPage>( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : AppSettings(appName, version, deviceVersion, hostInfo, defaultHostInfo), IAppSettings<TViewPage>
{
    private TViewPage? __currentViewPage;

    public virtual TViewPage? CurrentViewPage { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __currentViewPage; set => SetProperty(ref __currentViewPage, value); }

    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : this(appName, version, deviceVersion, hostInfo, hostInfo) { }
}
