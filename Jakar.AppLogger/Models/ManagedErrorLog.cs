// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  9:02 AM

using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;
using Exception = System.Exception;



namespace Jakar.AppLogger;


public class ManagedErrorLog : AbstractErrorLog
{
    /// <summary> Gets or sets unique ID for a Xamarin build or another similar technology. </summary>
    public string? BuildId { get; init; }

    public List<Binary>? Binaries  { get; init; }
    public Exception     Exception { get; init; } = new();


    public ManagedErrorLog() { }
    public ManagedErrorLog( Device        device,
                            Guid          id,
                            int           processId,
                            string        processName,
                            bool          fatal,
                            Exception     exception,
                            DateTime      timestamp          = default,
                            Guid?         sessionID          = default,
                            string?       userId             = default,
                            int?          parentProcessId    = default,
                            string?       parentProcessName  = default,
                            long?         errorThreadId      = default,
                            string?       errorThreadName    = default,
                            DateTime      appLaunchTimestamp = default,
                            string?       architecture       = default,
                            List<Binary>? binaries           = default,
                            string?       buildId            = default
    ) : base(device,
             id,
             processId,
             processName,
             fatal,
             timestamp,
             sessionID,
             userId,
             parentProcessId,
             parentProcessName,
             errorThreadId,
             errorThreadName,
             appLaunchTimestamp,
             architecture)
    {
        Binaries  = binaries;
        BuildId   = buildId;
        Exception = exception;
    }


    public override void Validate()
    {
        base.Validate();

        // Exception.Validate();


        if ( Binaries is null ) { return; }

        foreach ( Binary binary in Binaries ) { binary.Validate(); }
    }
}
