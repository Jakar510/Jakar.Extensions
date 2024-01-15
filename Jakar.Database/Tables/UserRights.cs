// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

using System.Buffers;



namespace Jakar.Database;


[ DefaultMember( nameof(Default) ), SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Local" ) ]
public struct UserRights : IEnumerator<UserRights.Right>, IEnumerable<UserRights.Right>, IRegisterDapperTypeHandlers
{
    public const            int                             MAX_SIZE     = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
    public const            char                            VALID        = '+';
    public const            char                            INVALID      = '-';
    private static readonly ConcurrentDictionary<Type, int> _enumLengths = new();
    private static readonly MemoryPool<char>                _pool        = MemoryPool<char>.Shared;
    private                 Memory<char>                    _rights;
    private                 IMemoryOwner<char>?             _owner;
    private                 int                             _index = 0;


    public static UserRights Default
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => new(1);
    }
    public readonly int Length
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _rights.Length;
    }
    public readonly Right Current
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => new(_index, Has( _index ));
    }
    readonly object IEnumerator.Current => Current;
    internal readonly Span<char> Span
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _rights.Span;
    }


    public UserRights() : this( 0 ) { }
    public UserRights( in int totalRightCount )
    {
        if ( totalRightCount <= 0 ) { throw new ArgumentException( $"{nameof(totalRightCount)} must be > 0" ); }

        _owner  = _pool.Rent( totalRightCount );
        _rights = _owner.Memory;
        for ( int i = 0; i < Length; i++ ) { Span[i] = INVALID; }
    }
    public UserRights( in ReadOnlySpan<char> rights )
    {
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


    [ Pure ]
    public static UserRights Create<T>( T rights )
        where T : IRights => new(rights.Rights);
    [ Pure ]
    public readonly UserRights With<T>( T rights )
        where T : IRights
    {
        using var other = new UserRights( rights.Rights );
        Trace.Assert( Length == other.Length, $"{typeof(T).Name}.{nameof(IRights.Rights)} should be {Length} long" );

        for ( int i = 0; i < Length; i++ )
        {
            Span[i] = other.Span[i] == VALID
                          ? VALID
                          : INVALID;
        }

        return this;
    }
    [ Pure ]
    public static UserRights Merge<T>( params IEnumerable<IRights>[] values )
        where T : struct, Enum => Merge( GetTotalRightCount<T>(), values.SelectMany( static x => x ) );
    [ Pure ]
    public static UserRights Merge<T>( IEnumerable<IEnumerable<IRights>> values )
        where T : struct, Enum => Merge( GetTotalRightCount<T>(), values.SelectMany( static x => x ) );
    [ Pure ]
    public static UserRights Merge<T>( IEnumerable<IRights> values )
        where T : struct, Enum => Merge( GetTotalRightCount<T>(), values );
    [ Pure ] public static UserRights Merge( in  int                totalRightCount, params IEnumerable<IRights>[]     values ) => Merge( totalRightCount, values.SelectMany( static x => x ) );
    [ Pure ] public static UserRights Merge( in  int                totalRightCount, IEnumerable<IEnumerable<IRights>> values ) => Merge( totalRightCount, values.SelectMany( static x => x ) );
    [ Pure ] public static UserRights Merge( in  int                totalRightCount, IEnumerable<IRights>              values ) => values.Aggregate( new UserRights( totalRightCount ), static ( current, value ) => current.With( value ) );
    [ Pure ] public static UserRights Create( in int                totalRightCount ) => new(totalRightCount);
    [ Pure ] public static UserRights Create( string                rights )          => new(rights);
    [ Pure ] public static UserRights Create( in ReadOnlySpan<char> rights )          => new(rights);


    [ Pure ]
    public static UserRights Create<T>()
        where T : struct, Enum
    {
        ReadOnlySpan<T> array = Enum.GetValues<T>();
        return new UserRights( array.Length ).Add( array );
    }

    [ Pure ]
    public static UserRights Create<T>( params T[] array )
        where T : struct, Enum => new UserRights( GetTotalRightCount<T>() ).Add( array );

    [ Pure ]
    public static UserRights Create<T>( in ReadOnlySpan<T> array )
        where T : struct, Enum => new UserRights( GetTotalRightCount<T>() ).Add( array );

    [ Pure ]
    public static int GetTotalRightCount<T>()
        where T : struct, Enum
    {
        Type type = typeof(T);
        if ( _enumLengths.TryGetValue( type, out int length ) is false ) { _enumLengths[type] = length = Enum.GetValues<T>().Length; }

        return length;
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] bool IEnumerator.MoveNext() => ++_index < Length;
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] void IEnumerator.Reset()    => _index = -1;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public readonly bool Has( int index ) => Span[index] == VALID;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public readonly bool Has<T>( T index )
        where T : struct, Enum => Has( index.AsInt() );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public readonly UserRights Remove( int index ) => Set( index, INVALID );

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public readonly UserRights Remove<T>( T index )
        where T : struct, Enum => Remove( index.AsInt() );

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public readonly UserRights Remove<T>( params T[] array )
        where T : struct, Enum => Remove( new ReadOnlySpan<T>( array ) );

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public readonly UserRights Remove<T>( in ReadOnlySpan<T> array )
        where T : struct, Enum
    {
        foreach ( T i in array ) { Remove( i.AsInt() ); }

        return this;
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public readonly UserRights Add( int index ) => Set( index, VALID );

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public readonly UserRights Add<T>( T index )
        where T : struct, Enum => Add( index.AsInt() );

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public readonly UserRights Add<T>( params T[] array )
        where T : struct, Enum => Add( new ReadOnlySpan<T>( array ) );

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public readonly UserRights Add<T>( in ReadOnlySpan<T> array )
        where T : struct, Enum
    {
        foreach ( T i in array ) { Add( i.AsInt() ); }

        return this;
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    internal readonly UserRights Set( int index, char value )
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
    public readonly IEnumerator<Right>      GetEnumerator() => this;
    readonly        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public readonly record struct Right( int Index, bool Value );



    public interface IRights
    {
        [ MaxLength( MAX_SIZE ) ] public string Rights { get; }

        [ Pure ] public UserRights GetRights();
    }



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



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, UserRights?>
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
