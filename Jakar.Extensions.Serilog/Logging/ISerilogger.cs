// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  14:01

using Microsoft.Extensions.Logging;
using Serilog.Context;
using ILogger = Serilog.ILogger;



namespace Jakar.Extensions.Serilog;


public interface ISerilogger : ILogger, ILoggerFactory, ILoggerProvider, ILogEventSink, ISupportExternalScope, IExceptionHandler, IValidator, IDeviceName, IDeviceID
{
    public static ISerilogger?             Instance       { get; protected set; }
    public        DebugLogEvent.Collection Events         { get; }
    public        MessageEvent.Collection  Messages       { get; }
    public        Logger                   Logger         { get; }
    public        ISeriloggerSettings      Settings       { get; }
    public        ReadOnlyMemory<byte>     ScreenShotData { get; }


    public Logger<T> CreateLogger<T>();
    public void      SetDeviceID( Guid deviceID );
    public void      SetDeviceID( long deviceID );


    public void TrackEvent<T>( T _, [CallerMemberName] string caller                                                                             = BaseRecord.EMPTY );
    public void TrackEvent<T>( T _, EventDetails              properties, [CallerMemberName] string caller                                       = BaseRecord.EMPTY );
    public void TrackEvent<T>( T _, string                    eventType,  EventDetails?             properties, [CallerMemberName] string caller = BaseRecord.EMPTY );

    public void TrackError<T>( T _, Exception exception, EventDetails?             details, IEnumerable<FileData>? attachments, [CallerMemberName] string caller = BaseRecord.EMPTY );
    public void TrackError<T>( T _, Exception exception, [CallerMemberName] string caller = BaseRecord.EMPTY );


    public ValueTask SaveFeedBackAppState( Dictionary<string, string> feedback, string key = "feedback" );

    public EventDetails AppState();


    public ValueTask<bool>       BufferScreenShot( CancellationToken   token                           = default );
    public ValueTask<LocalFile?> GetScreenShot( CancellationToken      token                           = default );
    public ValueTask<LocalFile?> WriteScreenShot( CancellationToken    token                           = default );
    public ValueTask<LocalFile?> WriteScreenShot( ReadOnlyMemory<byte> memory, CancellationToken token = default );
}



public interface ICreateSerilogger<out TSerilogger>
    where TSerilogger : class, ISerilogger
{
    public abstract static ILoggerProvider GetProvider( IServiceProvider   provider );
    public abstract static TSerilogger     Get( IServiceProvider           provider );
    public abstract static TSerilogger     Create( IServiceProvider        provider );
    public abstract static TSerilogger     Create( SeriloggerOptions options );
}
