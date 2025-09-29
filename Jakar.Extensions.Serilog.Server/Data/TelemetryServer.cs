// Jakar.Extensions :: Jakar.Extensions.Telemetry.Server
// 06/25/2024  11:06

namespace Jakar.Extensions.Serilog.Server.Data;


public sealed class TelemetryServer : IAppName
{
    public static string     AppName    => nameof(TelemetryServer);
    public static AppVersion AppVersion { get; } = new(1, 0, 0);
}
