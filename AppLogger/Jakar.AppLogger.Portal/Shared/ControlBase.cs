// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/23/2022  6:04 PM

namespace Jakar.AppLogger.Portal.Shared;


public abstract class ControlBase : Widget
{
    [Inject] public LoggerDB Api { get; set; } = default!;
}
