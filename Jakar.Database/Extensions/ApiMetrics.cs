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

    public void IncreaseRequestCount()                                        => _counter.Add( 1 );
    public void IncreaseRequestCount( Tag          tag1 )                     => _counter.Add( 1, tag1 );
    public void IncreaseRequestCount( Tag          tag1, Tag tag2 )           => _counter.Add( 1, tag1, tag2 );
    public void IncreaseRequestCount( Tag          tag1, Tag tag2, Tag tag3 ) => _counter.Add( 1, tag1, tag2, tag3 );
    public void IncreaseRequestCount( params Tag[] tags ) => _counter.Add( 1, Tag.Convert( tags ) );


    public Duration MeasureRequestDuration() => new(_histogram);


    public static ApiMetric Create<T>( IServiceProvider provider, string method )
        where T : IAppName => Create<T>( provider.GetRequiredService<IMeterFactory>(), method );
    public static ApiMetric Create<T>( IMeterFactory factory, string method )
        where T : IAppName => new(factory, $"{typeof(T).Name}.{method}");



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


    public static IServiceCollection AddApiMetrics<T>( this IServiceCollection services, string method )
        where T : IAppName => services.AddSingleton( provider => ApiMetric.Create<T>( provider, method ) );
    public static IServiceCollection AddApiMetrics<T>( this IServiceCollection services, scoped in ReadOnlySpan<string> methods )
        where T : IAppName
    {
        foreach ( string method in methods ) { services.AddApiMetrics<T>( method ); }

        return services;
    }
}
