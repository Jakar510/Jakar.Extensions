namespace Jakar.Extensions;


public interface IAppName
{
#if NET8_0_OR_GREATER
    public static abstract string     AppName    { get; }
    public static abstract AppVersion AppVersion { get; }
#endif
}



public interface IAppID : IAppName
{
#if NET8_0_OR_GREATER
    public static abstract Guid AppID { get; }
#endif
}
