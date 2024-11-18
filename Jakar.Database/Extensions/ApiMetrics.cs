// Jakar.Extensions :: Jakar.Database
// 06/02/2024  10:06

namespace Jakar.Database;


public sealed class ApiMetric
{
    private readonly Counter<long>     _counter;
    private readonly Histogram<double> _histogram;


    public ApiMetric( IMeterFactory meterFactory, string meterName )
    {
        Meter meter = meterFactory.Create( meterName );
        _counter   = meter.CreateCounter<long>( $"{meterName}.Count" );
        _histogram = meter.CreateHistogram<double>( $"{meterName}.Duration", "ms" );
    }

    public void IncreaseRequestCount()                                                    => _counter.Add( 1 );
    public void IncreaseRequestCount( Tag                      tag1 )                     => _counter.Add( 1, tag1 );
    public void IncreaseRequestCount( Tag                      tag1, Tag tag2 )           => _counter.Add( 1, tag1, tag2 );
    public void IncreaseRequestCount( Tag                      tag1, Tag tag2, Tag tag3 ) => _counter.Add( 1, tag1, tag2, tag3 );
    public void IncreaseRequestCount( params ReadOnlySpan<Tag> tags ) => _counter.Add( 1, Tag.Convert( in tags ) );


    public Duration MeasureRequestDuration() => new(_histogram);


    public static ApiMetric Create<TApp>( IServiceProvider provider, string method )
        where TApp : IAppName => Create<TApp>( provider.GetRequiredService<IMeterFactory>(), method );
    public static ApiMetric Create<TApp>( IMeterFactory factory, string method )
        where TApp : IAppName => new(factory, $"{TApp.AppName}.{method}");



    public sealed class Duration( Histogram<double> histogram ) : IDisposable
    {
        private readonly Histogram<double> _histogram        = histogram;
        private readonly long              _requestStartTime = TimeProvider.System.GetTimestamp();

        public void Dispose()
        {
            TimeSpan elapsed = TimeProvider.System.GetElapsedTime( _requestStartTime );
            _histogram.Record( elapsed.TotalMilliseconds );
        }
    }
}



public static class ApiMetrics
{
    /*
    public static TBuilder WithApiMetric<TBuilder>( this TBuilder builder )
        where TBuilder : IEndpointConventionBuilder
    {
        builder.CacheOutput( MinApis.ExpireOneMinutes );
        return builder;
    }
    */


    public static IServiceCollection AddApiMetrics<TApp>( this IServiceCollection services, string method )
        where TApp : IAppName => services.AddSingleton( provider => ApiMetric.Create<TApp>( provider, method ) );
    public static IServiceCollection AddApiMetrics<TApp>( this IServiceCollection services, scoped in ReadOnlySpan<string> methods )
        where TApp : IAppName
    {
        foreach ( string method in methods ) { services.AddApiMetrics<TApp>( method ); }

        return services;
    }
}
