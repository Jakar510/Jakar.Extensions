// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  9:02 AM

using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;
using Exception = System.Exception;



namespace Jakar.AppLogger;


public class ManagedErrorLog : Log, IDataBaseIDGuid
{
    /// <summary> Gets the numeric identifier for this event. </summary>
    public int EventId { get; init; }

    /// <summary> Gets the name of this event. </summary>
    public string? EventName { get; init; }


    /// <summary> Gets or sets unique ID for a Xamarin build or another similar technology. </summary>
    public string? BuildId { get; init; }


    /// <summary>
    /// Gets or sets if true, this error report is an application crash.
    /// Corresponds to the number of milliseconds elapsed between the time the error occurred and the app was launched.
    /// </summary>
    public bool Fatal { get; set; }


    public string            Message    { get; init; } = string.Empty;
    public LogLevel          Level      { get; init; }
    public ExceptionDetails? Exception  { get; init; }
    public ThreadInfo        ThreadInfo { get; init; } = new();
    public Device            Device     { get; init; } = new();

    public string? StackTrace => Exception?.StackTrace is not null
                                     ? string.Join('\n', Exception.StackTrace)
                                     : default;


    // public List<Binary>? Binaries  { get; init; }


    public ManagedErrorLog() { }
    public ManagedErrorLog( IAppLoggerConfig config, Exception? e, EventId eventId, string message, LogLevel level ) : base(config)
    {
        EventId            = eventId.Id;
        EventName          = eventId.Name;
        Message            = message;
        Level              = level;
        Exception          = e?.Details();
        Fatal              = level is LogLevel.Critical;
        ThreadInfo         = ThreadInfo.Create(e);
        AppLaunchTimestamp = config.AppLaunchTimestamp;
        Device             = config.Device;
    }


    // public ManagedErrorLog( Device        device,
    //                         Guid          id,
    //                         int           processId,
    //                         string        processName,
    //                         bool          fatal,
    //                         Exception     exception,
    //                         DateTime      timestamp          = default,
    //                         Guid?         sessionID          = default,
    //                         string?       userId             = default,
    //                         int?          parentProcessId    = default,
    //                         string?       parentProcessName  = default,
    //                         long?         errorThreadId      = default,
    //                         string?       errorThreadName    = default,
    //                         DateTime      appLaunchTimestamp = default,
    //                         string?       architecture       = default,
    //                         List<Binary>? binaries           = default,
    //                         string?       buildId            = default
    // ) : base(device,
    //          id,
    //          processId,
    //          processName,
    //          fatal,
    //          timestamp,
    //          sessionID,
    //          userId,
    //          parentProcessId,
    //          parentProcessName,
    //          errorThreadId,
    //          errorThreadName,
    //          appLaunchTimestamp,
    //          architecture)
    // {
    //     Binaries  = binaries;
    //     BuildId   = buildId;
    //     Exception = exception;
    // }


    // public override void Validate()
    // {
    //     base.Validate();
    //
    //     if ( Binaries is null ) { return; }
    //     
    //     foreach ( Binary binary in Binaries ) { binary.Validate(); }
    // }
}
