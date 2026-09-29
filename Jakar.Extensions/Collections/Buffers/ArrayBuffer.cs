// Jakar.Extensions :: Jakar.Extensions
// 03/12/2025  17:03

using ZLinq;
using ZLinq.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Jakar.Extensions;


[StructLayout(LayoutKind.Auto)]
[DefaultValue(nameof(Empty))]
[method: MustDisposeResource]
public struct ArrayBuffer<TValue>( int capacity ) : IReadOnlyCollection<TValue>, IValueEnumerator<TValue>
{
    public static readonly ArrayBuffer<TValue> Empty = new(0);
    private readonly TValue[]? __array = capacity > 0
                                             ? ArrayPool<TValue>.Shared.Rent(capacity)
                                             : null;
    public readonly int Capacity = capacity;
    private         int __index;


    public          int                  Length { get; internal set; } = 0;
    public readonly Memory<TValue>       Memory => new(__array, 0, Length);
    public readonly Span<TValue>         Span   => new(__array, 0, Capacity);
    public readonly ReadOnlySpan<TValue> Values => Span[..Length];
    public ref readonly TValue this[ int index ] => ref Values[index];
    int IReadOnlyCollection<TValue>.Count => Length;

    public readonly ArraySegment<TValue> Array => __array is not null
                                                      ? new ArraySegment<TValue>(__array, 0, Length)
                                                      : ArraySegment<TValue>.Empty;


    public ArrayBuffer() : this(0) { }
    [MustDisposeResource] public ArrayBuffer( IEnumerable<TValue> span, int count ) : this(count)
    {
        foreach ( TValue value in span ) { Add(value); }
    }
    [MustDisposeResource] public ArrayBuffer( params ReadOnlySpan<TValue> span ) : this(span.Length) => Add(span);
    public void Dispose()
    {
        ArrayBuffer<TValue> self = this;
        this = Empty;
        if ( self.__array is not null ) { ArrayPool<TValue>.Shared.Return(self.__array); }
    }


    public static implicit operator                       ReadOnlySpan<TValue>( ArrayBuffer<TValue>   self ) => self.Values;
    [MustDisposeResource] public static implicit operator ArrayBuffer<TValue>( Memory<TValue>         self ) => new(self.Span);
    [MustDisposeResource] public static implicit operator ArrayBuffer<TValue>( ReadOnlyMemory<TValue> self ) => new(self.Span);
    [MustDisposeResource] public static implicit operator ArrayBuffer<TValue>( PooledArray<TValue>    self ) => new(self.Span);
    [MustDisposeResource] public static implicit operator ArrayBuffer<TValue>( TValue[]               self ) => new(self.AsSpan());
    [MustDisposeResource] public static implicit operator ArrayBuffer<TValue>( ImmutableArray<TValue> self ) => new(self.AsSpan());
    [MustDisposeResource] public static implicit operator ArrayBuffer<TValue>( List<TValue>           self ) => new(self.AsSpan());


    [Pure] [MustDisposeResource] public static ArrayBuffer<TValue> Create( IEnumerable<TValue> self ) => Create(self.AsValueEnumerable());
    [Pure] [MustDisposeResource] public static ArrayBuffer<TValue> Create<TEnumerator>( ValueEnumerable<TEnumerator, TValue> source )
        where TEnumerator : struct, IValueEnumerator<TValue>, allows ref struct
    {
        using TEnumerator enumerator = source.Enumerator;

        if ( enumerator.TryGetNonEnumeratedCount(out int count) )
        {
            ArrayBuffer<TValue> array = new(count);
            while ( enumerator.TryGetNext(out TValue item) ) { array.Add(item); }

            return array;
        }


        Buffer<TValue> buffer = new(64);
        if ( enumerator.TryCopyTo(buffer.Span, 0) ) { return new ArrayBuffer<TValue>(buffer.Span); }

        while ( enumerator.TryGetNext(out TValue item) ) { buffer.Add(item); }

        return buffer.ToArrayBuffer();
    }


    [HandlesResourceDisposal] public TValue[] ToArray()
    {
        TValue[] array = Values.ToArray();
        Dispose();
        return array;
    }
    public ValueEnumerable<ArrayBuffer<TValue>, TValue> AsValueEnumerable() => new(this);
    IEnumerator IEnumerable.                            GetEnumerator()     => ( (IEnumerable<TValue>)this ).GetEnumerator();
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()

    {
        for ( int i = 0; i < Length; i++ ) { yield return Values[i]; }
    }

    public readonly ReadOnlySpan<TValue>.Enumerator GetEnumerator() => Values.GetEnumerator();


    public void Add( TValue              value ) => Span[Length++] = value;
    public void Add( ref readonly TValue value ) => Span[Length++] = value;

    public void Add( params ReadOnlySpan<TValue> span )
    {
        if ( span.TryCopyTo(Span[Length..]) ) { Length += span.Length; }
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
