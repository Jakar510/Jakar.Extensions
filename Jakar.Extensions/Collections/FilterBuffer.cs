// Jakar.Extensions :: Jakar.Extensions
// 03/12/2025  17:03

using ZLinq;
using ZLinq.Internal;
using ZLinq.Linq;



namespace Jakar.Extensions;


[StructLayout(LayoutKind.Auto)]
[DefaultValue(nameof(Empty))]
[method: MustDisposeResource]
public struct ArrayBuffer<TValue>( int capacity ) : IValueEnumerator<TValue>
{
    public static readonly ArrayBuffer<TValue> Empty = new(0);
    private readonly TValue[]? __array = capacity > 0
                                             ? ArrayPool<TValue>.Shared.Rent(capacity)
                                             : null;
    public readonly int Capacity = capacity;
    private         int length   = 0;
    private         int __index;


    public readonly int                  Length => length;
    public readonly Memory<TValue>       Memory => new(__array, 0, length);
    public readonly Span<TValue>         Span   => new(__array, 0, Capacity);
    public readonly ReadOnlySpan<TValue> Values => Span[..length];

    public readonly ArraySegment<TValue> Array => __array is not null
                                                      ? new ArraySegment<TValue>(__array, 0, length)
                                                      : ArraySegment<TValue>.Empty;


    public ArrayBuffer() : this(0) { }
    public ArrayBuffer( IEnumerable<TValue> span, int count ) : this(count)
    {
        foreach ( TValue value in span ) { Add(value); }
    }
    public ArrayBuffer( params ReadOnlySpan<TValue> span ) : this(span.Length)
    {
        foreach ( TValue value in span ) { Add(value); }
    }
    public void Dispose()
    {
        ArrayBuffer<TValue> self = this;
        this = Empty;
        if ( self.__array is not null ) { ArrayPool<TValue>.Shared.Return(self.__array); }
    }


    public static implicit operator ReadOnlySpan<TValue>( ArrayBuffer<TValue>   self ) => self.Values;
    public static implicit operator ArrayBuffer<TValue>( Memory<TValue>         self ) => new(self.Span);
    public static implicit operator ArrayBuffer<TValue>( ReadOnlyMemory<TValue> self ) => new(self.Span);
    public static implicit operator ArrayBuffer<TValue>( PooledArray<TValue>    self ) => new(self.Span);
    public static implicit operator ArrayBuffer<TValue>( ImmutableArray<TValue> self ) => new(self.AsSpan());
    public static implicit operator ArrayBuffer<TValue>( List<TValue>           self ) => new(self.AsSpan());
    public static ArrayBuffer<TValue> Create( IEnumerable<TValue> self )
    {
        return self switch
               {
                   IReadOnlyCollection<TValue> readOnlyCollection => new ArrayBuffer<TValue>(self, readOnlyCollection.Count),
                   ICollection<TValue> readOnlyCollection         => new ArrayBuffer<TValue>(self, readOnlyCollection.Count),
                   _                                              => self.ToList()
               };
    }


    public readonly ReadOnlySpan<TValue>.Enumerator GetEnumerator() => Values.GetEnumerator();


    public void Add( TValue              value ) => Span[length++] = value;
    public void Add( ref readonly TValue value ) => Span[length++] = value;


    public bool TryGetNonEnumeratedCount( out int count )
    {
        count = Length;
        return true;
    }
    public bool TryGetNext( out TValue current )
    {
        if ( __index < Length )
        {
            current = Span[__index++];
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
        if ( !EnumeratorHelper.TryGetSlice(Values, offset, destination.Length, out ReadOnlySpan<TValue> slice) ) { return false; }

        slice.CopyTo(destination);
        return true;
    }
}
