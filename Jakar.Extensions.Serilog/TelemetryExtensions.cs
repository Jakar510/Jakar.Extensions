// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  13:07

namespace Jakar.Extensions.Serilog;


public static class TelemetryExtensions
{
    public static IServiceCollection AddTelemetry( this IServiceCollection collection ) => collection.AddTelemetry( TelemetryLogger.Instance );
    public static IServiceCollection AddTelemetry<T>( this IServiceCollection collection, T logger )
        where T : TelemetryLogger
    {
        collection.AddSingleton<TelemetryLogger>( logger );
        collection.AddHostedService( TelemetryLogger.Get );
        return collection;
    }
    public static LoggerConfiguration WithTelemetry( this LoggerConfiguration configuration ) => configuration.WriteTo.Async( static x => x.Sink( TelemetryLogger.Instance ) );
}
