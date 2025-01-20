// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  14:01

using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;



namespace Jakar.Extensions.Serilog;


public interface ISerilogger : ILogger, ILoggerFactory, ILoggerProvider, ILogEventSink, ISupportExternalScope, Jakar.Extensions.IExceptionHandler, IValidator, IDeviceName, IDeviceID
{
    public DebugLogEvent.Collection Events   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public MessageEvent.Collection  Messages { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public bool                     Enabled  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public bool                     Disabled { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public Logger                   Logger   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }


    public Logger<T> CreateLogger<T>();
}



public interface ICreateSerilogger<out TSerilogger>
    where TSerilogger : class, ISerilogger
{
    public abstract static TSerilogger Create<TApp>( Activity activity, Logger logger, FilePaths paths, Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>>? takeScreenShot, Func<EventDetails, EventDetails>? updateEventDetails )
        where TApp : IAppID;
}
