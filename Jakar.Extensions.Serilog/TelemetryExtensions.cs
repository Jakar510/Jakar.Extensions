// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  13:07

namespace Jakar.Extensions.Serilog;


public static class TelemetryExtensions
{
    public static IServiceCollection AddTelemetry( this IServiceCollection collection ) => collection.AddTelemetry<TelemetryLogger>();
    public static IServiceCollection AddTelemetry<T>( this IServiceCollection collection )
        where T : TelemetryLogger
    {
        collection.AddSingleton<T>();
        collection.AddHostedService<T>();
        return collection;
    }
    public static LoggerConfiguration WithTelemetry( this LoggerConfiguration configuration ) => configuration.WriteTo.Sink( TelemetryLogger.Instance );
}
