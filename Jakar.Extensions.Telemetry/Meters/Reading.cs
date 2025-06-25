// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 03/13/2025  14:03

namespace Jakar.Extensions.Telemetry.Meters;


[DefaultValue( nameof(Empty) )]
[method: JsonConstructor]
public readonly struct Reading<TValue>( TValue value, DateTimeOffset timeStamp, params Pair[]? tags ) : IEqualityOperators<Reading<TValue>>, IComparisonOperators<Reading<TValue>>
    where TValue : IEquatable<TValue>
{
    public static readonly Reading<TValue>              Empty     = new(default!, DateTimeOffset.UtcNow, null);
    public readonly        DateTimeOffset               TimeStamp = timeStamp;
    public readonly        Pair[]?                      Tags      = tags;
    public readonly        TValue                       Value     = value;
    public static          ValueSorter<Reading<TValue>> Sorter => ValueSorter<Reading<TValue>>.Default;


    public Reading( TValue value ) : this( value, null ) { }
    public Reading( TValue value, params Pair[]? tags ) : this( value, DateTimeOffset.UtcNow, tags ) { }


    public static implicit operator Reading<TValue>( TValue value ) => new(value);


    public static Reading<TValue> Create( TValue value )                      => new(value);
    public static Reading<TValue> Create( TValue value, params Pair[]? tags ) => new(value, tags);
    public static Reading<TValue>? TryGetLastValue( params ReadOnlySpan<Reading<TValue>> span )
    {
        return span.Length > 0
                   ? span[^1].Value
                   : null;
    }


    public int CompareTo( Reading<TValue> other ) => TimeStamp.CompareTo( other.TimeStamp );
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is Reading<TValue> reading
                   ? CompareTo( reading )
                   : throw new ArgumentException( $"Object must be of type {nameof(Reading<TValue>)}" );
    }
    public          bool Equals( Reading<TValue> other )                              => TimeStamp.Equals( other.TimeStamp ) && Value.Equals( other.Value );
    public override bool Equals( object?         other )                              => other is Reading<TValue> reading    && Equals( reading );
    public override int  GetHashCode()                                                => HashCode.Combine( TimeStamp, Tags, Value );
    public static   bool operator ==( Reading<TValue>  left, Reading<TValue>  right ) => Sorter.Equals( left, right );
    public static   bool operator !=( Reading<TValue>  left, Reading<TValue>  right ) => Sorter.DoesNotEqual( left, right );
    public static   bool operator ==( Reading<TValue>? left, Reading<TValue>? right ) => Sorter.Equals( left, right );
    public static   bool operator !=( Reading<TValue>? left, Reading<TValue>? right ) => Sorter.DoesNotEqual( left, right );
    public static   bool operator >( Reading<TValue>   left, Reading<TValue>  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( Reading<TValue>  left, Reading<TValue>  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( Reading<TValue>   left, Reading<TValue>  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( Reading<TValue>  left, Reading<TValue>  right ) => Sorter.LessThanOrEqualTo( left, right );
}
