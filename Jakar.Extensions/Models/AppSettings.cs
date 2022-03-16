namespace Jakar.Extensions.Models;


public interface IAppSettings
{
    public string  AppName           { get; }
    public string  DeviceVersion     { get; }
    public bool    SendCrashes       { get; set; }
    public string? ScreenShotAddress { get; set; }
    public bool    CrashDataPending  { get; set; }
}



public interface IAppSettings<out TVersion> : IAppSettings
{
    public TVersion AppVersion { get; }
}



public interface IAppSettings<out TDeviceID, TViewPage, out TVersion> : IAppSettings<TVersion>
{
    public TDeviceID DeviceID        { get; }
    public TViewPage CurrentViewPage { get; set; }
}



public interface IAppSettings<out TDeviceID, TViewPage> : IAppSettings<TDeviceID, TViewPage, Version> { }



public class AppSettings : IAppSettings<Guid, object?, AppVersion>
{
    public         Guid       DeviceID          { get; set; }
    public         bool       SendCrashes       { get; set; }
    public         string?    ScreenShotAddress { get; set; }
    public virtual object?    CurrentViewPage   { get; set; }
    public         string     AppName           { get; set; }
    public         string     DeviceVersion     { get; set; }
    public         bool       CrashDataPending  { get; set; }
    public         AppVersion AppVersion        { get; set; }
}
