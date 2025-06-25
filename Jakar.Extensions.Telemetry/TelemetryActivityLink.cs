// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2025  16:29

namespace Jakar.Extensions.Telemetry;


[Serializable, StructLayout( LayoutKind.Auto )]
public readonly struct TelemetryActivityLink( in TelemetryActivityContext context, in TagsList tags )
{
    public readonly TelemetryActivityContext Context = context;
    public readonly TagsList                 Tags    = tags;
}
