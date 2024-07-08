// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  13:07

using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Serilog;


public static class TelemetryExtensions
{
    public static ILoggingBuilder AddTelemetry<TApp>( this ILoggingBuilder builder, LocalDirectory appData )
        where TApp : IAppID => builder.AddTelemetry<TApp>( new Serilogger<TApp>( appData ) );
    public static ILoggingBuilder AddTelemetry<TApp>( this ILoggingBuilder builder, Serilogger<TApp> options )
        where TApp : IAppID => options.Configure( builder );


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
