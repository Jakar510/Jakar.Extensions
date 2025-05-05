// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 05/05/2025  00:01

using System.IO;



namespace Jakar.Extensions.Telemetry.Meters;


public readonly struct TelemetryStopWatch( string caller, TextWriter? writer = null ) : IDisposable
{
    private readonly TextWriter? _writer = writer;
    private readonly string      _caller = caller;
    private readonly long        _start  = Stopwatch.GetTimestamp();
    public readonly  string      ID      = Guids.NewBase64();

    public TimeSpan Elapsed { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Stopwatch.GetElapsedTime( _start, Stopwatch.GetTimestamp() ); }


    public void Dispose()
    {
        if ( _writer is not null ) { _writer.WriteLine( ToString() ); }
        else { Debug.WriteLine( ToString() ); }
    }
    public override string ToString()
    {
        TimeSpan                            elapsed = Elapsed;
        (double Value, StopWatch.Unit Unit) range   = StopWatch.GetRangeWithUnit( in elapsed );
        return $"[{_caller}] {range.Value} {range.Unit}";
    }


    public static TelemetryStopWatch Start( [CallerMemberName] string caller                                   = BaseRecord.EMPTY ) => new(caller);
    public static TelemetryStopWatch Start( TextWriter                writer, [CallerMemberName] string caller = BaseRecord.EMPTY ) => new(caller, writer);
}
