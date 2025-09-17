// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 02/13/2025  15:02

using JetBrains.Annotations;



namespace Jakar.Extensions.Telemetry;


public sealed class AggregateLogger( ReadOnlyMemory<ILogger> loggers ) : ILogger
{
    private readonly ReadOnlyMemory<ILogger> __loggers = loggers;


    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
    {
        foreach ( ILogger logger in __loggers.Span )
        {
            if ( logger.IsEnabled(logLevel) ) { logger.Log(logLevel, eventId, state, exception, formatter); }
        }
    }
    public bool IsEnabled( LogLevel logLevel )
    {
        foreach ( ILogger logger in __loggers.Span )
        {
            if ( logger.IsEnabled(logLevel) ) { return true; }
        }

        return false;
    }

    [MustDisposeResource]
    public IDisposable BeginScope<TState>( TState state )
        where TState : notnull
    {
        Disposables states = new();

        foreach ( ILogger logger in __loggers.Span )
        {
            IDisposable? disposable = logger.BeginScope(state);
            if ( disposable is not null ) { states.Add(disposable); }
        }


        return new Scope(states);
    }



    [method: MustDisposeResource]
    public sealed class Scope( Disposables states ) : IDisposable
    {
        private readonly Disposables __states = states;

        public void Dispose() => __states.Dispose();
    }
}
