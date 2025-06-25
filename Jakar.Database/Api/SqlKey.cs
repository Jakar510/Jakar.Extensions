// Jakar.Extensions :: Jakar.Database
// 12/02/2024  11:12

using ZLinq;
using ZLinq.Linq;



namespace Jakar.Database;


public readonly struct SqlKey( bool matchAll, ImmutableArray<string> parameters ) : IEquatable<DynamicParameters>, IEqualityOperators<SqlKey>, IComparisonOperators<SqlKey>, IValueEnumerable<FromImmutableArray<string>, string>
{
    private readonly int                    _hash      = HashCode.Combine( matchAll, parameters );
    public readonly  bool                   matchAll   = matchAll;
    public readonly  ImmutableArray<string> parameters = parameters;
    public readonly  string                 key        = GetKey( matchAll, parameters.AsSpan()! );


    public static ValueSorter<SqlKey> Sorter { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueSorter<SqlKey>.Default; }


    private static string GetKey( bool matchAll, params ReadOnlySpan<string?> parameters ) => new StringBuilder( 6 + parameters.Sum( static x => x?.Length ?? 0 ) ).Append( matchAll ).Append( ':' ).AppendJoin( ',', parameters ).ToString();
    public static  SqlKey Create( bool matchAll, DynamicParameters            parameters ) => new(matchAll, [..parameters.ParameterNames]);


    public ValueEnumerable<FromImmutableArray<string>, string> AsValueEnumerable() => new(new FromImmutableArray<string>( parameters ));


    public int CompareTo( SqlKey other )
    {
        int keyComparison = string.Compare( key, other.key, StringComparison.Ordinal );
        if ( keyComparison != 0 ) { return keyComparison; }

        return _hash.CompareTo( other._hash );
    }
    public bool Equals( SqlKey other )
    {
        if ( _hash != other._hash ) { return false; }

        if ( matchAll != other.matchAll ) { return false; }

        return AsValueEnumerable().SequenceEqual( other.AsValueEnumerable() );
    }
    public override bool Equals( object?            other ) => other is SqlKey x && Equals( x );
    public          bool Equals( DynamicParameters? other ) => other is not null && AsValueEnumerable().SequenceEqual( other.ParameterNames, StringComparer.Ordinal );
    public override int  GetHashCode()                      => _hash;
    public int CompareTo( object? other ) =>
        other is SqlKey sqlKey
            ? CompareTo( sqlKey )
            : throw new ExpectedValueTypeException( nameof(other), other, typeof(SqlKey) );


    public static bool operator ==( SqlKey? left, SqlKey? right ) => Sorter.Equals( left, right );
    public static bool operator !=( SqlKey? left, SqlKey? right ) => Sorter.DoesNotEqual( left, right );
    public static bool operator ==( SqlKey  left, SqlKey  right ) => Sorter.Equals( left, right );
    public static bool operator !=( SqlKey  left, SqlKey  right ) => Sorter.DoesNotEqual( left, right );
    public static bool operator >( SqlKey   left, SqlKey  right ) => Sorter.GreaterThan( left, right );
    public static bool operator >=( SqlKey  left, SqlKey  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static bool operator <( SqlKey   left, SqlKey  right ) => Sorter.LessThan( left, right );
    public static bool operator <=( SqlKey  left, SqlKey  right ) => Sorter.LessThanOrEqualTo( left, right );
}
