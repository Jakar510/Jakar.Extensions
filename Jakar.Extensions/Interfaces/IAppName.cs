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



public sealed class AppData
{
    private ActivitySource? _source;


    public          ActivitySource ActivitySource { get => _source ??= new ActivitySource( AppName ); set => _source = value; }
    public required Guid           AppID          { get;                                              init; }
    public required string         AppName        { get;                                              init; }
    public required AppVersion     AppVersion     { get;                                              init; }
    public          Guid           DebugID        { get;                                              set; } = Guid.NewGuid();
    public          Guid           DeviceID       { get;                                              set; } = Guid.NewGuid();
    public          string         DeviceName     { get;                                              set; } = string.Empty;
}
