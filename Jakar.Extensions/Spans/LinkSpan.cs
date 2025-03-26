// Jakar.Extensions :: Jakar.Extensions
// 03/20/2025  14:03

namespace Jakar.Extensions;


public delegate bool RefCheck<TValue>( ref readonly TValue value );



public delegate TNext RefConvert<TValue, out TNext>( ref readonly TValue value );



public static class LinkSpan
{
    [Pure]
    public static ReadOnlySpan<TValue> Where<TValue>( ReadOnlySpan<TValue> span, RefCheck<TValue> selector )
    {
        if ( span.IsEmpty ) { return default; }

        TValue[] buffer = GC.AllocateUninitializedArray<TValue>( span.Length );
        int      index  = 0;

        foreach ( TValue value in span )
        {
            if ( selector( in value ) ) { buffer[index++] = value; }
        }

        return new ReadOnlySpan<TValue>( buffer, 0, index );
    }

    [Pure]
    public static ReadOnlySpan<TNext> Select<TValue, TNext>( ReadOnlySpan<TValue> span, RefConvert<TValue, TNext> func )
        where TNext : IEquatable<TNext>
    {
        if ( span.IsEmpty ) { return default; }

        TNext[] buffer = GC.AllocateUninitializedArray<TNext>( span.Length );
        int     index  = 0;

        foreach ( TValue value in span ) { buffer[index++] = func( in value ); }

        return buffer;
    }


    [Pure]
    public static ReadOnlySpan<TNext> Where<TValue, TNext>( ReadOnlySpan<TValue> span, RefCheck<TValue> selector, RefConvert<TValue, TNext> func )
        where TNext : IEquatable<TNext>
    {
        if ( span.IsEmpty ) { return default; }

        TNext[] buffer = GC.AllocateUninitializedArray<TNext>( span.Length );
        int     index  = 0;

        foreach ( TValue value in span )
        {
            if ( selector( in value ) ) { buffer[index++] = func( in value ); }
        }

        return new ReadOnlySpan<TNext>( buffer, 0, index );
    }


    [Pure]
    public static ReadOnlySpan<TValue> Join<TValue>( this ReadOnlySpan<TValue> first, params ReadOnlySpan<TValue> last )
    {
        int          size   = first.Length;
        TValue[]     buffer = GC.AllocateUninitializedArray<TValue>( size + last.Length );
        Span<TValue> result = buffer;
        first.CopyTo( result[..size] );
        last.CopyTo( result[size..] );
        return buffer;
    }


    [Pure]
    public static ReadOnlySpan<TValue> Replace<TValue>( this ReadOnlySpan<TValue> value, scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue )
        where TValue : unmanaged, IEquatable<TValue>
    {
        Buffer<TValue> buffer = new(value.Length + value.Count( oldValue ) * (newValue.Length - oldValue.Length));

        try
        {
            value.Replace( oldValue, newValue, ref buffer );
            return buffer.ToArray();
        }
        finally { buffer.Dispose(); }
    }
    public static void Replace<TValue>( this ReadOnlySpan<TValue> source, scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue, [MustDisposeResource] scoped ref Buffer<TValue> buffer )
        where TValue : unmanaged, IEquatable<TValue>
    {
        if ( source.ContainsExact( oldValue ) is false )
        {
            source.CopyTo( buffer.Next );
            return;
        }

        int sourceIndex = 0;

        while ( sourceIndex < source.Length )
        {
            if ( source[sourceIndex..].StartsWith( oldValue ) )
            {
                buffer = buffer.EnsureCapacity( newValue.Length );
                buffer.Add( newValue );
                sourceIndex += oldValue.Length;
            }
            else { buffer.Add( source[sourceIndex++] ); }
        }
    }


    [Pure]
    public static ReadOnlySpan<TValue> Remove<TValue>( this ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        TValue[] buffer = GC.AllocateUninitializedArray<TValue>( span.Length );
        int      index  = 0;

        foreach ( TValue equatable in span )
        {
            if ( equatable.Equals( value ) is false ) { buffer[index++] = equatable; }
        }

        return new ReadOnlySpan<TValue>( buffer, 0, index );
    }


    [Pure]
    public static ReadOnlySpan<TValue> Remove<TValue>( this ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> values )
        where TValue : IEquatable<TValue>
    {
        TValue[] buffer = GC.AllocateUninitializedArray<TValue>( span.Length );
        int      index  = 0;

        foreach ( TValue equatable in span )
        {
            if ( equatable.IsOneOf( values ) is false ) { buffer[index++] = equatable; }
        }

        return new ReadOnlySpan<TValue>( buffer, 0, index );
    }
}
