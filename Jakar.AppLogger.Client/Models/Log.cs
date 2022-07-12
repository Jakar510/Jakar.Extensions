// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  8:55 AM

namespace Jakar.AppLogger.Client;


public class Log
{
    public Guid           ID                 { get; init; } = Guid.Empty;
    public DateTime       Timestamp          { get; init; } = DateTime.UtcNow;
    public DateTimeOffset AppLaunchTimestamp { get; init; }


    /// <summary>
    /// Gets or sets when tracking an analytics session, logs can be part of the session by specifying this identifier.
    /// This attribute is optional, a missing value means the session tracking is disabled (like when using only error reporting feature).
    /// Concrete types like StartSessionLog or PageLog are always part of a session and always include this identifier.
    ///  </summary>
    public Guid? SessionID { get; init; }


    /// <summary> Gets or sets optional string used for associating logs with users. </summary>
    public string? UserID { get; init; }


    public Log() { }
    public Log( IAppLoggerConfig config, Guid? id = default )
    {
        AppLaunchTimestamp = config.AppLaunchTimestamp;
        UserID             = config.UserID;
        SessionID          = config.SessionID;
        ID                 = id ?? Guid.NewGuid();
    }


    /// <summary>Validate the object.</summary>
    /// <exception cref="T:Microsoft.AppCenter.Ingestion.Models.ValidationException">
    /// Thrown if validation fails
    /// </exception>
    public virtual void Validate() { }
}
