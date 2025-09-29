// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2025  16:29

namespace Jakar.Extensions.Telemetry;


[Serializable, StructLayout(LayoutKind.Auto)]
public readonly struct TelemetryActivityLink( in TelemetryActivityContext context, in Pairs tags )
{
    public readonly TelemetryActivityContext Context = context;
    public readonly Pairs                    Tags    = tags;
    public TelemetryActivityLink( in TelemetryActivityContext context ) : this(context, in Pairs.Empty) { }
}
