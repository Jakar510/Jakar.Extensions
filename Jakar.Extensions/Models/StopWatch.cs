// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:26 PM

namespace Jakar.Extensions;


public readonly struct StopWatch( string caller ) : IDisposable
{
    private readonly string _caller = caller;
    private readonly long   _start  = Stopwatch.GetTimestamp();


    public TimeSpan Elapsed { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Stopwatch.GetElapsedTime( _start ); }


    public void Dispose() => Console.WriteLine( ToString() );
    public override string ToString()
    {
        (double Value, string Unit) range = GetRange( Elapsed );
        return $"[{_caller}] {range.Value} {range.Unit}";
    }
    public static (double Value, string Unit) GetRange( TimeSpan span )
    {
        if ( span.Days > 0 ) { return (span.TotalDays, nameof(TimeSpan.Days)); }

        if ( span.Hours > 0 ) { return (span.TotalHours, nameof(TimeSpan.Hours)); }

        if ( span.Minutes > 0 ) { return (span.TotalMinutes, nameof(TimeSpan.Minutes)); }

        if ( span.Seconds > 0 ) { return (span.TotalSeconds, nameof(TimeSpan.Seconds)); }

        if ( span.Milliseconds > 0 ) { return (span.TotalMilliseconds, nameof(TimeSpan.Milliseconds)); }

        if ( span.Microseconds > 0 ) { return (span.TotalMicroseconds, nameof(TimeSpan.Microseconds)); }

        return (span.TotalNanoseconds, nameof(TimeSpan.Nanoseconds));
    }


    public static StopWatch Start( [CallerMemberName] string caller = EMPTY ) => new(caller);
}
