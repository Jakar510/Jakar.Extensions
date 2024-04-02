namespace Jakar.Extensions;


public interface IAppSettings
{
    public string     AppName           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public AppVersion AppVersion        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public Guid       DeviceID          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public string     DeviceVersion     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile? ScreenShotAddress { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
}



public interface IAppSettings<TViewPage> : IAppSettings
{
    public TViewPage? CurrentViewPage { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
}



[Serializable]
public class AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : BaseHostViewModel( hostInfo, defaultHostInfo ), IAppSettings
{
    private Guid       _deviceID;
    private LocalFile? _screenShotAddress;


    public         string     AppName           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = appName;
    public         AppVersion AppVersion        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = version;
    public         Guid       DeviceID          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _deviceID; set => SetProperty( ref _deviceID, value ); }
    public         string     DeviceVersion     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = deviceVersion;
    public virtual LocalFile? ScreenShotAddress { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _screenShotAddress; set => SetProperty( ref _screenShotAddress, value ); }


    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : this( appName, version, deviceVersion, hostInfo, hostInfo ) { }
}



[Serializable]
public class AppSettings<TViewPage> : AppSettings, IAppSettings<TViewPage>
{
    private TViewPage? _currentViewPage;

    public virtual TViewPage? CurrentViewPage { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _currentViewPage; set => SetProperty( ref _currentViewPage, value ); }


    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : base( appName, version, deviceVersion, hostInfo ) { }
    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : base( appName, version, deviceVersion, hostInfo, defaultHostInfo ) { }
}
