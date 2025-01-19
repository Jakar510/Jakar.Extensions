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