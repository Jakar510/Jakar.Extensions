// TrueLogic :: TrueLogic.Models
// 07/29/2025  09:23

using Serilog.Core;



namespace Jakar.Extensions;


public interface IOpenTelemetryActivityEnricher
{
    DeviceInfo                       DeviceInfo { get; }
    ref readonly AppInfo             Info       { get; }
    public       ILogEventEnricher[] Enrichers  { get; }
}
