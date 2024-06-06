namespace Jakar.Extensions;


public interface IAppName
{
#if NET8_0_OR_GREATER
    public static abstract string     Name    { get; }
    public static abstract AppVersion Version { get; }
#endif
}
