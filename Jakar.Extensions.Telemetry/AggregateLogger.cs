// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 02/13/2025  15:02

namespace Jakar.Extensions.Telemetry;


public sealed class AggregateLogger( ReadOnlyMemory<ILogger> loggers ) : ILogger
{
    private readonly ReadOnlyMemory<ILogger> _loggers = loggers;


    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
    {
        foreach ( ILogger logger in _loggers.Span ) { logger.Log( logLevel, eventId, state, exception, formatter ); }
    }
    public bool IsEnabled( LogLevel logLevel )
    {
        foreach ( ILogger logger in _loggers.Span )
        {
            if ( logger.IsEnabled( logLevel ) ) { return true; }
        }

        return false;
    }
    public IDisposable BeginScope<TState>( TState state )
        where TState : notnull
    {
        List<IDisposable> states = new(_loggers.Length + 1);

        foreach ( ILogger logger in _loggers.Span )
        {
            IDisposable? disposable = logger.BeginScope( state );
            if ( disposable is not null ) { states.Add( disposable ); }
        }


        return new Scope( states );
    }



    public sealed class Scope( List<IDisposable> states ) : IDisposable
    {
        private readonly List<IDisposable> _states = states;

        public void Dispose()
        {
            foreach ( IDisposable disposable in CollectionsMarshal.AsSpan( _states ) ) { disposable.Dispose(); }
        }
    }
}
