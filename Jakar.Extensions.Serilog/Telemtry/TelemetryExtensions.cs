// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  13:07

using Microsoft.Extensions.Options;



namespace Jakar.Extensions.Serilog;


public static class TelemetryExtensions
{
    public static IServiceCollection AddSerilogger( this IServiceCollection collection, SeriloggerOptions options ) => collection.AddSerilogger<Serilogger>( options );
    public static IServiceCollection AddSerilogger<TSerilogger>( this IServiceCollection collection, SeriloggerOptions options )
        where TSerilogger : Serilogger, ICreateSerilogger<TSerilogger>
    {
        collection.AddSingleton<IOptions<SeriloggerOptions>>( options );
        TSerilogger serilogger = TSerilogger.Create( options );
        collection.AddSingleton( serilogger );
        collection.AddSingleton( TSerilogger.GetProvider );
        return collection;
    }


    public static IServiceCollection AddSerilogger( this IServiceCollection collection, Func<IServiceProvider, SeriloggerOptions> configure ) => collection.AddSerilogger<Serilogger>( configure );
    public static IServiceCollection AddSerilogger<TSerilogger>( this IServiceCollection collection, Func<IServiceProvider, SeriloggerOptions> configure )
        where TSerilogger : Serilogger, ICreateSerilogger<TSerilogger>
    {
        collection.AddScoped<IOptions<SeriloggerOptions>>( configure );
        collection.AddScoped( TSerilogger.Create );
        collection.AddScoped( TSerilogger.GetProvider );
        return collection;
    }
}
