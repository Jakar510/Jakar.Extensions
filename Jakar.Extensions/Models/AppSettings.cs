﻿namespace Jakar.Extensions;


public interface IAppSettings
{
    public string     AppName           { get; }
    public AppVersion AppVersion        { get; }
    public Guid       DeviceID          { get; set; }
    public string     DeviceVersion     { get; }
    public LocalFile? ScreenShotAddress { get; set; }
}



public interface IAppSettings<TViewPage> : IAppSettings
{
    public TViewPage? CurrentViewPage { get; set; }
}



[Serializable]
public class AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : BaseHostViewModel( hostInfo, defaultHostInfo ), IAppSettings
{
    private        Guid       _deviceID;
    private        LocalFile? _screenShotAddress;
    public virtual string     AppName { get; } = appName;


    public         AppVersion AppVersion        { get; } = version;
    public virtual Guid       DeviceID          { get => _deviceID; set => SetProperty( ref _deviceID, value ); }
    public virtual string     DeviceVersion     { get; } = deviceVersion;
    public virtual LocalFile? ScreenShotAddress { get => _screenShotAddress; set => SetProperty( ref _screenShotAddress, value ); }


    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : this( appName, version, deviceVersion, hostInfo, hostInfo ) { }
}



[Serializable]
public class AppSettings<TViewPage> : AppSettings, IAppSettings<TViewPage>
{
    private TViewPage? _currentViewPage;

    public virtual TViewPage? CurrentViewPage { get => _currentViewPage; set => SetProperty( ref _currentViewPage, value ); }


    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo ) : base( appName, version, deviceVersion, hostInfo ) { }
    public AppSettings( string appName, AppVersion version, string deviceVersion, Uri hostInfo, Uri defaultHostInfo ) : base( appName, version, deviceVersion, hostInfo, defaultHostInfo ) { }
}
