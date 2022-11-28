﻿// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/24/2022  9:32 PM

namespace Jakar.AppLogger.Common;


public interface ILog : IUniqueID<long>, IValidator
{
    bool IsError { get; }


    /// <summary>
    ///     Gets or sets if true, this error report is an application crash.
    ///     Corresponds to the number of milliseconds elapsed between the time the error occurred and the app was launched.
    /// </summary>
    public bool IsFatal { get; }

    public DateTimeOffset AppErrorTime       { get; }
    public DateTimeOffset AppLaunchTimestamp { get; }


    public DateTimeOffset AppStartTime { get; }
    public DateTimeOffset Timestamp    { get; }


    /// <summary>
    ///     Gets or sets when tracking an analytics session, logs can be part of the session by specifying this identifier.
    ///     <para> This attribute is optional, a missing value means the session tracking is disabled (like when using only error reporting feature). </para>
    /// </summary>
    public Guid SessionID { get; }

    public Guid? ScopeID { get; }


    /// <summary> Gets the numeric identifier for this event. </summary>
    public int EventID { get; }

    public LogLevel Level   { get; }
    public string   Message { get; }


    /// <summary> An optional string used for associating logs with users. </summary>
    public string? AppUserID { get; }


    /// <summary> Gets or sets unique ID for a Xamarin/Maui build or another similar technology. </summary>
    public string? BuildID { get; }


    /// <summary> Gets the name of this event. </summary>
    public string? EventName { get; }

    public string? StackTrace { get; }
}
