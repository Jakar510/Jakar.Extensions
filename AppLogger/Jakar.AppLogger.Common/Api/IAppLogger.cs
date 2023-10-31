// Jakar.AppLogger :: Jakar.AppLogger.Client
// 10/03/2022  9:57 AM

using Microsoft.Extensions.Hosting;



namespace Jakar.AppLogger.Common;


[ SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" ) ]
public interface IAppLogger : ILogger, ILoggerProvider, IHostedService, IAsyncDisposable
{
    public LoggingSettings  Config  { get; }
    public AppLoggerOptions Options { get; }


    public void TrackError( Exception e, EventID? eventID = default );
    public void TrackError( Exception e, EventID? eventID, IDictionary<string, JToken?>? eventDetails );
    public void TrackError( Exception e, EventID? eventID, params LoggerAttachment[]     attachments );
    public void TrackError( Exception e, EventID? eventID, IDictionary<string, JToken?>? eventDetails, params LoggerAttachment[]     attachments );
    public void TrackError( Exception e, EventID  eventID, IEnumerable<LoggerAttachment> attachments,  IDictionary<string, JToken?>? eventDetails = default );


    public void TrackEvent<T>( LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default, [ CallerMemberName ] string? caller = default );


    public void TrackEvent<T>( T   _,      LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default, [ CallerMemberName ] string? caller = default );
    public void TrackEvent( string source, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null );


    ValueTask<byte[]?> TryTakeScreenShot();


    public void Debug( EventID    eventID,   Exception?       exception, string?          message, params object?[] args );
    public void Debug( EventID    eventID,   string?          message,   params object?[] args );
    public void Debug( Exception? exception, string?          message,   params object?[] args );
    public void Debug( string?    message,   params object?[] args );


    public void Trace( EventID    eventID,   Exception?       exception, string?          message, params object?[] args );
    public void Trace( EventID    eventID,   string?          message,   params object?[] args );
    public void Trace( Exception? exception, string?          message,   params object?[] args );
    public void Trace( string?    message,   params object?[] args );


    public void Information( EventID    eventID,   Exception?       exception, string?          message, params object?[] args );
    public void Information( EventID    eventID,   string?          message,   params object?[] args );
    public void Information( Exception? exception, string?          message,   params object?[] args );
    public void Information( string?    message,   params object?[] args );


    public void Warning( EventID    eventID,   Exception?       exception, string?          message, params object?[] args );
    public void Warning( EventID    eventID,   string?          message,   params object?[] args );
    public void Warning( Exception? exception, string?          message,   params object?[] args );
    public void Warning( string?    message,   params object?[] args );


    public void Error( EventID    eventID,   Exception?       exception, string?          message, params object?[] args );
    public void Error( EventID    eventID,   string?          message,   params object?[] args );
    public void Error( Exception? exception, string?          message,   params object?[] args );
    public void Error( string?    message,   params object?[] args );


    public void Critical( EventID    eventID,   Exception?       exception, string?          message, params object?[] args );
    public void Critical( EventID    eventID,   string?          message,   params object?[] args );
    public void Critical( Exception? exception, string?          message,   params object?[] args );
    public void Critical( string?    message,   params object?[] args );
}
