// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static partial class Spans
{
    [Pure]
    public static T First<T>( this Span<T> values, Predicate<T> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.First( selector );
    }
    [Pure]
    public static T First<T>( this ReadOnlySpan<T> values, Predicate<T> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static T? FirstOrDefault<T>( this Span<T> values, Predicate<T> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.FirstOrDefault( selector );
    }
    [Pure]
    public static T? FirstOrDefault<T>( this ReadOnlySpan<T> values, Predicate<T> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static bool All<T>( this Span<T> values, Predicate<T> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.All( selector );
    }
    [Pure]
    public static bool All<T>( this ReadOnlySpan<T> values, Predicate<T> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) is false ) { return false; }
        }

        return true;
    }


    [Pure]
    public static bool Any<T>( this Span<T> values, Predicate<T> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.Any( selector );
    }
    [Pure]
    public static bool Any<T>( this ReadOnlySpan<T> values, Predicate<T> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return true; }
        }

        return false;
    }


    [Pure]
    public static T Single<T>( this Span<T> values, Predicate<T> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.Single( selector );
    }
    [Pure]
    public static T Single<T>( this ReadOnlySpan<T> values, Predicate<T> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static T? SingleOrDefault<T>( this Span<T> values, Predicate<T> selector )
    {
        ReadOnlySpan<T> span = values;
        return span.SingleOrDefault( selector );
    }
    [Pure]
    public static T? SingleOrDefault<T>( this ReadOnlySpan<T> values, Predicate<T> selector )
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static Span<T> Where<T>( scoped in Span<T> values, Predicate<T> selector )
    {
        if ( values.IsEmpty ) { return default; }

        T[]     buffer = AsyncLinq.GetArray<T>( values.Length );
        Span<T> span   = buffer;
        Where( values, ref span, selector, out int length );

        return new Span<T>( buffer, 0, length );
    }
    [Pure]
    public static ReadOnlySpan<T> Where<T>( scoped in ReadOnlySpan<T> values, Predicate<T> selector )
    {
        if ( values.IsEmpty ) { return default; }

        Buffer<T> buffer = new(values.Length);

        try
        {
            Where( values, ref buffer, selector );
            return buffer.ToArray();
        }
        finally { buffer.Dispose(); }
    }
    [Pure]
    public static ReadOnlySpan<T> WhereValues<T>( scoped in ReadOnlySpan<T> values, Predicate<T> selector )
        where T : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;

            default:
            {
                Buffer<T> buffer = new(values.Length);

                try
                {
                    Where( values, ref buffer, selector );
                    return buffer.ToArray();
                }
                finally { buffer.Dispose(); }
            }
        }
    }
    [Pure]
    public static Span<T> WhereValues<T>( scoped in Span<T> values, Predicate<T> selector )
        where T : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;

            default:
            {
                Buffer<T> buffer = new(values.Length);

                try
                {
                    Where( values, ref buffer, selector );
                    return buffer.ToArray();
                }
                finally { buffer.Dispose(); }
            }
        }
    }


    public static bool Where<T>( scoped in ReadOnlySpan<T> values, scoped ref Buffer<T> span, Predicate<T> selector )
    {
        if ( values.IsEmpty ) { return false; }

        foreach ( T value in values )
        {
            if ( selector( value ) ) { span.Add( value ); }
        }

        return span.Length > 0;
    }
    public static bool Where<T>( scoped in ReadOnlySpan<T> values, scoped ref Span<T> span, Predicate<T> selector, out int length )
    {
        if ( values.IsEmpty )
        {
            length = 0;
            return false;
        }

        if ( values.Length > span.Length ) { throw new ArgumentException( "buffer too small" ); }

        length = 0;

        foreach ( T value in values )
        {
            if ( selector( value ) ) { span[length++] = value; }
        }

        return length > 0;
    }
    public static bool Where<T, TNext>( scoped in ReadOnlySpan<T> values, scoped ref Span<TNext> span, Func<T, TNext> func, out int length )
    {
        if ( values.IsEmpty )
        {
            length = 0;
            return false;
        }

        if ( values.Length > span.Length ) { throw new ArgumentException( "buffer too small" ); }

        length = 0;

        foreach ( T value in values ) { span[length++] = func( value ); }

        return length > 0;
    }
    public static bool Where<T, TNext>( scoped in ReadOnlySpan<T> values, scoped ref Span<TNext> span, Predicate<T> selector, Func<T, TNext> func, out int length )
    {
        if ( values.IsEmpty )
        {
            length = 0;
            return false;
        }

        if ( values.Length > span.Length ) { throw new ArgumentException( "buffer too small" ); }

        length = 0;

        foreach ( T value in values )
        {
            if ( selector( value ) ) { span[length++] = func( value ); }
        }

        return length > 0;
    }


    [Pure]
    public static Span<TNext> Select<T, TNext>( scoped in Span<T> values, Func<T, TNext> func )
    {
        if ( values.IsEmpty ) { return default; }

        TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
        Span<TNext> span   = buffer;
        Where( values, ref span, func, out int length );

        return new Span<TNext>( buffer, 0, length );
    }
    [Pure]
    public static ReadOnlySpan<TNext> Select<T, TNext>( scoped in ReadOnlySpan<T> values, Func<T, TNext> func )
    {
        if ( values.IsEmpty ) { return default; }

        TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
        Span<TNext> span   = buffer;
        Where( values, ref span, func, out int length );

        return new ReadOnlySpan<TNext>( buffer, 0, length );
    }
    [Pure]
    public static Span<TNext> SelectValues<T, TNext>( scoped in Span<T> values, Func<T, TNext> func )
        where TNext : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;

            default:
            {
                TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
                Span<TNext> span   = buffer;
                Where( values, ref span, func, out int length );

                return new Span<TNext>( buffer, 0, length );
            }
        }
    }
    [Pure]
    public static ReadOnlySpan<TNext> SelectValues<T, TNext>( scoped in ReadOnlySpan<T> values, Func<T, TNext> func )
        where TNext : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;

            default:
            {
                TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
                Span<TNext> span   = buffer;
                Where( values, ref span, func, out int length );

                return new ReadOnlySpan<TNext>( buffer, 0, length );
            }
        }
    }


    [Pure]
    public static Span<TNext> Select<T, TNext>( scoped in Span<T> values, Predicate<T> selector, Func<T, TNext> func )
    {
        if ( values.IsEmpty ) { return default; }

        TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
        Span<TNext> span   = buffer;
        Where( values, ref span, selector, func, out int length );

        return new Span<TNext>( buffer, 0, length );
    }
    [Pure]
    public static ReadOnlySpan<TNext> Select<T, TNext>( scoped in ReadOnlySpan<T> values, Predicate<T> selector, Func<T, TNext> func )
    {
        if ( values.IsEmpty ) { return default; }

        TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
        Span<TNext> span   = buffer;
        Where( values, ref span, selector, func, out int length );

        return new ReadOnlySpan<TNext>( buffer, 0, length );
    }
    [Pure]
    public static Span<TNext> SelectValues<T, TNext>( scoped in Span<T> values, Predicate<T> selector, Func<T, TNext> func )
        where TNext : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;
                
            default:
            {
                TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
                Span<TNext> span   = buffer;
                Where( values, ref span, selector, func, out int length );

                return new Span<TNext>( buffer, 0, length );
            }
        }
    }
    [Pure]
    public static ReadOnlySpan<TNext> SelectValues<T, TNext>( scoped in ReadOnlySpan<T> values, Predicate<T> selector, Func<T, TNext> func )
        where TNext : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;

            default:
            {
                TNext[]     buffer = AsyncLinq.GetArray<TNext>( values.Length );
                Span<TNext> span   = buffer;
                Where( values, ref span, selector, func, out int length );

                return new ReadOnlySpan<TNext>( buffer, 0, length );
            }
        }
    }


    [Pure]
    public static int Count<T>( this Span<T> span, T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> temp = span;
        return temp.Count( value );
    }
    [Pure]
    public static int Count<T>( this ReadOnlySpan<T> span, T value )
        where T : IEquatable<T>
    {
        int result = 0;

        foreach ( T v in span )
        {
            if ( v.Equals( value ) ) { result++; }
        }

        return result;
    }


    [Pure]
    public static ReadOnlySpan<T> Join<T>( this ReadOnlySpan<T> value, scoped in ReadOnlySpan<T> other )
        where T : unmanaged, IEquatable<T>
    {
        T[]     array  = AsyncLinq.GetArray<T>( value.Length + other.Length );
        Span<T> buffer = array;
        Join( value, other, ref buffer, out int length );
        Debug.Assert( buffer.Length == length );
        return new ReadOnlySpan<T>( array, 0, length );
    }
    [Pure]
    public static Span<T> Join<T>( this Span<T> value, scoped in ReadOnlySpan<T> other )
        where T : unmanaged, IEquatable<T>
    {
        T[]     array  = AsyncLinq.GetArray<T>( value.Length + other.Length );
        Span<T> buffer = array;
        Join( value, other, ref buffer, out int length );
        Debug.Assert( buffer.Length == length );
        return new Span<T>( array, 0, length );
    }
    [Pure]
    public static bool Join<T>( scoped in ReadOnlySpan<T> first, scoped in ReadOnlySpan<T> last, scoped ref Span<T> buffer, out int length )
    {
        int size = first.Length;
        length = size + last.Length;
        first.CopyTo( buffer[..size] );
        last.CopyTo( buffer[size..] );
        return true;
    }
    [Pure]
    public static bool Join<T>( scoped in ReadOnlySpan<T> first, scoped in ReadOnlySpan<T> last, scoped ref Buffer<T> buffer )
    {
        buffer.Add( first );
        buffer.Add( last );
        return true;
    }
}
