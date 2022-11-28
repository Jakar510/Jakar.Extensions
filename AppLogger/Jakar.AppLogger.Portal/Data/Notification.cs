// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/26/2022  2:52 PM

namespace Jakar.AppLogger.Portal.Data;


public sealed record Notification : ObservableRecord
{
    public string Message { get; init; } = string.Empty;
    public string Type    { get; init; } = string.Empty;


    public Notification() { }
}
