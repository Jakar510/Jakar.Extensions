﻿// Jakar.AppLogger :: Jakar.AppLogger.Client
// 10/03/2022  9:57 AM

namespace Jakar.AppLogger.Common;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IAppLogger : ILogger, ILoggerProvider, IHostedService, IAsyncDisposable
{
    public LoggingSettings  Config  { get; }
    public AppLoggerOptions Options { get; }


    public void TrackError( Exception e, EventId? eventId = default );
    public void TrackError( Exception e, EventId? eventId, IDictionary<string, JToken?>? eventDetails );
    public void TrackError( Exception e, EventId? eventId, params Attachment[]           attachments );
    public void TrackError( Exception e, EventId? eventId, IDictionary<string, JToken?>? eventDetails, params Attachment[]           attachments );
    public void TrackError( Exception e, EventId  eventId, IEnumerable<Attachment>       attachments,  IDictionary<string, JToken?>? eventDetails = default );


    public void TrackEvent<T>( LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default, [CallerMemberName] string? caller = default );

    public void TrackEvent<T>( T   _,      LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = default, [CallerMemberName] string? caller = default );
    public void TrackEvent( string source, LogLevel level = LogLevel.Trace, IDictionary<string, JToken?>? eventDetails = null );


    ValueTask<byte[]?> TryTakeScreenShot();
}
