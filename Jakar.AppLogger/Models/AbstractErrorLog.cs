// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  9:03 AM

using Microsoft.AppCenter.Ingestion.Models;



namespace Jakar.AppLogger;


/// <summary>Abstract error log.</summary>
public class AbstractErrorLog : Log, IDataBaseIDGuid
{
    public Guid ID { get; init; }
    public int ProcessId { get; set; }
    public string? ProcessName { get; set; }
    public int? ParentProcessId { get; set; }
    public string? ParentProcessName { get; set; }
    public long? ErrorThreadId { get; set; }
    public string? ErrorThreadName { get; set; }
    public DateTime AppLaunchTimestamp { get; set; }
    public string? Architecture { get; set; }


    /// <summary>
    /// Gets or sets if true, this error report is an application crash.
    /// Corresponds to the number of milliseconds elapsed between the time the error occurred and the app was launched.
    /// </summary>
    public bool Fatal { get; set; }


    /// <summary>
    /// Initializes a new instance of the AbstractErrorLog class.
    /// </summary>
    public AbstractErrorLog() { }

    /// <summary>
    /// Initializes a new instance of the AbstractErrorLog class.
    /// </summary>
    /// <param name = "device" > </param>
    /// <param name="id">Error identifier.</param>
    /// <param name="processId">Process identifier.</param>
    /// <param name="processName">Process name.</param>
    /// <param name="fatal">If true, this error report is an application
    /// crash.
    /// Corresponds to the number of milliseconds elapsed between the time
    /// the error occurred and the app was launched.</param>
    /// <param name="timestamp">Log timestamp, example:
    /// '2017-03-13T18:05:42Z'.
    /// </param>
    /// <param name="sessionID">When tracking an analytics session, logs can be
    /// part of the session by specifying this identifier.
    /// This attribute is optional, a missing value means the session
    /// tracking is disabled (like when using only error reporting
    /// feature).
    /// Concrete types like StartSessionLog or PageLog are always part of a
    /// session and always include this identifier.
    /// </param>
    /// <param name="userId">optional string used for associating logs with
    /// users.
    /// </param>
    /// <param name="parentProcessId">Parent's process identifier.</param>
    /// <param name="parentProcessName">Parent's process name.</param>
    /// <param name="errorThreadId">Error thread identifier.</param>
    /// <param name="errorThreadName">Error thread name.</param>
    /// <param name="appLaunchTimestamp">Timestamp when the app was
    /// launched, example: '2017-03-13T18:05:42Z'.
    /// </param>
    /// <param name="architecture">CPU architecture.</param>
    public AbstractErrorLog(Device device,
                             Guid id,
                             int processId,
                             string processName,
                             bool fatal,
                             DateTime timestamp = default,
                             Guid? sessionID = default,
                             string? userId = default,
                             int? parentProcessId = default,
                             string? parentProcessName = default,
                             long? errorThreadId = default,
                             string? errorThreadName = default,
                             DateTime appLaunchTimestamp = default,
                             string? architecture = default
    ) : base(device, timestamp, sessionID, userId)
    {
        ID = id;
        ProcessId = processId;
        ProcessName = processName;
        ParentProcessId = parentProcessId;
        ParentProcessName = parentProcessName;
        ErrorThreadId = errorThreadId;
        ErrorThreadName = errorThreadName;
        Fatal = fatal;
        AppLaunchTimestamp = appLaunchTimestamp;
        Architecture = architecture;
    }


    /// <summary>Validate the object.</summary>
    /// <exception cref="T:Microsoft.AppCenter.Ingestion.Models.ValidationException">
    /// Thrown if validation fails
    /// </exception>
    public override void Validate()
    {
        base.Validate();
        if (ProcessName == default)
        {
            throw new ValidationException(0, "ProcessName", default);
        }
    }
}
