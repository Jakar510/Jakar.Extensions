// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static partial class Spans
{
    [Pure]
    public static T First<T>( this scoped ref readonly Span<T> values, Func<T, bool> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.First( selector );
    }
    [Pure]
    public static T First<T>( this scoped ref readonly ReadOnlySpan<T> values, Func<T, bool> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static T? FirstOrDefault<T>( this scoped ref readonly Span<T> values, Func<T, bool> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.FirstOrDefault( selector );
    }
    [Pure]
    public static T? FirstOrDefault<T>( this scoped ref readonly ReadOnlySpan<T> values, Func<T, bool> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static T Last<T>( this scoped ref readonly ReadOnlySpan<T> span, Func<T, bool> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate( span[index] ) ) return span[index];
        }

        throw new NotFoundException();
    }
    [Pure]
    public static T? LastOrDefault<T>( this scoped ref readonly ReadOnlySpan<T> span, Func<T, bool> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate( span[index] ) ) return span[index];
        }

        return default;
    }


    [Pure]
    public static bool All<T>( this scoped ref readonly Span<T> values, Func<T, bool> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.All( selector );
    }
    [Pure]
    public static bool All<T>( this scoped ref readonly ReadOnlySpan<T> values, Func<T, bool> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) is false ) { return false; }
        }

        return true;
    }


    [Pure]
    public static bool Any<T>( this scoped ref readonly Span<T> values, Func<T, bool> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.Any( selector );
    }
    [Pure]
    public static bool Any<T>( this scoped ref readonly ReadOnlySpan<T> values, Func<T, bool> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return true; }
        }

        return false;
    }


    [Pure]
    public static T Single<T>( this scoped ref readonly Span<T> values, Func<T, bool> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.Single( selector );
    }
    [Pure]
    public static T Single<T>( this scoped ref readonly ReadOnlySpan<T> values, Func<T, bool> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static T? SingleOrDefault<T>( this scoped ref readonly Span<T> values, Func<T, bool> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.SingleOrDefault( selector );
    }
    [Pure]
    public static T? SingleOrDefault<T>( this scoped ref readonly ReadOnlySpan<T> values, Func<T, bool> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static int Count<T>( this scoped ref readonly Span<T> span, T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> temp = span;
        return temp.Count( value );
    }
    [Pure]
    public static int Count<T>( this scoped ref readonly ReadOnlySpan<T> span, T value )
        where T : IEquatable<T>
    {
        int result = 0;

        foreach ( T v in span )
        {
            if ( v.Equals( value ) ) { result++; }
        }

        return result;
    }
}
