// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  8:45 AM

using System.Diagnostics.CodeAnalysis;
using Microsoft.AppCenter.Crashes;



namespace Jakar.AppLogger.Client;


[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ErrorReport : IDataBaseIDGuid
{
    public Guid                 ID             { get; init; }
    public DateTimeOffset       AppStartTime   { get; init; }
    public DateTimeOffset       AppErrorTime   { get; init; }
    public Device               Device         { get; init; }
    public string?              StackTrace     { get; init; }
    public AndroidErrorDetails? AndroidDetails { get; init; }
    public iOSErrorDetails?     iOSDetails     { get; init; }
    public MacOSErrorDetails?   MacOSDetails   { get; init; }


    /// <summary>Creates a new error report.</summary>
    /// <param name="log">The managed error log.</param>
    /// <param name="stackTrace">The associated exception stack trace.</param>
    public ErrorReport( ManagedErrorLog log, string stackTrace )
    {
        ID           = log.ID;
        AppStartTime = log.AppLaunchTimestamp;
        AppErrorTime = log.Timestamp;
        StackTrace   = stackTrace;
        StackTrace   = log.StackTrace;
        Device       = new Device(log.Device);
    }
}
