// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

namespace Jakar.Database;


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



[DefaultMember( nameof(Default) ), SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Local" ), SuppressMessage( "ReSharper", "LoopCanBeConvertedToQuery" )]
public struct UserRights<TEnum>
    where TEnum : struct, Enum
{
    public const            char    VALID       = '+';
    public const            char    INVALID     = '-';
    private readonly        char[]  _rights     = ArrayPool<char>.Shared.Rent( _enumValues.Length );
    private static readonly TEnum[] _enumValues = Enum.GetValues<TEnum>();


    internal      Span<char>        Span    { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _rights; }
    public static UserRights<TEnum> Default { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(); }
    public        int               Length  { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Span.Length; }
    public Right[] Rights
    {
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
        get
        {
            Right[] array = AsyncLinq.GetArray<Right>( Length );
            for ( int i = 0; i < array.Length; i++ ) { array[i] = new Right( _enumValues[i], Has( i ) ); }

            return array;
        }
    }


    public UserRights() => Span.Fill( INVALID );
    public void Dispose()
    {
        ArrayPool<char>.Shared.Return( _rights );
        this = default;
    }
    public override string ToString()
    {
        ReadOnlySpan<char> span   = Span;
        string             result = span.ToString();
        Dispose();
        return result;
    }


    [Pure]
    public static UserRights<TEnum> Create<TRecord>( TRecord record )
        where TRecord : class, IRights => Default.With( record );


    [Pure]
    public UserRights<TEnum> With<TRecord>( TRecord record )
        where TRecord : class, IRights => With( record.Rights );

    private UserRights<TEnum> With( scoped in ReadOnlySpan<char> other )
    {
        With( Span, other );
        return this;
    }
    private static void With( scoped in Span<char> span, scoped in ReadOnlySpan<char> other )
    {
        Guard.IsGreaterThanOrEqualTo( other.Length, span.Length );

        for ( int i = 0; i < span.Length; i++ )
        {
            if ( VALID.Equals( other[i] ) ) { span[i] = VALID; }
        }
    }


    [Pure] public static UserRights<TEnum> Merge( IEnumerable<IEnumerable<IRights>> values ) => Merge( values.SelectMany( static x => x ) );

    [Pure]
    public static UserRights<TEnum> Merge( IEnumerable<IRights> values )
    {
        UserRights<TEnum> rights = new();
        foreach ( IRights value in values ) { rights = rights.With( value ); }

        return rights;
    }

    [Pure]
    public static UserRights<TEnum> Merge( scoped in ReadOnlySpan<IRights> values )
    {
        UserRights<TEnum> rights = new();
        foreach ( IRights value in values ) { rights = rights.With( value ); }

        return rights;
    }


    [Pure] public static UserRights<TEnum> Create()                                       => Default;
    [Pure] public static UserRights<TEnum> Create( scoped in ReadOnlySpan<char>  rights ) => Default.With( rights );
    [Pure] public static UserRights<TEnum> Create( scoped in ReadOnlySpan<TEnum> array )  => Default.Add( array );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Has( int   index ) => Span[index] == VALID;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Has( TEnum index ) => Has( index.AsInt() );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public UserRights<TEnum> Remove( int   index ) => Set( index, INVALID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public UserRights<TEnum> Remove( TEnum index ) => Remove( index.AsInt() );

    public UserRights<TEnum> Remove( scoped in ReadOnlySpan<TEnum> array )

    {
        foreach ( TEnum i in array ) { Remove( i.AsInt() ); }

        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public UserRights<TEnum> Add( int   index ) => Set( index, VALID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public UserRights<TEnum> Add( TEnum index ) => Add( index.AsInt() );
    public UserRights<TEnum> Add( scoped in ReadOnlySpan<TEnum> array )
    {
        foreach ( TEnum i in array ) { Add( i ); }

        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    internal UserRights<TEnum> Set( int index, char value )
    {
        Guard.IsGreaterThanOrEqualTo( index, 0 );
        Guard.IsLessThan( index, Length );
        Guard.IsTrue( value is VALID or INVALID );

        Span[index] = value;
        return this;
    }


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public record struct Right( TEnum Index, bool Value );



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, UserRights<TEnum>>
    {
        public override void SetValue( IDbDataParameter parameter, UserRights<TEnum> value ) => parameter.Value = value.ToString();
        public override UserRights<TEnum> Parse( object value ) => value switch
                                                                   {
                                                                       string s => Create( s ),
                                                                       _        => Default
                                                                   };
    }



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, UserRights<TEnum>?>
    {
        public override void SetValue( IDbDataParameter parameter, UserRights<TEnum>? value ) => parameter.Value = value?.ToString();
        public override UserRights<TEnum>? Parse( object value ) => value switch
                                                                    {
                                                                        string s => Create( s ),
                                                                        _        => null
                                                                    };
    }
}
