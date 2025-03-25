// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/31/2024  22:08

namespace Jakar.SettingsView.Abstractions;


public sealed class ChangedEventArgs<TValue>( TValue? oldValue, TValue? newValue ) : EventArgs
{
    public TValue? NewValue { get; } = newValue;
    public TValue? OldValue { get; } = oldValue;
}
