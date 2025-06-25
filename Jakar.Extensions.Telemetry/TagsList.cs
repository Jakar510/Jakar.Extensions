// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2025  16:28

using ZLinq;
using ZLinq.Internal;



namespace Jakar.Extensions.Telemetry;


[StructLayout( LayoutKind.Auto )]
[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
public struct TagsList( int capacity ) : IEnumerable<Pair>, IValueEnumerable<TagsList.Enumerator, Pair>
{
    public static readonly TagsList Empty = new(0);
    private readonly IMemoryOwner<Pair>? _owner = capacity > 0
                                                      ? MemoryPool<Pair>.Shared.Rent( capacity )
                                                      : null;
    public readonly int Capacity = capacity;
    private         int _length  = 0;


    public            int                Length { get => _length; }
    internal readonly Memory<Pair>       Memory => _owner?.Memory ?? Memory<Pair>.Empty;
    internal readonly Span<Pair>         Span   => Memory.Span[..Capacity];
    public readonly   ReadOnlySpan<Pair> Values => Span[.._length];
    public ref Pair this[ string key ]
    {
        get
        {
            for ( int i = 0; i < Length; i++ )
            {
                ref Pair pair = ref Span[i];
                if ( string.Equals( pair.Key, key, StringComparison.Ordinal ) ) { return ref pair; }
            }

            return ref Span[_length++];
        }
    }


    public TagsList() : this( Buffers.DEFAULT_CAPACITY ) { }
    public readonly void Dispose() => _owner?.Dispose();


    public TagsList SetTag( string key, string? value )
    {
        ref Pair pair = ref this[key];
        if ( pair.Value != value ) { pair = new Pair( key, value ); }

        return this;
    }
    public TagsList Add( in Pair value )
    {
        if ( _owner is not null ) { _owner.Memory.Span[_length++] = value; }

        if ( _length > Capacity ) { throw new InvalidOperationException( "Cannot add more items than the capacity of the buffer." ); }

        return this;
    }


    public static TagsList Create( params ReadOnlySpan<Pair> pairs ) => new() { pairs };
    public TagsList Add( params ReadOnlySpan<Pair> pairs )
    {
        foreach ( ref readonly Pair pair in pairs ) { Add( in pair ); }

        return this;
    }


    [Pure] public ValueEnumerable<Enumerator, Pair> AsValueEnumerable() => new(new Enumerator( this ));


    public IEnumerator<Pair> GetEnumerator()
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        // ReSharper disable once LoopCanBeConvertedToQuery
        for ( int i = 0; i < _length; i++ ) { yield return Values[i]; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    [StructLayout( LayoutKind.Auto )]
    public struct Enumerator( TagsList tags ) : IValueEnumerator<Pair>
    {
        private TagsList tags = tags;
        private int      index;

        public bool TryGetNonEnumeratedCount( out int count )
        {
            count = tags._length;
            return true;
        }

        public bool TryGetSpan( out ReadOnlySpan<Pair> span )
        {
            span = tags.Values;
            return true;
        }

        public bool TryCopyTo( Span<Pair> destination, Index offset )
        {
            if ( EnumeratorHelper.TryGetSlice( tags.Values, offset, destination.Length, out ReadOnlySpan<Pair> slice ) )
            {
                slice.CopyTo( destination );
                return true;
            }

            return false;
        }

        public bool TryGetNext( out Pair current )
        {
            ReadOnlySpan<Pair> values = tags.Values;

            if ( index < values.Length )
            {
                current = values[index];
                index++;
                return true;
            }

            Unsafe.SkipInit( out current );
            return false;
        }

        public void Dispose() => tags = Empty;
    }
}
