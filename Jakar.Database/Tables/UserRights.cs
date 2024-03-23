// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

namespace Jakar.Database;


public readonly record struct Right<TEnum>( TEnum Index, bool Value )
    where TEnum : struct, Enum;



public interface IRights
{
    public const                      int    MAX_SIZE = SQL.ANSI_STRING_CAPACITY;
    [StringLength( MAX_SIZE )] public string Rights { get; }
}



public static class RightsExtensions
{
    [Pure]
    public static UserRights<TEnum> GetRights<TEnum>( this IRights rights )
        where TEnum : struct, Enum => UserRights<TEnum>.Create( rights );
}



public interface IUserRights : IRegisterDapperTypeHandlers
{
    int Length { get; }
    IUserRights With<TRecord>( TRecord rights )
        where TRecord : class, IRights;
    bool        Has( int    index );
    IUserRights Remove( int index );
    IUserRights Add( int    index );
    string      ToString();
}



public interface IUserRights<TEnum> : IUserRights, IEnumerator<Right<TEnum>>, IEnumerable<Right<TEnum>>
    where TEnum : struct, Enum
{
    new IUserRights<TEnum> With<TRecord>( TRecord rights )
        where TRecord : class, IRights;
    bool               Has( TEnum                         index );
    IUserRights<TEnum> Remove( TEnum                      index );
    IUserRights<TEnum> Remove( params TEnum[]             array );
    IUserRights<TEnum> Remove( in     ReadOnlySpan<TEnum> array );
    IUserRights<TEnum> Add( TEnum                         index );
    IUserRights<TEnum> Add( params TEnum[]                array );
    IUserRights<TEnum> Add( in     ReadOnlySpan<TEnum>    array );


    [Pure] public abstract static IUserRights<TEnum> Merge( params IEnumerable<IRights>[]     values );
    [Pure] public abstract static IUserRights<TEnum> Merge( IEnumerable<IEnumerable<IRights>> values );
    [Pure] public abstract static IUserRights<TEnum> Merge( IEnumerable<IRights>              values );
    [Pure] public abstract static IUserRights<TEnum> Merge( in  int                           totalRightCount, params IEnumerable<IRights>[]     values );
    [Pure] public abstract static IUserRights<TEnum> Merge( in  int                           totalRightCount, IEnumerable<IEnumerable<IRights>> values );
    [Pure] public abstract static IUserRights<TEnum> Merge( in  int                           totalRightCount, IEnumerable<IRights>              values );
    [Pure] public abstract static IUserRights<TEnum> Create( in int                           totalRightCount );
    [Pure] public abstract static IUserRights<TEnum> Create( string                           rights );
    [Pure] public abstract static IUserRights<TEnum> Create( in ReadOnlySpan<char>            rights );
    [Pure] public abstract static IUserRights<TEnum> Create();
    [Pure] public abstract static IUserRights<TEnum> Create( params TEnum[]             array );
    [Pure] public abstract static IUserRights<TEnum> Create( in     ReadOnlySpan<TEnum> array );

    [Pure]
    public abstract static IUserRights<TEnum> Create<TRecord>( TRecord rights )
        where TRecord : class, IRights;
}



[DefaultMember( nameof(Default) )]
[SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Local" )]
public struct UserRights<TEnum> : IUserRights<TEnum>
    where TEnum : struct, Enum
{
    public const            char                  VALID   = '+';
    public const            char                  INVALID = '-';
    private static readonly MemoryPool<char>      _pool   = MemoryPool<char>.Shared;
    private static readonly ImmutableArray<TEnum> _values = [.. Enum.GetValues<TEnum>()];
    private                 IMemoryOwner<char>?   _owner;
    private                 int                   _index = 0;


    private readonly  Memory<char>       _Rights { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _owner?.Memory ?? Memory<char>.Empty; }
    public static     UserRights<TEnum>  Default { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(1); }
    public readonly   int                Length  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Rights.Length; }
    public readonly   Right<TEnum>       Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_values[_index], Has( _index )); }
    readonly          object IEnumerator.Current => Current;
    internal readonly Span<char>         Span    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Rights.Span; }


    public UserRights() : this( _values.Length ) { }
    public UserRights( int totalRightCount )
    {
        totalRightCount = Math.Max( totalRightCount, _values.Length );
        if ( totalRightCount <= 0 ) { throw new ArgumentException( $"{nameof(totalRightCount)} must be > 0" ); }

        _owner = _pool.Rent( totalRightCount );
        for ( int i = 0; i < Length; i++ ) { Span[i] = INVALID; }
    }
    public UserRights( in ReadOnlySpan<char> rights ) : this( rights.Length ) => rights.CopyTo( Span );
    public void Dispose()
    {
        _owner?.Dispose();
        _owner = default;
        this   = default;
    }


    [Pure] static IUserRights<TEnum> IUserRights<TEnum>.Create<TRecord>( TRecord rights ) => Create( rights );
    [Pure]
    public static UserRights<TEnum> Create<TRecord>( TRecord rights )
        where TRecord : class, IRights => new(rights.Rights);
    readonly IUserRights IUserRights.              With<TRecord>( TRecord rights ) => With( rights );
    readonly IUserRights<TEnum> IUserRights<TEnum>.With<TRecord>( TRecord rights ) => With( rights );
    [Pure]
    public readonly UserRights<TEnum> With<TRecord>( TRecord rights )
        where TRecord : class, IRights
    {
        using var other = new UserRights<TEnum>( rights.Rights );
        Trace.Assert( Length == other.Length, $"{typeof(TEnum).Name}.{nameof(IRights.Rights)} should be {Length} long" );

        for ( int i = 0; i < Length; i++ )
        {
            Span[i] = other.Span[i] == VALID
                          ? VALID
                          : INVALID;
        }

        return this;
    }
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Merge( params IEnumerable<IRights>[]     values )                                                    => Merge( values );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Merge( IEnumerable<IEnumerable<IRights>> values )                                                    => Merge( values );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Merge( IEnumerable<IRights>              values )                                                    => Merge( values );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Merge( in     int                        totalRightCount, params IEnumerable<IRights>[]     values ) => Merge( totalRightCount, values );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Merge( in     int                        totalRightCount, IEnumerable<IEnumerable<IRights>> values ) => Merge( totalRightCount, values );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Merge( in     int                        totalRightCount, IEnumerable<IRights>              values ) => Merge( totalRightCount, values );
    [Pure] public static UserRights<TEnum>                     Merge( params IEnumerable<IRights>[]     values )                                                    => Merge( _values.Length,  values.SelectMany( static x => x ) );
    [Pure] public static UserRights<TEnum>                     Merge( IEnumerable<IEnumerable<IRights>> values )                                                    => Merge( _values.Length,  values.SelectMany( static x => x ) );
    [Pure] public static UserRights<TEnum>                     Merge( IEnumerable<IRights>              values )                                                    => Merge( _values.Length,  values );
    [Pure] public static UserRights<TEnum>                     Merge( in  int                           totalRightCount, params IEnumerable<IRights>[]     values ) => Merge( totalRightCount, values.SelectMany( static x => x ) );
    [Pure] public static UserRights<TEnum>                     Merge( in  int                           totalRightCount, IEnumerable<IEnumerable<IRights>> values ) => Merge( totalRightCount, values.SelectMany( static x => x ) );
    [Pure] public static UserRights<TEnum>                     Merge( in  int                           totalRightCount, IEnumerable<IRights>              values ) => values.Aggregate( new UserRights<TEnum>( totalRightCount ), static ( current, value ) => current.With( value ) );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Create( in int                           totalRightCount ) => Create( totalRightCount );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Create( string                           rights )          => Create( rights );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Create( in ReadOnlySpan<char>            rights )          => Create( rights );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Create()                                                   => Create();
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Create( params TEnum[]             array )                 => Create( array );
    [Pure] static        IUserRights<TEnum> IUserRights<TEnum>.Create( in     ReadOnlySpan<TEnum> array )                 => Create( array );
    [Pure] public static UserRights<TEnum>                     Create( in     int                 totalRightCount )       => new(totalRightCount);
    [Pure] public static UserRights<TEnum>                     Create( string                     rights )                => new(rights);
    [Pure] public static UserRights<TEnum>                     Create( in ReadOnlySpan<char>      rights )                => new(rights);
    [Pure] public static UserRights<TEnum>                     Create()                                                   => new UserRights<TEnum>( _values.Length ).Add( _values.AsSpan() );
    [Pure] public static UserRights<TEnum>                     Create( params TEnum[]             array )                 => new UserRights<TEnum>( _values.Length ).Add( array );
    [Pure] public static UserRights<TEnum>                     Create( in     ReadOnlySpan<TEnum> array )                 => new UserRights<TEnum>( _values.Length ).Add( array );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] bool IEnumerator.MoveNext() => ++_index < Length;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] void IEnumerator.Reset()    => _index = -1;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly bool Has( int   index ) => Span[index] == VALID;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly bool Has( TEnum index ) => Has( index.AsInt() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights IUserRights.              Remove( int                        index ) => Set( index, INVALID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights<TEnum> IUserRights<TEnum>.Remove( TEnum                      index ) => Remove( index.AsInt() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights<TEnum> IUserRights<TEnum>.Remove( params TEnum[]             array ) => Remove( new ReadOnlySpan<TEnum>( array ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights<TEnum> IUserRights<TEnum>.Remove( in     ReadOnlySpan<TEnum> array ) => Remove( array );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum>                     Remove( int                        index ) => Set( index, INVALID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum>                     Remove( TEnum                      index ) => Remove( index.AsInt() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum>                     Remove( params TEnum[]             array ) => Remove( new ReadOnlySpan<TEnum>( array ) );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public readonly UserRights<TEnum> Remove( in ReadOnlySpan<TEnum> array )

    {
        foreach ( TEnum i in array ) { Remove( i.AsInt() ); }

        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights IUserRights.              Add( int                        index ) => Set( index, INVALID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights<TEnum> IUserRights<TEnum>.Add( TEnum                      index ) => Add( index.AsInt() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights<TEnum> IUserRights<TEnum>.Add( params TEnum[]             array ) => Add( new ReadOnlySpan<TEnum>( array ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] readonly        IUserRights<TEnum> IUserRights<TEnum>.Add( in     ReadOnlySpan<TEnum> array ) => Add( array );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum>                     Add( int                        index ) => Set( index, VALID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum>                     Add( TEnum                      index ) => Add( index.AsInt() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum>                     Add( params TEnum[]             array ) => Add( new ReadOnlySpan<TEnum>( array ) );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public readonly UserRights<TEnum> Add( in ReadOnlySpan<TEnum> array )

    {
        foreach ( TEnum i in array ) { Add( i.AsInt() ); }

        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    internal readonly UserRights<TEnum> Set( int index, char value )
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
    public readonly IEnumerator<Right<TEnum>> GetEnumerator() => this;
    readonly        IEnumerator IEnumerable.  GetEnumerator() => GetEnumerator();


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, UserRights<TEnum>>
    {
        public override void SetValue( IDbDataParameter parameter, UserRights<TEnum> value ) => parameter.Value = value;

        public override UserRights<TEnum> Parse( object value ) =>
            value switch
            {
                string guidValue => new UserRights<TEnum>( guidValue ),
                _                => default
            };
    }



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, UserRights<TEnum>?>
    {
        public override void SetValue( IDbDataParameter parameter, UserRights<TEnum>? value ) => parameter.Value = value;

        public override UserRights<TEnum>? Parse( object value ) =>
            value switch
            {
                string guidValue => new UserRights<TEnum>( guidValue ),
                _                => default
            };
    }
}
