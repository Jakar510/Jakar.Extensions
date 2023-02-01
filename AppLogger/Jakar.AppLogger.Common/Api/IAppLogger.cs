// Jakar.AppLogger :: Jakar.AppLogger.Client
// 10/03/2022  9:57 AM

namespace Jakar.AppLogger.Common;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IAppLogger : ILoggerProvider, IHostedService, ILogger, IAsyncDisposable
{
    public AppLoggerOptions Options { get; }
    public LoggingSettings  Config  { get; }


    public void TrackError( Exception exception );
    public void TrackError( Exception exception, IDictionary<string, JToken?>? eventDetails );
    public void TrackError( Exception exception, params Attachment[]           attachments );
    public void TrackError( Exception exception, IDictionary<string, JToken?>? eventDetails, params Attachment[] attachments );
    public void TrackError( Exception exception, IEnumerable<Attachment>       attachments );
    public void TrackError( Exception exception, IDictionary<string, JToken?>? eventDetails, IEnumerable<Attachment> attachments );


    public void TrackEvent<T>( LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default,        [CallerMemberName] string?    caller       = default );
    public void TrackEvent<T>( T        _,                      LogLevel                      level        = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default, [CallerMemberName] string? caller = default );
    public void TrackEvent( string?     source,                 LogLevel                      level        = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null );


    ValueTask<byte[]?> TryTakeScreenShot();
}
