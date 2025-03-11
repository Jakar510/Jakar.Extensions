namespace Jakar.Extensions;


public interface IAppName
{
    public abstract static string     AppName    { get; }
    public abstract static AppVersion AppVersion { get; }
}



public interface IAppID<out T> : IAppName
{
    public abstract static T AppID { get; }
}



public interface IAppID : IAppID<Guid>;



public interface IApp<T> : IAppID<T>
{
    public abstract static T              DebugID        { get; set; }
    public abstract static T              DeviceID       { get; set; }
    public abstract static string         DeviceName     { get; set; }
    public abstract static ActivitySource ActivitySource { get; }
}



public interface IApp : IApp<Guid>;



public interface IAppMetaData
{
    public ActivitySource ActivitySource { get; set; }
    public Guid           AppID          { get; set; }
    public string         AppName        { get; set; }
    public AppVersion     AppVersion     { get; set; }
    public Guid           DebugID        { get; set; }
    public Guid           DeviceID       { get; set; }
    public string         DeviceName     { get; set; }
}
