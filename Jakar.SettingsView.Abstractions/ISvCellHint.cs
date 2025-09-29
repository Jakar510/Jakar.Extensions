// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/13/2024  20:08

namespace Jakar.SettingsView.Abstractions;


public interface ISvCellHint : ISvCellDescription
{
    public string? Hint { get; set; }
}
