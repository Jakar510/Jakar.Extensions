// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

using Newtonsoft.Json.Linq;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static partial class Spans
{
    [Pure]
    public static TValue First<TValue>( scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return First( in span, selector );
    }
    [Pure]
    public static TValue First<TValue>( scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return FirstOrDefault( in span, selector );
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static TValue First<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.First( selector );
    }
    [Pure]
    public static TValue First<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( in value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.FirstOrDefault( selector );
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( in value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static TValue Last<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, Func<TValue, bool> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate( span[index] ) ) return span[index];
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? LastOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, Func<TValue, bool> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate( span[index] ) ) return span[index];
        }

        return default;
    }


    [Pure]
    public static TValue Last<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, RefCheck<TValue> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate( in span[index] ) ) return span[index];
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? LastOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, RefCheck<TValue> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate( in span[index] ) ) return span[index];
        }

        return default;
    }


    [Pure]
    public static bool All<TValue>( this scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.All( selector );
    }
    [Pure]
    public static bool All<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( value ) is false ) { return false; }
        }

        return true;
    }


    [Pure]
    public static bool All<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.All( selector );
    }
    [Pure]
    public static bool All<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( in value ) is false ) { return false; }
        }

        return true;
    }


    [Pure]
    public static bool Any<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.Any( selector );
    }
    [Pure]
    public static bool Any<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( in value ) ) { return true; }
        }

        return false;
    }


    [Pure]
    public static TValue Single<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.Single( selector );
    }
    [Pure]
    public static TValue Single<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( in value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.SingleOrDefault( selector );
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( in value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static TValue Single<TValue>( scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return Single( in span, selector );
    }
    [Pure]
    public static TValue Single<TValue>( scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return SingleOrDefault( in span, selector );
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( TValue value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static int Count<TValue>( this scoped ref readonly Span<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> temp = span;
        return temp.Count( value );
    }
    [Pure]
    public static int Count<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        int result = 0;

        foreach ( TValue v in span )
        {
            if ( v.Equals( value ) ) { result++; }
        }

        return result;
    }
    [Pure]
    public static int Count<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, RefCheck<TValue> check )
        where TValue : IEquatable<TValue>
    {
        int result = 0;

        foreach ( TValue v in span )
        {
            if ( check( in v ) ) { result++; }
        }

        return result;
    }
}
