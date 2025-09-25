// Jakar.Extensions :: Jakar.Database
// 12/02/2024  11:12

using ZLinq.Linq;



namespace Jakar.Database;


public readonly struct SqlKey( bool matchAll, ImmutableArray<string> parameters ) : IEquatable<DynamicParameters>, IEqualityOperators<SqlKey>, IComparisonOperators<SqlKey>, IValueEnumerable<FromImmutableArray<string>, string>
{
    private readonly int                    __hash     = HashCode.Combine(matchAll, parameters);
    public readonly  bool                   matchAll   = matchAll;
    public readonly  ImmutableArray<string> parameters = parameters;
    public readonly  string                 key        = GetKey(matchAll, parameters.AsSpan());


    private static string GetKey( bool matchAll, params ReadOnlySpan<string> parameters ) => new StringBuilder(6 + parameters.Sum(static x => x.Length)).Append(matchAll).Append(':').AppendJoin(',', parameters!).ToString();
    public static  SqlKey Create( bool matchAll, DynamicParameters           parameters ) => new(matchAll, [..parameters.ParameterNames]);


    public ValueEnumerable<FromImmutableArray<string>, string> AsValueEnumerable() => new(new FromImmutableArray<string>(parameters));


    public int CompareTo( SqlKey other )
    {
        int keyComparison = string.Compare(key, other.key, StringComparison.Ordinal);
        if ( keyComparison != 0 ) { return keyComparison; }

        return __hash.CompareTo(other.__hash);
    }
    public bool Equals( SqlKey other )
    {
        if ( __hash != other.__hash ) { return false; }

        if ( matchAll != other.matchAll ) { return false; }

        return AsValueEnumerable().SequenceEqual(other.AsValueEnumerable());
    }
    public override bool Equals( object?            other ) => other is SqlKey x && Equals(x);
    public          bool Equals( DynamicParameters? other ) => other is not null && AsValueEnumerable().SequenceEqual(other.ParameterNames, StringComparer.Ordinal);
    public override int  GetHashCode()                      => __hash;
    public int CompareTo( object? other ) =>
        other is SqlKey sqlKey
            ? CompareTo(sqlKey)
            : throw new ExpectedValueTypeException(nameof(other), other, typeof(SqlKey));


    public static bool operator ==( SqlKey? left, SqlKey? right ) => Nullable.Equals(left, right);
    public static bool operator !=( SqlKey? left, SqlKey? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( SqlKey  left, SqlKey  right ) => EqualityComparer<SqlKey>.Default.Equals(left, right);
    public static bool operator !=( SqlKey  left, SqlKey  right ) => !EqualityComparer<SqlKey>.Default.Equals(left, right);
    public static bool operator >( SqlKey   left, SqlKey  right ) => Comparer<SqlKey>.Default.Compare(left, right) > 0;
    public static bool operator >=( SqlKey  left, SqlKey  right ) => Comparer<SqlKey>.Default.Compare(left, right) >= 0;
    public static bool operator <( SqlKey   left, SqlKey  right ) => Comparer<SqlKey>.Default.Compare(left, right) < 0;
    public static bool operator <=( SqlKey  left, SqlKey  right ) => Comparer<SqlKey>.Default.Compare(left, right) <= 0;
}
