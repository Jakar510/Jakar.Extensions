// Jakar.Extensions :: Jakar.Extensions
// 05/23/2024  11:05

namespace Jakar.Extensions;


/// <summary> An empty scope without any logic </summary>
public sealed class NullScope : IDisposable
{
    public static NullScope Instance { get; } = new();

    private NullScope() { }

    /// <inheritdoc/>
    public void Dispose() { }
}
