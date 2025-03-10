// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  13:07

using Microsoft.Extensions.Options;



namespace Jakar.Extensions.Serilog;


public static class TelemetryExtensions
{
    public static IServiceCollection AddTelemetry<TSerilogger>( this IServiceCollection collection, SeriloggerConstants options )
        where TSerilogger : Serilogger<TSerilogger, SeriloggerSettings>, ICreateSerilogger<TSerilogger>
    {
        collection.AddSingleton<IOptions<SeriloggerConstants>>( options );
        TSerilogger serilogger = TSerilogger.Create( options );
        collection.AddSingleton( serilogger );
        collection.AddSingleton( TSerilogger.GetProvider );
        return collection;
    }
    public static IServiceCollection AddTelemetry<TSerilogger, TSeriloggerSettings>( this IServiceCollection collection, SeriloggerConstants options )
        where TSerilogger : Serilogger<TSerilogger, TSeriloggerSettings>, ICreateSerilogger<TSerilogger>
        where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>, ISeriloggerSettings
    {
        collection.AddSingleton<IOptions<SeriloggerConstants>>( options );
        TSerilogger serilogger = TSerilogger.Create( options );
        collection.AddSingleton( serilogger );
        collection.AddSingleton( TSerilogger.GetProvider );
        return collection;
    }


    public static IServiceCollection AddTelemetry<TSerilogger>( this IServiceCollection collection, Func<IServiceProvider, SeriloggerConstants> configure )
        where TSerilogger : Serilogger<TSerilogger, SeriloggerSettings>, ICreateSerilogger<TSerilogger>
    {
        collection.AddScoped<IOptions<SeriloggerConstants>>( configure );
        collection.AddScoped( TSerilogger.Create );
        collection.AddScoped( TSerilogger.GetProvider );
        return collection;
    }
    public static IServiceCollection AddTelemetry<TSerilogger, TSeriloggerSettings>( this IServiceCollection collection, Func<IServiceProvider, SeriloggerConstants> configure )
        where TSerilogger : Serilogger<TSerilogger, TSeriloggerSettings>, ICreateSerilogger<TSerilogger>
        where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>, ISeriloggerSettings
    {
        collection.AddScoped<IOptions<SeriloggerConstants>>( configure );
        collection.AddScoped( TSerilogger.Create );
        collection.AddScoped( TSerilogger.GetProvider );
        return collection;
    }
}
