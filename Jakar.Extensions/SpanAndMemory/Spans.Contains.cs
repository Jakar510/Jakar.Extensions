// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:17 AM

namespace Jakar.Extensions.SpanAndMemory;


public static partial class Spans
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Contains( this Span<char>         span, in ReadOnlySpan<char> value ) => MemoryExtensions.Contains(span, value, StringComparison.Ordinal);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Contains( this ReadOnlySpan<char> span, in ReadOnlySpan<char> value ) => span.Contains(value, StringComparison.Ordinal);


    public static bool Contains<T>( this Span<T> span, in T value ) where T : struct, IEquatable<T>
    {
    #if NET6_0
        return MemoryExtensions.Contains(span, value);
    #else
        foreach ( T item in span )
        {
            if ( item.Equals(value) ) { return true; }
        }

        return false;
    #endif
    }
    public static bool Contains<T>( this ReadOnlySpan<T> span, in T value ) where T : struct, IEquatable<T>
    {
    #if NET6_0
        return MemoryExtensions.Contains(span, value);
    #else
        foreach ( T item in span )
        {
            if ( item.Equals(value) ) { return true; }
        }

        return false;
    #endif

    }
    public static bool Contains<T>( this Span<T> span, in ReadOnlySpan<T> value ) where T : struct, IEquatable<T>
    {
        for ( var i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            Span<T> temp = span.Slice(i, value.Length);
            if ( span.Length >= value.Length ) { return false; }

            if ( temp.SequenceEqual(value) ) { return true; }
        }

        return false;
    }
    public static bool Contains<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> value ) where T : struct, IEquatable<T>
    {
        for ( var i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            ReadOnlySpan<T> temp = span.Slice(i, value.Length);
            if ( span.Length >= value.Length ) { return false; }

            if ( temp.SequenceEqual(value) ) { return true; }
        }

        return false;
    }


    public static bool ContainsAll<T>( this Span<T> span, in ReadOnlySpan<T> values ) where T : struct, IEquatable<T>
    {
        var result = true;

        foreach ( T c in values ) { result &= span.Contains(c); }

        return result;
    }
    public static bool ContainsAll<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> values ) where T : struct, IEquatable<T>
    {
        var result = true;

        foreach ( T c in values ) { result &= span.Contains(c); }

        return result;
    }


    public static bool ContainsNone<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> values ) where T : struct, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains(c) ) { return false; }
        }

        return true;
    }
    public static bool ContainsNone<T>( this Span<T> span, in ReadOnlySpan<T> values ) where T : struct, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains(c) ) { return false; }
        }

        return true;
    }


    public static bool ContainsNone( this Span<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains(c) ) { return false; }
        }

        return true;
    }
    public static bool ContainsNone( this ReadOnlySpan<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains(c) ) { return false; }
        }

        return true;
    }


    public static bool ContainsAny<T>( this Span<T> span, in ReadOnlySpan<T> values ) where T : struct, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains(c) ) { return true; }
        }

        return false;
    }
    public static bool ContainsAny<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> values ) where T : struct, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains(c) ) { return true; }
        }

        return false;
    }


    public static bool ContainsAny( this Span<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains(c) ) { return true; }
        }

        return false;
    }
    public static bool ContainsAny( this ReadOnlySpan<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains(c) ) { return true; }
        }

        return false;
    }


    public static bool StartsWith<T>( this Span<T> span, in T value ) where T : struct, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[0].Equals(value);
    }
    public static bool StartsWith<T>( this ReadOnlySpan<T> span, in T value ) where T : struct, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[0].Equals(value);
    }


    public static bool EndsWith<T>( this Span<T> span, in T value ) where T : struct, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[^1].Equals(value);
    }
    public static bool EndsWith<T>( this ReadOnlySpan<T> span, in T value ) where T : struct, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[^1].Equals(value);
    }
}
