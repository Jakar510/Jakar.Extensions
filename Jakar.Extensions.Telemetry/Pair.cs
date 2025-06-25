// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/10/2025  11:01

namespace Jakar.Extensions.Telemetry;


public readonly struct Pair( string key, string? value ) : IEqualityOperators<Pair>, IComparisonOperators<Pair>
{
    public readonly string  Key   = key;
    public readonly string? Value = value;


    public static ValueSorter<Pair> Sorter => ValueSorter<Pair>.Default;


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
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is Pair other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(Pair)}" );
    }
    public          bool Equals( Pair    other ) => Key == other.Key  && Value == other.Value;
    public override bool Equals( object? obj )   => obj is Pair other && Equals( other );
    public override int  GetHashCode()           => HashCode.Combine( Key, Value );

    
    public static bool operator ==( Pair  left, Pair  right ) => Sorter.Equals( left, right );
    public static bool operator !=( Pair  left, Pair  right ) => Sorter.DoesNotEqual( left, right );
    public static bool operator ==( Pair? left, Pair? right ) => Sorter.Equals( left, right );
    public static bool operator !=( Pair? left, Pair? right ) => Sorter.DoesNotEqual( left, right );
    public static bool operator >( Pair   left, Pair  right ) => Sorter.GreaterThan( left, right );
    public static bool operator >=( Pair  left, Pair  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static bool operator <( Pair   left, Pair  right ) => Sorter.LessThan( left, right );
    public static bool operator <=( Pair  left, Pair  right ) => Sorter.LessThanOrEqualTo( left, right );
}
