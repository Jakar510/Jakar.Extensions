// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

namespace Jakar.Extensions;


public interface IUserRights
{
    public const                      int    MAX_SIZE = BaseRecord.ANSI_STRING_CAPACITY;
    [StringLength( MAX_SIZE )] public string Rights { get; set; }
}



public static class RightsExtensions
{
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
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
    public const     char   VALID   = '+';
    public const     char   INVALID = '-';
    private readonly char[] _rights = ArrayPool<char>.Shared.Rent( _enumValues.Length );

#if NET6_0_OR_GREATER
    private static readonly TEnum[] _enumValues = Enum.GetValues<TEnum>();
#else
    private static readonly TEnum[] _enumValues = (TEnum[])Enum.GetValues( typeof(TEnum) );
#endif

    internal readonly Span<char>        Span    { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _rights; }
    public static     UserRights<TEnum> Default { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(); }
    public readonly Right[] Rights
    {
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
        get
        {
            Right[] array = AsyncLinq.GetArray<Right>( _enumValues.Length );
            for ( int i = 0; i < array.Length; i++ ) { array[i] = new Right( _enumValues[i], Has( i ) ); }

            return array;
        }
    }
    public static UserRights<TEnum> SA { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Default.Add( _enumValues ); }


    public UserRights()
    {
        if ( _enumValues.Length > IUserRights.MAX_SIZE ) { throw new OutOfRangeException( nameof(TEnum), typeof(TEnum).Name, $"Max permission count is {IUserRights.MAX_SIZE}" ); }

        Span.Fill( INVALID );
    }
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
        where TRecord : class, IUserRights => Default.With( record );


    [Pure]
    public readonly UserRights<TEnum> With<TRecord>( TRecord record )
        where TRecord : class, IUserRights => With( record.Rights );

    [Pure]
    private readonly UserRights<TEnum> With( scoped in ReadOnlySpan<char> other )
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


    [Pure] public static UserRights<TEnum> Merge( IEnumerable<IEnumerable<IUserRights>> values ) => Merge( values.SelectMany( static x => x ) );

    [Pure]
    public static UserRights<TEnum> Merge( IEnumerable<IUserRights> values )
    {
        UserRights<TEnum> rights = new();
        foreach ( IUserRights value in values ) { rights = rights.With( value ); }

        return rights;
    }

    [Pure]
    public static UserRights<TEnum> Merge( scoped in ReadOnlySpan<IUserRights> values )
    {
        UserRights<TEnum> rights = new();
        foreach ( IUserRights value in values ) { rights = rights.With( value ); }

        return rights;
    }


    [Pure] public static   UserRights<TEnum> Create()                                       => Default;
    [Pure] internal static UserRights<TEnum> Create( scoped in ReadOnlySpan<char>  rights ) => Default.With( rights );
    [Pure] public static   UserRights<TEnum> Create( scoped in ReadOnlySpan<TEnum> array )  => Default.Add( array );


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly bool Has( int   index ) => Span[index] == VALID;
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly bool Has( TEnum index ) => Has( index.AsInt() );

    [Pure]
    public readonly bool Has( scoped in ReadOnlySpan<TEnum> array )
    {
        foreach ( TEnum i in array )
        {
            if ( Has( i ) is false ) { return false; }
        }

        return true;
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum> Remove( TEnum index ) => Set( index.AsInt(), INVALID );

    [Pure]
    public readonly UserRights<TEnum> Remove( scoped in ReadOnlySpan<TEnum> array )

    {
        foreach ( TEnum i in array ) { Remove( i ); }

        return this;
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly UserRights<TEnum> Add( TEnum index ) => Set( index.AsInt(), VALID );

    [Pure]
    public readonly UserRights<TEnum> Add( scoped in ReadOnlySpan<TEnum> array )
    {
        foreach ( TEnum i in array ) { Add( i ); }

        return this;
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
    internal readonly UserRights<TEnum> Set( int index, char value )
    {
        Guard.IsInRange( index, 0, _enumValues.Length );
        Guard.IsTrue( value is VALID or INVALID );

        Span[index] = value;
        return this;
    }



    public readonly record struct Right( TEnum Index, bool Value );
}
