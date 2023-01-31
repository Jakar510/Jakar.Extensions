// Jakar.Extensions :: Jakar.Extensions.Xamarin.Forms
// 01/31/2023  2:46 PM

using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Xamarin.Forms;


public sealed class LoggerWrapper : ILogger
{
    private readonly ILogger _logger;


    public LoggerWrapper( ILogger logger ) => _logger = logger;


    public static LoggerWrapper Create( ILogger           logger ) => new(logger);
    public static LoggerWrapper Create<T>( ILoggerFactory factory ) => new(factory.CreateLogger<T>());
    public static LoggerWrapper Create( ILoggerFactory    factory, Type   type ) => new(factory.CreateLogger( type ));
    public static LoggerWrapper Create( ILoggerFactory    factory, string name ) => new(factory.CreateLogger( name ));


    public void Critical( EventId       eventId,   Exception?       exception, string?          message, params object?[] args ) => _logger.Log( LogLevel.Critical, eventId, exception, message, args );
    public void Critical( EventId       eventId,   string?          message,   params object?[] args ) => _logger.Log( LogLevel.Critical, eventId,   message, args );
    public void Critical( Exception?    exception, string?          message,   params object?[] args ) => _logger.Log( LogLevel.Critical, exception, message, args );
    public void Critical( string?       message,   params object?[] args ) => _logger.Log( LogLevel.Critical,                                                    message, args );
    public void Debug( EventId          eventId,   Exception?       exception, string?          message, params object?[] args ) => _logger.Log( LogLevel.Debug, eventId, exception, message, args );
    public void Debug( EventId          eventId,   string?          message,   params object?[] args ) => _logger.Log( LogLevel.Debug, eventId,   message, args );
    public void Debug( Exception?       exception, string?          message,   params object?[] args ) => _logger.Log( LogLevel.Debug, exception, message, args );
    public void Debug( string?          message,   params object?[] args ) => _logger.Log( LogLevel.Debug,                                                       message, args );
    public void Error( EventId          eventId,   Exception?       exception, string?          message, params object?[] args ) => _logger.Log( LogLevel.Error, eventId, exception, message, args );
    public void Error( EventId          eventId,   string?          message,   params object?[] args ) => _logger.Log( LogLevel.Error, eventId,   message, args );
    public void Error( Exception?       exception, string?          message,   params object?[] args ) => _logger.Log( LogLevel.Error, exception, message, args );
    public void Error( string?          message,   params object?[] args ) => _logger.Log( LogLevel.Error,                                                             message, args );
    public void Information( EventId    eventId,   Exception?       exception, string?          message, params object?[] args ) => _logger.Log( LogLevel.Information, eventId, exception, message, args );
    public void Information( EventId    eventId,   string?          message,   params object?[] args ) => _logger.Log( LogLevel.Information, eventId,   message, args );
    public void Information( Exception? exception, string?          message,   params object?[] args ) => _logger.Log( LogLevel.Information, exception, message, args );
    public void Information( string?    message,   params object?[] args ) => _logger.Log( LogLevel.Information,                                                 message, args );
    public void Trace( EventId          eventId,   Exception?       exception, string?          message, params object?[] args ) => _logger.Log( LogLevel.Trace, eventId, exception, message, args );
    public void Trace( EventId          eventId,   string?          message,   params object?[] args ) => _logger.Log( LogLevel.Trace, eventId,   message, args );
    public void Trace( Exception?       exception, string?          message,   params object?[] args ) => _logger.Log( LogLevel.Trace, exception, message, args );
    public void Trace( string?          message,   params object?[] args ) => _logger.Log( LogLevel.Trace,                                                         message, args );
    public void Warning( EventId        eventId,   Exception?       exception, string?          message, params object?[] args ) => _logger.Log( LogLevel.Warning, eventId, exception, message, args );
    public void Warning( EventId        eventId,   string?          message,   params object?[] args ) => _logger.Log( LogLevel.Warning, eventId,   message, args );
    public void Warning( Exception?     exception, string?          message,   params object?[] args ) => _logger.Log( LogLevel.Warning, exception, message, args );
    public void Warning( string?        message,   params object?[] args ) => _logger.Log( LogLevel.Warning, message, args );
    public void Log<TState>( LogLevel              logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter ) => _logger.Log( logLevel, eventId, state, exception, formatter );
    public bool IsEnabled( LogLevel                logLevel ) => _logger.IsEnabled( logLevel );
    public IDisposable? BeginScope<TState>( TState state ) where TState : notnull => _logger.BeginScope( state );
}
