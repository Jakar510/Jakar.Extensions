// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 03/14/2025  15:03

using Serilog.Core;
using Serilog.Events;



namespace Jakar.Extensions.Telemetry.Meters;


public sealed class RuntimeMetricsEnricher : ILogEventEnricher
{
    public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory ) { }
}
