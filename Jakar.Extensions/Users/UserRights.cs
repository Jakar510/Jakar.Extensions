// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

namespace Jakar.Extensions;


public interface IUserRights
{
    public const                      int    MAX_SIZE = ANSI_CAPACITY;
    [StringLength( MAX_SIZE )] public string Rights { get; set; }
}



public interface IUserRights<out T> : IUserRights
    where T : IUserRights<T>
{
    public T WithRights<TEnum>( scoped in UserRights<TEnum> rights )
        where TEnum : struct, Enum;
}



public interface IUserRights<out T, TEnum> : IUserRights
    where TEnum : struct, Enum
    where T : IUserRights<T, TEnum>
{
    public T WithRights( scoped in UserRights<TEnum> rights );
}



public static class RightsExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static UserRights<TEnum> GetRights<TEnum>( this IUserRights rights )
        where TEnum : struct, Enum => UserRights<TEnum>.Create( rights );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void SetRights<TEnum>( this IUserRights user, scoped in ReadOnlySpan<TEnum> array )
        where TEnum : struct, Enum => user.SetRights( user.GetRights<TEnum>().Add( array ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void SetRights<TEnum>( this IUserRights user, scoped in UserRights<TEnum> value )
        where TEnum : struct, Enum => user.Rights = value.ToString();
}



[DefaultMember( nameof(Default) ), SuppressMessage( "ReSharper", "FieldCanBeMadeReadOnly.Local" ), SuppressMessage( "ReSharper", "LoopCanBeConvertedToQuery" )]
public ref struct UserRights<TEnum>
    where TEnum : struct, Enum
{
    public const            char    VALID       = '+';
    public const            char    INVALID     = '-';
    private static readonly TEnum[] _enumValues = Enum.GetValues<TEnum>();
    private readonly        char[]  _rights     = ArrayPool<char>.Shared.Rent( _enumValues.Length );


    internal readonly Span<char>        Span    => _rights;
    public static     UserRights<TEnum> Default => new();
    public readonly Right[] Rights
    {
        [Pure]
        get
        {
            Right[] array = AsyncLinq.GetArray<Right>( _enumValues.Length );
            for ( int i = 0; i < array.Length; i++ ) { array[i] = new Right( _enumValues[i], Has( i ) ); }

            return array;
        }
    }
    public static UserRights<TEnum> SA => Default.Add( _enumValues );


    public UserRights()
    {
        if ( _enumValues.Length > IUserRights.MAX_SIZE ) { throw OutOfRangeException.Create( typeof(TEnum).Name, $"Max permission count is {IUserRights.MAX_SIZE}" ); }

        Span.Fill( INVALID );
    }
    public void Dispose()
    {
        ArrayPool<char>.Shared.Return( _rights );
        this = default;
    }


    public static   UserRights<TEnum> Create()                                       => Default;
    internal static UserRights<TEnum> Create( scoped in ReadOnlySpan<char>  rights ) => Default.With( rights );
    public static   UserRights<TEnum> Create( scoped in ReadOnlySpan<TEnum> array )  => Default.Add( array );
    public static UserRights<TEnum> Create<T>( IEnumerable<IEnumerable<T>> user )
        where T : IUserRights => Default.With( user );
    public static UserRights<TEnum> Create<T>( IEnumerable<T> user )
        where T : IUserRights => Default.With( user );
    public static UserRights<TEnum> Create<T>( T user )
        where T : IUserRights => Default.With( user );


    public override string ToString()
    {
        ReadOnlySpan<char> span   = Span;
        string             result = span.ToString();
        Dispose();
        return result;
    }


    public UserRights<TEnum> With<T>( IEnumerable<IEnumerable<T>> values )
        where T : IUserRights => With( values.SelectMany( static x => x ) );
    public UserRights<TEnum> With<T>( IEnumerable<T> values )
        where T : IUserRights
    {
        foreach ( T value in values ) { this = With( value ); }

        return this;
    }
    public UserRights<TEnum> With<T>( scoped in ReadOnlySpan<T> values )
        where T : IUserRights
    {
        foreach ( T value in values ) { this = With( value ); }

        return this;
    }


    public readonly UserRights<TEnum> With<T>( T user )
        where T : IUserRights => With( user.Rights );
    private readonly UserRights<TEnum> With( scoped in ReadOnlySpan<char> other )
    {
        Span<char> span = Span;
        Guard.IsGreaterThanOrEqualTo( other.Length, span.Length );

        for ( int i = 0; i < span.Length; i++ )
        {
            if ( VALID.Equals( other[i] ) ) { span[i] = VALID; }
        }

        return this;
    }


    public readonly bool Has( int   index ) => Span[index] == VALID;
    public readonly bool Has( TEnum index ) => Has( index.AsInt() );
    public readonly bool Has( scoped in ReadOnlySpan<TEnum> array )
    {
        foreach ( TEnum i in array )
        {
            if ( Has( i ) is false ) { return false; }
        }

        return true;
    }


    public readonly UserRights<TEnum> Remove( TEnum index ) => Set( index.AsInt(), INVALID );
    public readonly UserRights<TEnum> Remove( scoped in ReadOnlySpan<TEnum> array )

    {
        foreach ( TEnum i in array ) { Remove( i ); }

        return this;
    }


    public readonly UserRights<TEnum> Add( TEnum index ) => Set( index.AsInt(), VALID );
    public readonly UserRights<TEnum> Add( scoped in ReadOnlySpan<TEnum> array )
    {
        foreach ( TEnum i in array ) { Add( i ); }

        return this;
    }


    private readonly UserRights<TEnum> Set( int index, char value )
    {
        Guard.IsInRange( index, 0, _enumValues.Length );
        Guard.IsTrue( value is VALID or INVALID );

        Span[index] = value;
        return this;
    }



    public readonly record struct Right( TEnum Index, bool Value );
}
