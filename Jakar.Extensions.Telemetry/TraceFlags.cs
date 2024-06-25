// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  18:06

using System;



namespace Jakar.Extensions.Telemetry;


/// <summary> Specifies flags defined by the W3C standard that are associated with an activity. </summary>
[Flags]
public enum TraceFlags
{
    /// <summary> The activity has not been marked. </summary>
    None = 0,
    /// <summary> The activity (or more likely its parents) has been marked as useful to record. </summary>
    Recorded = 1
}
