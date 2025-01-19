// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  14:01

using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;



namespace Jakar.Extensions.Serilog;


public interface ISerilogger : ILogger, ILoggerFactory, ILoggerProvider, ILogEventSink, ISupportExternalScope, Jakar.Extensions.IExceptionHandler, IValidator, IDeviceName, IDeviceID
{
    public FilePaths                Paths    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public DebugLogEvent.Collection Events   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public MessageEvent.Collection  Messages { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public bool                     Enabled  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public bool                     Disabled { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public Logger                   Logger   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }


    public Logger<T> CreateLogger<T>();
}
