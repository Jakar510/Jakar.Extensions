// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

using System.Buffers;



namespace Jakar.Database;


[ DefaultMember( nameof(Default) ), SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Local" ) ]
public struct UserRights : IEnumerator<(int Index, bool Value)>, IEnumerable<(int Index, bool Value)>
{
    public interface IRights
    {
        [ MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] public string Rights { get; }

        public UserRights GetRights();
    }



    public static           UserRights          Default => new();
    private static readonly MemoryPool<char>    _pool   = MemoryPool<char>.Shared;
    public const            char                VALID   = '+';
    public const            char                INVALID = '-';
    private                 Memory<char>        _rights;
    private                 IMemoryOwner<char>? _owner;
    private                 int                 _index = 0;


    public readonly   int                     Length  => _rights.Length;
    public readonly   (int Index, bool Value) Current => (_index, Has( _index ));
    readonly          object IEnumerator.     Current => Current;
    internal readonly Span<char>              Span    => _rights.Span;


    public UserRights() : this( 0 ) { }
    public UserRights( int length )
    {
        if ( length <= 0 ) { throw new ArgumentException( $"{length} must be > 0" ); }

        _owner  = _pool.Rent( length );
        _rights = _owner.Memory;
        for ( int i = 0; i < Length; i++ ) { Span[i] = INVALID; }
    }
    public UserRights( string rights )
    {
        // using IMemoryOwner<char> buffer = _pool.Rent( rights.Length );
        // Span<char>               span   = buffer.Memory.Span;
        // Convert.TryFromBase64String( rights, span, out int length );
        _owner  = _pool.Rent( rights.Length );
        _rights = _owner.Memory;
        rights.CopyTo( Span );
    }
    public void Dispose()
    {
        _owner?.Dispose();
        _owner = default;
        this   = default;
    }


    public static UserRights Create<T>( T rights ) where T : IRights => new(rights.Rights);
    public readonly UserRights With<T>( T rights ) where T : IRights
    {
        using var other = new UserRights( rights.Rights );
        Trace.Assert( Length == other.Length, $"{typeof(T).Name}.{nameof(IRights.Rights)} should be {Length} long" );
        int end = Math.Min( Length, other.Length );

        for ( int i = 0; i < end; i++ )
        {
            Span[i] = other.Span[i] == VALID
                          ? VALID
                          : INVALID;
        }

        return this;
    }
    public static UserRights Merge( int                  totalRightCount, params IEnumerable<IRights>[] values )          => Merge( values.SelectMany( x => x ), totalRightCount );
    public static UserRights Merge( IEnumerable<IRights> values,          int                           totalRightCount ) => values.Aggregate( new UserRights( totalRightCount ), ( current, value ) => current.With( value ) );
    public static UserRights Create( int                 length ) => new(length);
    public static UserRights Create<T>() where T : struct, Enum   => new(Enum.GetValues<T>().Length);


    public bool MoveNext() => ++_index < Length;
    public void Reset()    => _index = -1;


    public readonly bool Has( int  index )                        => Span[index] == VALID;
    public readonly bool Has<T>( T index ) where T : struct, Enum => Has( index.AsInt() );


    public readonly UserRights Remove( int  index )                        => Set( index, INVALID );
    public readonly UserRights Remove<T>( T index ) where T : struct, Enum => Remove( index.AsInt() );


    public readonly UserRights Add( int  index )                        => Set( index, VALID );
    public readonly UserRights Add<T>( T index ) where T : struct, Enum => Add( index.AsInt() );


    public readonly UserRights Set( int index, char value )
    {
        Trace.Assert( index >= 0 );
        Trace.Assert( index < Length );
        Trace.Assert( value is VALID or INVALID );
        Span[index] = value;
        return this;
    }


    public override string ToString()
    {
        string result = Span.ToString();
        Dispose();
        return result;
    }
    public readonly IEnumerator<(int Index, bool Value)> GetEnumerator() => this;
    readonly        IEnumerator IEnumerable.             GetEnumerator() => GetEnumerator();



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, UserRights>
    {
        public override void SetValue( IDbDataParameter parameter, UserRights value ) => parameter.Value = value;

        public override UserRights Parse( object value ) =>
            value switch
            {
                string guidValue => new UserRights( guidValue ),
                _                => default
            };
    }



    public class DapperTypeHandlerNullable : SqlConverter<DapperTypeHandlerNullable, UserRights?>
    {
        public override void SetValue( IDbDataParameter parameter, UserRights? value ) => parameter.Value = value;

        public override UserRights? Parse( object value ) =>
            value switch
            {
                string guidValue => new UserRights( guidValue ),
                _                => default
            };
    }
}
