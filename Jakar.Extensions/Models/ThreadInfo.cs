// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  14:45

namespace Jakar.Extensions;


public readonly record struct ThreadInfo( string Name, int ManagedThreadID, Language CurrentCulture, Language CurrentUICulture )
{
    public ThreadInfo() : this(Thread.CurrentThread) { }
    public ThreadInfo( Thread                          thread ) : this(thread.Name ?? string.Empty, thread.ManagedThreadId, thread.CurrentCulture, thread.CurrentUICulture) { }
    public static implicit operator ThreadInfo( Thread thread ) => new(thread);
    public readonly                 string     Name             = Name;
    public readonly                 int        ManagedThreadID  = ManagedThreadID;
    public readonly                 Language   CurrentCulture   = CurrentCulture;
    public readonly                 Language   CurrentUICulture = CurrentUICulture;
    public static                   ThreadInfo Create() => new();
}
