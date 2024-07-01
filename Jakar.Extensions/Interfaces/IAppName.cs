namespace Jakar.Extensions;


public interface IAppName
{
#if NET8_0_OR_GREATER
    public abstract static string     AppName    { get; }
    public abstract static AppVersion AppVersion { get; }
#endif
}



#if NET8_0_OR_GREATER
public interface IAppID<out T> : IAppName
{
    public abstract static T AppID { get; }
}



public interface IAppID : IAppID<Guid>;
#endif
