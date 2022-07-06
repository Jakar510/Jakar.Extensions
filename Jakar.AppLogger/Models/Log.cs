// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  8:55 AM

namespace Jakar.AppLogger;


public class Log
{
    public DateTime Timestamp { get; init; }
    public Device Device { get; init; } = new();


    /// <summary>
    /// Gets or sets when tracking an analytics session, logs can be part of the session by specifying this identifier.
    /// This attribute is optional, a missing value means the session tracking is disabled (like when using only error reporting feature).
    /// Concrete types like StartSessionLog or PageLog are always part of a session and always include this identifier.
    ///  </summary>
    public Guid? SessionID { get; init; }


    /// <summary> Gets or sets optional string used for associating logs with users. </summary>
    public string? UserId { get; init; }


    public Log() { }

    /// <summary>Initializes a new instance of the Log class.</summary>
    /// <param name = "device" > </param>
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
    public Log(Device device, DateTime timestamp, Guid? sessionID = default, string? userId = default)
    {
        Timestamp = timestamp;
        SessionID = sessionID;
        UserId = userId;
        Device = device;
    }


    /// <summary>Validate the object.</summary>
    /// <exception cref="T:Microsoft.AppCenter.Ingestion.Models.ValidationException">
    /// Thrown if validation fails
    /// </exception>
    public virtual void Validate() { }
}
