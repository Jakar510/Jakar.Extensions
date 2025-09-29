// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/13/2024  20:08

namespace Jakar.SettingsView.Abstractions;


public interface ISvCellSaveValue<TValue> : ISvCellValue<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public ErrorOrResult<TValue> Save();
}
