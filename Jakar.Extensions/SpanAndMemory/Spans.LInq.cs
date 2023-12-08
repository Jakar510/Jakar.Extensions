// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" ) ]
public static partial class Spans
{
    [ Pure ]
    public static T First<T>( this Span<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = values;
        return span.First( selector );
    }
    [ Pure ]
    public static T First<T>( this ReadOnlySpan<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [ Pure ]
    public static T? FirstOrDefault<T>( this Span<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = values;
        return span.FirstOrDefault( selector );
    }
    [ Pure ]
    public static T? FirstOrDefault<T>( this ReadOnlySpan<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [ Pure ]
    public static T Single<T>( this Span<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = values;
        return span.Single( selector );
    }
    [ Pure ]
    public static T Single<T>( this ReadOnlySpan<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        throw new NotFoundException();
    }
    [ Pure ]
    public static T? SingleOrDefault<T>( this Span<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = values;
        return span.SingleOrDefault( selector );
    }
    [ Pure ]
    public static T? SingleOrDefault<T>( this ReadOnlySpan<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        foreach ( T value in values )
        {
            if ( selector( value ) ) { return value; }
        }

        return default;
    }


    [ Pure ]
    public static Span<T> Where<T>( this Span<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        if ( values.Length == 0 ) { return default; }

        T[]     buffer = AsyncLinq.GetArray<T>( values.Length );
        Span<T> span   = buffer;
        Where( values, ref span, selector, out int length );

        return new Span<T>( buffer, 0, length );
    }
    [ Pure ]
    public static ReadOnlySpan<T> Where<T>( this ReadOnlySpan<T> values, Func<T, bool> selector )
        where T : IEquatable<T>
    {
        if ( values.Length == 0 ) { return default; }

        T[]     buffer = AsyncLinq.GetArray<T>( values.Length );
        Span<T> span   = buffer;
        Where( values, ref span, selector, out int length );

        return new ReadOnlySpan<T>( buffer, 0, length );
    }
    [ Pure ]
    public static ReadOnlySpan<T> WhereValues<T>( this ReadOnlySpan<T> values, Func<T, bool> selector )
        where T : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;

            case > 256:
            {
                Span<T> buffer = stackalloc T[values.Length];
                Where( values, ref buffer, selector, out int length );

                return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), length );
            }

            default:
            {
                T[]     buffer = AsyncLinq.GetArray<T>( values.Length );
                Span<T> span   = buffer;
                Where( values, ref span, selector, out int length );

                return new ReadOnlySpan<T>( buffer, 0, length );
            }
        }
    }
    [ Pure ]
    public static Span<T> WhereValues<T>( this Span<T> values, Func<T, bool> selector )
        where T : unmanaged, IEquatable<T>
    {
        switch ( values.Length )
        {
            case 0: return default;

            case > 256:
            {
                Span<T> buffer = stackalloc T[values.Length];
                Where( values, ref buffer, selector, out int length );

                return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), length );
            }

            default:
            {
                T[]     buffer = AsyncLinq.GetArray<T>( values.Length );
                Span<T> span   = buffer;
                Where( values, ref span, selector, out int length );

                return new Span<T>( buffer, 0, length );
            }
        }
    }


    public static void Where<T>( in ReadOnlySpan<T> values,
                             #if NET6_0_OR_GREATER
                                 scoped
                                 #endif
                                     ref Span<T> span,
                                 Func<T, bool> selector,
                                 out int       length
    )
        where T : IEquatable<T>
    {
        if ( values.Length == 0 )
        {
            length = 0;
            return;
        }

        if ( values.Length > span.Length ) { throw new ArgumentException( "buffer too small" ); }

        length = 0;

        foreach ( T value in values )
        {
            if ( selector( value ) ) { span[length++] = value; }
        }
    }


    [ Pure ]
    public static ReadOnlySpan<T> Join<T>( this ReadOnlySpan<T> value,
                                       #if NET6_0_OR_GREATER
                                           scoped
                                           #endif
                                               in ReadOnlySpan<T> other
    )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + other.Length];
        Join( value, other, ref buffer, out int charWritten );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }
    [ Pure ]
    public static Span<T> Join<T>( this Span<T> value, in ReadOnlySpan<T> other )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + other.Length];
        Join( value, other, ref buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }
    [ Pure ]
    public static bool Join<T>( in ReadOnlySpan<T> first,
                                in ReadOnlySpan<T> last,
                            #if NET6_0_OR_GREATER
                                scoped
                                #endif
                                    ref Span<T> buffer,
                                out int charWritten
    )
    {
        charWritten = first.Length + last.Length;
        Guard.IsInRangeFor( charWritten - 1, buffer, nameof(buffer) );
        return first.TryCopyTo( buffer[..first.Length] ) && last.TryCopyTo( buffer[(first.Length + 1)..] );
    }
}
