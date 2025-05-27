// Jakar.Extensions :: Jakar.Extensions
// 03/13/2025  13:03

using Newtonsoft.Json.Linq;



namespace Jakar.Extensions;


public readonly struct Pair( string key, string? value ) : IEquatable<Pair>, IComparable<Pair>, IComparable
{
    public readonly string  Key   = key;
    public readonly string? Value = value;


    public static ValueSorter<Pair>    Sorter    => ValueSorter<Pair>.Default;
    public static ValueEqualizer<Pair> Equalizer => ValueEqualizer<Pair>.Default;


    public static implicit operator KeyValuePair<string, string?>( Pair tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, string? Value)( Pair   tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair( KeyValuePair<string, string?> tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair( (string Key, string? Value)   tag ) => new(tag.Key, tag.Value);


    public int CompareTo( Pair other )
    {
        int keyComparison = string.Compare( Key, other.Key, StringComparison.Ordinal );
        if ( keyComparison != 0 ) { return keyComparison; }

        return string.Compare( Value, other.Value, StringComparison.Ordinal );
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is Pair pair
                   ? CompareTo( pair )
                   : throw new ArgumentException( $"Object must be of type {nameof(Pair)}" );
    }
    public          bool Equals( Pair    other ) => string.Equals( Key, other.Key, StringComparison.Ordinal ) && string.Equals( Value, other.Value, StringComparison.Ordinal );
    public override bool Equals( object? obj )   => obj is Pair other                                         && Equals( other );
    public override int  GetHashCode()           => HashCode.Combine( Key, Value );


    public static bool operator ==( Pair left, Pair right ) => left.Equals( right );
    public static bool operator !=( Pair left, Pair right ) => !left.Equals( right );
    public static bool operator <( Pair  left, Pair right ) => left.CompareTo( right ) < 0;
    public static bool operator >( Pair  left, Pair right ) => left.CompareTo( right ) > 0;
    public static bool operator <=( Pair left, Pair right ) => left.CompareTo( right ) <= 0;
    public static bool operator >=( Pair left, Pair right ) => left.CompareTo( right ) >= 0;
}



public readonly struct Pair<TValue>( string key, TValue value ) : IEquatable<Pair<TValue>>, IComparable<Pair<TValue>>, IComparable
    where TValue : IComparable<TValue>, IEquatable<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    public readonly string Key   = key;
    public readonly TValue Value = value;


    public static ValueSorter<Pair<TValue>>    Sorter    => ValueSorter<Pair<TValue>>.Default;
    public static ValueEqualizer<Pair<TValue>> Equalizer => ValueEqualizer<Pair<TValue>>.Default;


    public static implicit operator KeyValuePair<string, TValue>( Pair<TValue> tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, TValue Value)( Pair<TValue>   tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair<TValue>( KeyValuePair<string, TValue> tag ) => new(tag.Key, tag.Value);
    public static implicit operator Pair<TValue>( (string Key, TValue Value)   tag ) => new(tag.Key, tag.Value);


    public int CompareTo( Pair<TValue> other )
    {
        int keyComparison = string.Compare( Key, other.Key, StringComparison.Ordinal );
        if ( keyComparison != 0 ) { return keyComparison; }

        return Value.CompareTo( other.Value );
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is Pair<TValue> pair
                   ? CompareTo( pair )
                   : throw new ArgumentException( $"Object must be of type {nameof(Pair)}" );
    }
    public          bool Equals( Pair<TValue> other ) => string.Equals( Key, other.Key, StringComparison.Ordinal ) && Value.Equals( other.Value );
    public override bool Equals( object?      obj )   => obj is Pair<TValue> other                                 && Equals( other );
    public override int  GetHashCode()                => HashCode.Combine( Key, Value );


    public static bool operator ==( Pair<TValue> left, Pair<TValue> right ) => left.Equals( right );
    public static bool operator !=( Pair<TValue> left, Pair<TValue> right ) => !left.Equals( right );
    public static bool operator <( Pair<TValue>  left, Pair<TValue> right ) => left.CompareTo( right ) < 0;
    public static bool operator >( Pair<TValue>  left, Pair<TValue> right ) => left.CompareTo( right ) > 0;
    public static bool operator <=( Pair<TValue> left, Pair<TValue> right ) => left.CompareTo( right ) <= 0;
    public static bool operator >=( Pair<TValue> left, Pair<TValue> right ) => left.CompareTo( right ) >= 0;
}
