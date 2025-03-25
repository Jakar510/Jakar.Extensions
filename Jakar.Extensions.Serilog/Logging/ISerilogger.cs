// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  14:01

using Microsoft.Extensions.Logging;
using Serilog.Context;
using ILogger = Serilog.ILogger;



namespace Jakar.Extensions.Serilog;


public interface ISerilogger : ILogger, ILogEventSink, IExceptionHandler, IValidator, IDeviceName, IDeviceID // ILoggerFactory, ILoggerProvider, ISupportExternalScope
{
    public static ISerilogger?             Instance       { get; protected set; }
    public        DebugLogEvent.Collection Events         { get; }
    public        MessageEvent.Collection  Messages       { get; }
    public        Logger                   Logger         { get; }
    public        ISeriloggerSettings      Settings       { get; }
    public        FilePaths                Paths          { get; }
    public        ReadOnlyMemory<byte>     ScreenShotData { get; }


    public void SetDeviceID( Guid deviceID );
    public void SetDeviceID( long deviceID );


    public void TrackEvent<TValue>( TValue _, [CallerMemberName] string caller                                                                             = BaseRecord.EMPTY );
    public void TrackEvent<TValue>( TValue _, EventDetails              properties, [CallerMemberName] string caller                                       = BaseRecord.EMPTY );
    public void TrackEvent<TValue>( TValue _, string                    eventType,  EventDetails?             properties, [CallerMemberName] string caller = BaseRecord.EMPTY );

    public void TrackError<TValue>( TValue _, Exception exception, EventDetails?             details, IEnumerable<FileData>? attachments, [CallerMemberName] string caller = BaseRecord.EMPTY );
    public void TrackError<TValue>( TValue _, Exception exception, [CallerMemberName] string caller = BaseRecord.EMPTY );


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
    public abstract static ILoggerProvider GetProvider( IServiceProvider provider );
    public abstract static TSerilogger     Get( IServiceProvider         provider );
    public abstract static TSerilogger     Create( IServiceProvider      provider );
    public abstract static TSerilogger     Create( SeriloggerOptions     options );
}
