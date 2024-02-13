// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:26 PM

namespace Jakar.Extensions;


public readonly struct StopWatch( string caller ) : IDisposable
{
    private readonly string   _caller = caller;
    private readonly DateTime _start = DateTime.Now;


    public TimeSpan Elapsed => DateTime.Now - _start;


    public          void   Dispose()  => Console.WriteLine( ToString() );
    public override string ToString() => $"[{_caller}] {Elapsed}";


    public static StopWatch Start( [ CallerMemberName ] string? caller = default ) => new(caller ?? string.Empty);
}
