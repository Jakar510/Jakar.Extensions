// Jakar.Extensions :: Jakar.Extensions
// 03/12/2025  17:03

using ZLinq;
using ZLinq.Internal;
using ZLinq.Linq;



namespace Jakar.Extensions;


[DefaultValue(nameof(Empty))]
[method: MustDisposeResource]
public struct ArrayBuffer<TValue>( int capacity ) : IValueEnumerator<TValue>
{
    public static readonly ArrayBuffer<TValue> Empty = new(0);
    private readonly TValue[]? __array = capacity > 0
                                             ? ArrayPool<TValue>.Shared.Rent(capacity)
                                             : null;
    public readonly int Capacity = capacity;
    internal        int length   = 0;
    private         int __index;


    public readonly int            Length => length;
    public readonly Memory<TValue> Memory => new(__array, 0, length);
    public readonly Span<TValue>   Values => new(__array, 0, length);


    public ArrayBuffer() : this(0) { }
    public void Dispose()
    {
        ArrayBuffer<TValue> self = this;
        this = Empty;
        if ( self.__array is not null ) { ArrayPool<TValue>.Shared.Return(self.__array); }
    }
    public readonly ReadOnlySpan<TValue>.Enumerator GetEnumerator() => new ReadOnlySpan<TValue>(__array, 0, length).GetEnumerator();


    public void Add( TValue value )
    {
        if ( __array is not null ) { __array[length++] = value; }

        if ( length > Capacity ) { throw new InvalidOperationException("Cannot add more items than the capacity of the buffer."); }
    }
    public void Add( ref readonly TValue value )
    {
        if ( __array is not null ) { __array[length++] = value; }

        if ( length > Capacity ) { throw new InvalidOperationException("Cannot add more items than the capacity of the buffer."); }
    }


    public bool TryGetNonEnumeratedCount( out int count )
    {
        count = Length;
        return true;
    }
    public bool TryGetNext( out TValue current )
    {
        if ( __index < Length )
        {
            current = Values[__index];
            __index++;
            return true;
        }

        Unsafe.SkipInit(out current);
        return false;
    }
    public bool TryGetSpan( out ReadOnlySpan<TValue> span )
    {
        span = Values;
        return true;
    }
    public bool TryCopyTo( Span<TValue> destination, Index offset )
    {
        if ( !EnumeratorHelper.TryGetSlice<TValue>(Values, offset, destination.Length, out ReadOnlySpan<TValue> slice) ) { return false; }

        slice.CopyTo(destination);
        return true;
    }
}
