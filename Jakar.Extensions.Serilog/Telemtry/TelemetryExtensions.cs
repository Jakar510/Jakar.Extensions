// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 07/01/2024  13:07

using Microsoft.Extensions.Options;



namespace Jakar.Extensions.Serilog;


public static class TelemetryExtensions
{
    public static IServiceCollection AddTelemetry<TSerilogger, TApp>( this IServiceCollection collection, SeriloggerOptions options )
        where TSerilogger : Serilogger<TSerilogger, TApp>, ICreateSerilogger<TSerilogger>
        where TApp : IAppID
    {
        collection.AddSingleton<IOptions<SeriloggerOptions>>( options );
        TSerilogger serilogger = TSerilogger.Create( options );
        collection.AddSingleton( serilogger );
        collection.AddSingleton( TSerilogger.GetProvider );
        return collection;
    }
    public static IServiceCollection AddTelemetry<TSerilogger, TSeriloggerSettings, TApp>( this IServiceCollection collection, SeriloggerOptions options )
        where TSerilogger : Serilogger<TSerilogger, TSeriloggerSettings, TApp>, ICreateSerilogger<TSerilogger>
        where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>, ISeriloggerSettings
        where TApp : IAppID
    {
        collection.AddSingleton<IOptions<SeriloggerOptions>>( options );
        TSerilogger serilogger = TSerilogger.Create( options );
        collection.AddSingleton( serilogger );
        collection.AddSingleton( TSerilogger.GetProvider );
        return collection;
    }


    public static IServiceCollection AddTelemetry<TSerilogger, TApp>( this IServiceCollection collection, Func<IServiceProvider, SeriloggerOptions> configure )
        where TSerilogger : Serilogger<TSerilogger, TApp>, ICreateSerilogger<TSerilogger>
        where TApp : IAppID
    {
        collection.AddScoped<IOptions<SeriloggerOptions>>( configure );
        collection.AddScoped( TSerilogger.Create );
        collection.AddScoped( TSerilogger.GetProvider );
        return collection;
    }
    public static IServiceCollection AddTelemetry<TSerilogger, TSeriloggerSettings, TApp>( this IServiceCollection collection, Func<IServiceProvider, SeriloggerOptions> configure )
        where TSerilogger : Serilogger<TSerilogger, TSeriloggerSettings, TApp>, ICreateSerilogger<TSerilogger>
        where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>, ISeriloggerSettings
        where TApp : IAppID
    {
        collection.AddScoped<IOptions<SeriloggerOptions>>( configure );
        collection.AddScoped( TSerilogger.Create );
        collection.AddScoped( TSerilogger.GetProvider );
        return collection;
    }
}
