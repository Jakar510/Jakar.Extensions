// TrueLogic :: TrueLogic.Models
// 07/29/2025  09:23

using Serilog.Core;



namespace Jakar.Extensions;


public interface IOpenTelemetryActivityEnricher
{
    DeviceInformation                DeviceInfo { get; }
    public       ILogEventEnricher[] Enrichers  { get; }
    ref readonly AppInformation      Info       { get; }
}
