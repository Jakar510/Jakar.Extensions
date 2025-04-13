// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:26 PM

namespace Jakar.Extensions;


public readonly struct StopWatch( string caller, TextWriter? writer = null ) : IDisposable
{
    private readonly TextWriter? _writer = writer;
    private readonly string      _caller = caller;
    private readonly long        _start  = Stopwatch.GetTimestamp();


    public TimeSpan Elapsed { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Stopwatch.GetElapsedTime( _start, Stopwatch.GetTimestamp() ); }


    public void Dispose()
    {
        if (_writer is not null) { _writer.WriteLine( ToString() ); }
        else { Debug.WriteLine(ToString()); }
    }
    public override string ToString()
    {
        TimeSpan                    elapsed = Elapsed;
        (double Value, string Unit) range   = GetRange( in elapsed );
        return $"[{_caller}] {range.Value} {range.Unit}";
    }
    
    
    public static (double Value, string Unit) GetRange( ref readonly TimeSpan span )
    {
        if ( span.Days > 0 ) { return (span.TotalDays, nameof(TimeSpan.Days)); }

        if ( span.Hours > 0 ) { return (span.TotalHours, nameof(TimeSpan.Hours)); }

        if ( span.Minutes > 0 ) { return (span.TotalMinutes, nameof(TimeSpan.Minutes)); }

        if ( span.Seconds > 0 ) { return (span.TotalSeconds, nameof(TimeSpan.Seconds)); }

        if ( span.Milliseconds > 0 ) { return (span.TotalMilliseconds, nameof(TimeSpan.Milliseconds)); }

        if ( span.Microseconds > 0 ) { return (span.TotalMicroseconds, nameof(TimeSpan.Microseconds)); }

        return (span.TotalNanoseconds, nameof(TimeSpan.Nanoseconds));
    }
    public static (double Value, Unit Unit) GetRangeWithUnit( ref readonly TimeSpan span )
    {
        if ( span.Days != 0 ) { return (span.TotalDays, Unit.Days); }

        if ( span.Hours != 0 ) { return (span.TotalHours, Unit.Hours); }

        if ( span.Minutes != 0 ) { return (span.TotalMinutes, Unit.Minutes); }

        if ( span.Seconds != 0 ) { return (span.TotalSeconds, Unit.Seconds); }

        if ( span.Milliseconds != 0 ) { return (span.TotalMilliseconds, Unit.Milliseconds); }

        if ( span.Microseconds != 0 ) { return (span.TotalMicroseconds, Unit.Microseconds); }

        return (span.TotalNanoseconds, Unit.Nanoseconds);
    }


    public static StopWatch Start( [CallerMemberName] string caller = EMPTY ) => new(caller);
    public static StopWatch Start(TextWriter writer, [CallerMemberName] string caller = EMPTY ) => new(caller, writer);



    public enum Unit
    {
        Days,
        Hours,
        Minutes,
        Seconds,
        Milliseconds,
        Microseconds,
        Nanoseconds,
    }
}
