// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2025  16:28

namespace Jakar.Extensions.Telemetry;


[DefaultValue(nameof(Empty))][Serializable][StructLayout(LayoutKind.Auto)]
public readonly record struct Pair( string Key, string? Value ) : IEqualityOperators<Pair>, IComparisonOperators<Pair>
{
    public static readonly Pair    Empty = new(string.Empty, string.Empty);
    public readonly        string  Key   = Key;
    public readonly        string? Value = Value;


    public static implicit operator KeyValuePair<string, string?>( Pair tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, string? Value)( Pair   tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair( KeyValuePair<string, string?> tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair( (string Key, string? Value)   tag ) => new(tag.Key, tag.Value);


    public int CompareTo( Pair other )
    {
        int keyComparison = string.Compare(Key, other.Key, StringComparison.Ordinal);
        if ( keyComparison != 0 ) { return keyComparison; }

        return string.Compare(Value, other.Value, StringComparison.Ordinal);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is Pair other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(Pair)}");
    }


    public static bool operator >( Pair  left, Pair right ) => left.CompareTo(right) > 0;
    public static bool operator >=( Pair left, Pair right ) => left.CompareTo(right) >= 0;
    public static bool operator <( Pair  left, Pair right ) => left.CompareTo(right) < 0;
    public static bool operator <=( Pair left, Pair right ) => left.CompareTo(right) <= 0;
}



[DefaultValue(nameof(Empty))][Serializable][StructLayout(LayoutKind.Auto)][SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")][SuppressMessage("ReSharper", "ConvertToAutoPropertyWithPrivateSetter")]
public record struct Pairs( int Capacity ) : IEnumerable<Pair>, IValueEnumerable<Pairs.Enumerator, Pair>
{
    public static readonly Pairs Empty = new(0);
    private readonly Pair[]? _owner = Capacity > 0
                                          ? ArrayPool<Pair>.Shared.Rent(Capacity)
                                          : null;
    private int _length = 0;


    public readonly int Capacity => Span.Length;
    public ref Pair this[ string key ]
    {
        get
        {
            for ( int i = 0; i < Length; i++ )
            {
                ref Pair pair = ref Span[i];
                if ( string.Equals(pair.Key, key, StringComparison.Ordinal) ) { return ref pair; }
            }

            return ref Span[_length++];
        }
    }
    public readonly ref Pair this[ int index ] => ref Span[index];
    public readonly   int                Length => _length;
    internal readonly Memory<Pair>       Memory => _owner;
    internal readonly Span<Pair>         Span   => new(_owner);
    public readonly   ReadOnlySpan<Pair> Values => Span[.._length];


    public Pairs() : this(DEFAULT_CAPACITY) { }
    public void Dispose()
    {
        if ( _owner is not null ) { ArrayPool<Pair>.Shared.Return(_owner); }
    }
    public static Pairs Create( params ReadOnlySpan<Pair> pairs ) => new() { pairs };


    public readonly Pairs Sort()
    {
        Span.Sort();
        return this;
    }
    public Pairs SetTag( string key, string? value )
    {
        ref Pair pair = ref this[key];
        if ( pair.Value != value ) { pair = new Pair(key, value); }

        return this;
    }
    public Pairs Add( in Pair value )
    {
        if ( ++_length > Capacity ) { throw new InvalidOperationException("Cannot add more pairs than the capacity of the buffer."); }

        if ( _owner is not null ) { Span[_length] = value; }

        return this;
    }
    public Pairs Add( params ReadOnlySpan<Pair> pairs )
    {
        foreach ( ref readonly Pair pair in pairs ) { Add(in pair); }

        return this;
    }


    public readonly void CopyTo( Span<Pair> span ) => Values.CopyTo(span);
    public static void EnsureCapacity( scoped ref Pairs pairs, int minNeededCapacity )
    {
        if ( pairs.Capacity - pairs.Length >= minNeededCapacity ) { return; }

        Pairs newOwner = new(Math.Max(pairs.Capacity * 2, pairs.Length + minNeededCapacity));
        pairs.CopyTo(newOwner.Span);
        pairs.Dispose();
        pairs = newOwner;
    }
    [Pure] public ValueEnumerable<Enumerator, Pair> AsValueEnumerable() => new(new Enumerator(this));


    public IEnumerator<Pair> GetEnumerator()
    {
        for ( int i = 0; i < _length; i++ ) { yield return Values[i]; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator( in Pairs tags ) : IValueEnumerator<Pair>
    {
        private readonly Pairs _tags = tags;
        private          int   index;


        public bool TryGetNonEnumeratedCount( out int count )
        {
            count = _tags._length;
            return true;
        }

        public bool TryGetSpan( out ReadOnlySpan<Pair> span )
        {
            span = _tags.Values;
            return true;
        }

        public bool TryCopyTo( Span<Pair> destination, Index offset )
        {
            if ( EnumeratorHelper.TryGetSlice(_tags.Values, offset, destination.Length, out ReadOnlySpan<Pair> slice) )
            {
                slice.CopyTo(destination);
                return true;
            }

            return false;
        }

        public bool TryGetNext( out Pair current )
        {
            ReadOnlySpan<Pair> values = _tags.Values;

            if ( index < values.Length )
            {
                current = values[index];
                index++;
                return true;
            }

            current = Pair.Empty;
            return false;
        }

        public void Dispose() { }
    }
}
