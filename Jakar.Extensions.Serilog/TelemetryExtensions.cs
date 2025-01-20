// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  13:07

using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Serilog;


public static class TelemetryExtensions
{
    public static ILoggingBuilder AddTelemetry<TApp, TSeriloggerSettings>( this ILoggingBuilder builder, FilePaths paths, Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> takeScreenShot, Func<EventDetails, EventDetails> updateEventDetails )
        where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>, ISeriloggerSettings
        where TApp : IAppID => builder.AddTelemetry( Serilogger.Create<TApp>( paths, takeScreenShot, updateEventDetails ) );
    public static ILoggingBuilder AddTelemetry( this ILoggingBuilder builder, Serilogger logger ) => builder.AddSerilog( logger );


#if DEBUG
#pragma warning disable TelemetryLogger
    [Experimental( nameof(TelemetryLogger) )] public static IServiceCollection AddTelemetry( this IServiceCollection collection ) => collection.AddTelemetry( TelemetryLogger.Instance );

    [Experimental( nameof(TelemetryLogger) )]
    public static IServiceCollection AddTelemetry<T>( this IServiceCollection collection, T logger )
        where T : TelemetryLogger
    {
        collection.AddSingleton<TelemetryLogger>( logger );
        collection.AddHostedService( TelemetryLogger.Get );
        return collection;
    }

    [Experimental( nameof(TelemetryLogger) )] public static LoggerConfiguration WithTelemetry( this LoggerConfiguration configuration ) => configuration.WriteTo.Async( static x => x.Sink( TelemetryLogger.Instance ) );
#pragma warning restore TelemetryLogger
#endif
}
