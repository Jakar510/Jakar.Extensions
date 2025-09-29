namespace Jakar.Extensions.Telemetry;


public class TelemetryLoggerFactory<TApp>( IEnumerable<ILoggerProvider> providers ) : ILoggerFactory
    where TApp : IAppID
{
    protected readonly List<ILoggerProvider> _providers = [..providers];


    public TelemetryActivitySource ActivitySource { get; } = TelemetryActivitySource.Create<TApp>();


    public virtual void Dispose()
    {
        _providers.Clear();
        ActivitySource.Dispose();
        GC.SuppressFinalize( this );
    }


    public static TelemetryLoggerFactory<TApp> Create( IServiceProvider provider ) => new(provider.GetRequiredService<IEnumerable<ILoggerProvider>>());
    public static TelemetryLoggerFactory<TApp> Get( IServiceProvider    provider ) => provider.GetRequiredService<TelemetryLoggerFactory<TApp>>();


    public virtual ILogger CreateLogger( string categoryName )
    {
        ILogger[] loggers = GC.AllocateUninitializedArray<ILogger>( _providers.Count );
        for ( int i = 0; i < loggers.Length; i++ ) { loggers[i] = _providers[i].CreateLogger( categoryName ); }

        return new AggregateLogger( loggers );
    }
    public void AddProvider( ILoggerProvider provider ) => _providers.Add( provider );
}
