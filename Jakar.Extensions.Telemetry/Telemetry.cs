namespace Jakar.Extensions.Telemetry;


// OpenTelemetry Logger



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class Telemetry( ActivityCollection collection, IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null ) : LoggerFactory( providers, filterOption, options, scopeProvider )
{
    public Activity           RootActivity { get; } = collection.StartActivity( collection.Context.AppName );
    public ActivityCollection Source       { get; } = collection;


    public static Telemetry Get( IServiceProvider provider ) => provider.GetRequiredService<Telemetry>();
}



public class Telemetry<TApp>( ActivityCollection collection, IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null ) : Telemetry( collection, providers, filterOption, options, scopeProvider )
    where TApp : IAppID
{
    public Telemetry( IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null ) : this( ActivityCollection.Create<TApp>(), providers, filterOption, options, scopeProvider ) { }


    public static Telemetry<TApp> Create( IServiceProvider provider ) =>
        Create( provider.GetRequiredService<IEnumerable<ILoggerProvider>>(), provider.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>(), provider.GetService<IOptions<LoggerFactoryOptions>>(), provider.GetService<IExternalScopeProvider>() );

    public static Telemetry<TApp> Create( IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null ) => new(providers, filterOption, options, scopeProvider);
}
