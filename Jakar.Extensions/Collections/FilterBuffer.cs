// Jakar.Extensions :: Jakar.Extensions
// 03/12/2025  17:03

namespace Jakar.Extensions;


public struct FilterBuffer<TValue>( int capacity ) : IDisposable
{
    private readonly IMemoryOwner<TValue> _owner   = MemoryPool<TValue>.Shared.Rent( capacity );
    public readonly  int                  Capacity = capacity;
    internal         int                  length;


    public readonly int                    Length => length;
    public readonly ReadOnlyMemory<TValue> Memory => _owner.Memory[..length];
    public readonly ReadOnlySpan<TValue>   Values => Memory.Span;


    public void Dispose()                        => _owner.Dispose();
    public void Add( ref readonly TValue value ) => _owner.Memory.Span[length++] = value;
}
