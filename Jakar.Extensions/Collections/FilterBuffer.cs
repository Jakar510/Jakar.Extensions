// Jakar.Extensions :: Jakar.Extensions
// 03/12/2025  17:03

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public record struct FilterBuffer<TValue>( int Capacity ) : IDisposable
{
    private readonly IMemoryOwner<TValue>   _owner = MemoryPool<TValue>.Shared.Rent( Capacity );
    private          int                    _length;
    public           int                    Capacity                         { get; } = Capacity;
    public readonly  int                    Length                           => _length;
    public readonly  ReadOnlyMemory<TValue> Memory                           => _owner.Memory[.._length];
    public           void                   Dispose()                        => _owner.Dispose();
    public           void                   Add( ref readonly TValue value ) => _owner.Memory.Span[_length++] = value;
}
