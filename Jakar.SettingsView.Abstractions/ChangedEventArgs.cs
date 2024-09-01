// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/31/2024  22:08

namespace Jakar.SettingsView.Abstractions;


public sealed class ChangedEventArgs<T>( T? oldValue, T? newValue ) : EventArgs
{
    public T? NewValue { get; } = newValue;
    public T? OldValue { get; } = oldValue;
}
