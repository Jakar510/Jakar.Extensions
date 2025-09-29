// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 05/05/2025  00:01

using System.IO;



namespace Jakar.Extensions.Telemetry.Meters;


public readonly struct TelemetryStopWatch( string caller, TextWriter? writer = null ) : IDisposable
{
    private readonly TextWriter? __writer = writer;
    private readonly string      __caller = caller;
    private readonly long        __start  = Stopwatch.GetTimestamp();
    public readonly  string      ID      = Guids.NewBase64();

    public TimeSpan Elapsed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Stopwatch.GetElapsedTime(__start, Stopwatch.GetTimestamp()); }


    public void Dispose()
    {
        if ( __writer is not null ) { __writer.WriteLine(ToString()); }
        else { Serilog.Debugging.SelfLog.WriteLine(ToString()); }
    }
    public override string ToString() => SpanDuration.ToString(Elapsed, $"[{__caller}] ");


    public static TelemetryStopWatch Start( [CallerMemberName] string caller                                   = BaseRecord.EMPTY ) => new(caller);
    public static TelemetryStopWatch Start( TextWriter                writer, [CallerMemberName] string caller = BaseRecord.EMPTY ) => new(caller, writer);
}
