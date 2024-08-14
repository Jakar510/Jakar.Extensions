// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/13/2024  20:08

namespace Jakar.SettingsView.Abstractions;


public interface ISvCellSaveValue<T> : ISvCellValue<T>
    where T : IEquatable<T>, IComparable<T>
{
    public ErrorOrResult<T> Save();
}
