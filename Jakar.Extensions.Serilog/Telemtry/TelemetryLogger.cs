// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/28/2024  10:06

#if DEBUG
namespace Jakar.Extensions.Serilog;



[Experimental( nameof(TelemetryLogger) )]
[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class TelemetryLogger : BackgroundService, IBatchedLogEventSink, ILogEventSink
{
    protected readonly ConcurrentBag<TelemetryLogEvent> _logs = [];
    public static      TelemetryLogger                  Instance { get; protected set; } = new();


    protected TelemetryLogger() { }
    public static  TelemetryLogger Get( IServiceProvider                         provider ) => provider.GetRequiredService<TelemetryLogger>();
    public         void            Emit( LogEvent                                log )      => Emit( TelemetryLogEvent.Create( log ) );
    public virtual void            Emit( TelemetryLogEvent                       log )      => _logs.Add( log );
    public         Task            EmitBatchAsync( IReadOnlyCollection<LogEvent> logs )     => EmitBatchAsync( TelemetryLogEvent.Create( logs ) );
    public Task EmitBatchAsync( IEnumerable<TelemetryLogEvent> logs )
    {
        foreach ( TelemetryLogEvent log in logs ) { Emit( log ); }

        return Task.CompletedTask;
    }


    protected override async Task ExecuteAsync( CancellationToken token )
    {
        while ( token.ShouldContinue() )
        {
            List<TelemetryLogEvent> logs = new(_logs.Count);
            while ( _logs.TryTake( out TelemetryLogEvent? log ) ) { logs.Add( log ); }

            await SendAsync( logs );
        }
    }
    public virtual Task SendAsync<TValue>( TValue logs )
        where TValue : IEnumerable<TelemetryLogEvent> => Task.CompletedTask;


    public override async Task StopAsync( CancellationToken token )
    {
        List<TelemetryLogEvent> logs = new(_logs.Count);
        while ( _logs.TryTake( out TelemetryLogEvent? log ) ) { logs.Add( log ); }

        await SendAsync( logs );
        await base.StopAsync( token );
    }
}
#endif
