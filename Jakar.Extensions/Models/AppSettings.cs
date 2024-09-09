namespace Jakar.Extensions;


public interface IScreenShotAddress
{
    public LocalFile? ScreenShotAddress { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
}



public interface IAppSettings : IDeviceID
{
    public string     AppName       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public AppVersion AppVersion    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public string     DeviceVersion { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }


    public void SetDeviceID( Guid id );
}



public interface IAppSettings<TViewPage> : IAppSettings, IScreenShotAddress
{
    public TViewPage? CurrentViewPage { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
}



[Serializable]
public class AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : BaseHostViewModel( hostInfo, defaultHostInfo ), IAppSettings, IScreenShotAddress
{
    private Guid       _deviceID;
    private LocalFile? _screenShotAddress;


    public string     AppName    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = appName;
    public AppVersion AppVersion { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = version;
    public Guid DeviceID
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _deviceID;
        set
        {
            if ( SetProperty( ref _deviceID, value ) ) { OnSetDeviceID( value ); }
        }
    }
    public         string     DeviceVersion     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = deviceVersion;
    public virtual LocalFile? ScreenShotAddress { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _screenShotAddress; set => SetProperty( ref _screenShotAddress, value ); }


    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : this( appName, version, deviceVersion, hostInfo, hostInfo ) { }


    public            void SetDeviceID( Guid      id )       => DeviceID = id;
    protected virtual void OnSetDeviceID( in Guid deviceID ) { }
}



[Serializable]
public class AppSettings<TViewPage>( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : AppSettings( appName, version, deviceVersion, hostInfo, defaultHostInfo ), IAppSettings<TViewPage>
{
    private TViewPage? _currentViewPage;

    public virtual TViewPage? CurrentViewPage { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _currentViewPage; set => SetProperty( ref _currentViewPage, value ); }

    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : this( appName, version, deviceVersion, hostInfo, hostInfo ) { }
}
