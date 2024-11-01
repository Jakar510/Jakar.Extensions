// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:26 PM

namespace Jakar.Extensions;


public readonly struct StopWatch( string caller ) : IDisposable
{
    private readonly string _caller = caller;
    private readonly long   _start  = Stopwatch.GetTimestamp();


    public TimeSpan Elapsed { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Stopwatch.GetElapsedTime( _start ); }


    public          void   Dispose()  => Console.WriteLine( ToString() );
    public override string ToString() => $"[{_caller}] {Elapsed}";


    public static StopWatch Start( [CallerMemberName] string caller = EMPTY ) => new(caller);
}
