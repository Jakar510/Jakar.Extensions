// Jakar.Extensions :: Jakar.Extensions
// 03/12/2025  17:03

using ZLinq;
using ZLinq.Internal;



namespace Jakar.Extensions;


[DefaultValue( nameof(Empty) )]
public struct FilterBuffer<TValue>( int capacity ) : IValueEnumerator<TValue>, IValueEnumerable<FilterBuffer<TValue>, TValue>
{
    public static readonly FilterBuffer<TValue> Empty = new(0);
    private readonly IMemoryOwner<TValue>? _owner = capacity > 0
                                                        ? MemoryPool<TValue>.Shared.Rent( capacity )
                                                        : null;
    public readonly int Capacity = capacity;
    internal        int length   = 0;
    private         int _index   = 0;


    public readonly int                    Length => length;
    public readonly ReadOnlyMemory<TValue> Memory => _owner?.Memory[..length] ?? Memory<TValue>.Empty;
    public readonly ReadOnlySpan<TValue>   Values => Memory.Span;


    public FilterBuffer() : this( 0 ) { }
    public readonly void Dispose() => _owner?.Dispose();
    public void Add( ref readonly TValue value )
    {
        if ( _owner is not null ) { _owner.Memory.Span[length++] = value; }

        if ( length > Capacity ) { throw new InvalidOperationException( "Cannot add more items than the capacity of the buffer." ); }
    }


    [Pure] public ValueEnumerable<FilterBuffer<TValue>, TValue> AsValueEnumerable() => new(this);
    public bool TryGetNext( out TValue current )
    {
        if ( _index < length )
        {
            current = Values[_index++];
            return true;
        }

        Unsafe.SkipInit( out current );
        return false;
    }
    public bool TryGetNonEnumeratedCount( out int count )
    {
        count = length;
        return false;
    }
    public bool TryGetSpan( out ReadOnlySpan<TValue> span )
    {
        span = Values;
        return false;
    }
    public bool TryCopyTo( scoped Span<TValue> destination, Index offset )
    {
        if ( !EnumeratorHelper.TryGetSlice(Values, offset, destination.Length, out ReadOnlySpan<TValue> slice) ) { return false; }

        slice.CopyTo( destination );
        return true;
    }
}
