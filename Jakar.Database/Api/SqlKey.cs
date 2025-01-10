// Jakar.Extensions :: Jakar.Database
// 12/02/2024  11:12

namespace Jakar.Database;


public readonly struct SqlKey( bool matchAll, ImmutableArray<string> parameters ) : IEquatable<SqlKey>
{
    private readonly int                    _hash      = HashCode.Combine( matchAll, parameters );
    public readonly  bool                   matchAll   = matchAll;
    public readonly  ImmutableArray<string> parameters = parameters;
    public readonly  string                 key        = GetKey( matchAll, parameters.AsSpan()! );


    public static ValueEqualizer<SqlKey> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueEqualizer<SqlKey>.Default; }


    public bool Equals( SqlKey other )
    {
        if ( _hash != other._hash ) { return false; }

        if ( matchAll != other.matchAll ) { return false; }

        return parameters.AsSpan().SequenceEqual( other.parameters.AsSpan() );
    }
    public override bool Equals( object?           other ) => other is SqlKey x && Equals( x );
    public          bool Equals( DynamicParameters other ) => parameters.SequenceEqual( other.ParameterNames, StringComparer.Ordinal );
    public override int  GetHashCode()                     => _hash;


    private static string GetKey( bool matchAll, ReadOnlySpan<string?> parameters ) =>
        new StringBuilder( 6 + parameters.Sum( static x => x?.Length ?? 0 ) ).Append( matchAll ).Append( ':' ).AppendJoin( ',', parameters ).ToString();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static SqlKey Create( bool matchAll, DynamicParameters parameters ) => new(matchAll, [..parameters.ParameterNames]);
}
