// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:17 AM

namespace Jakar.Extensions.SpanAndMemory;


public static partial class Spans
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Contains( this Span<char>         span, in ReadOnlySpan<char> value ) => span.Contains(value, StringComparison.Ordinal);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Contains( this ReadOnlySpan<char> span, in ReadOnlySpan<char> value ) => span.Contains(value, StringComparison.Ordinal);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains( this Span<char> span, in ReadOnlySpan<char> value, StringComparison comparison )
    {
        ReadOnlySpan<char> temp = span;
        return temp.Contains(value, comparison);
    }


    /// <summary>
    /// <see cref="MemoryExtensions"/> doesn't have Contains in .Net Standard 2.1. but does in .Net 6.0.
    /// <para>Will be removed in a future version.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="span"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>( in ReadOnlySpan<T> span, in T value ) where T : unmanaged, IEquatable<T>
    {
        foreach ( T item in span )
        {
            if ( item.Equals(value) ) { return true; }
        }

        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> value ) where T : unmanaged, IEquatable<T>
    {
        for ( var i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            if ( span.Slice(i, value.Length).SequenceEqual(value) ) { return true; }
        }

        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAll<T>( this Span<T> span, in ReadOnlySpan<T> values ) where T : unmanaged, IEquatable<T>
    {
        var result = true;


        foreach ( T c in values ) { result &= Contains(span, c); }

        return result;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAll<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> values ) where T : unmanaged, IEquatable<T>
    {
        var result = true;


        foreach ( T c in values ) { result &= Contains(span, c); }

        return result;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsNone<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> values ) where T : unmanaged, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( Contains(span, c) ) { return false; }
        }

        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsNone<T>( this Span<T> span, in ReadOnlySpan<T> values ) where T : unmanaged, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( Contains(span, c) ) { return false; }
        }

        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsNone( this Span<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( Contains(span, c) ) { return false; }
        }

        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsNone( this ReadOnlySpan<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( Contains(span, c) ) { return false; }
        }

        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>( this Span<T> span, in ReadOnlySpan<T> values ) where T : unmanaged, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( Contains(span, c) ) { return true; }
        }

        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> values ) where T : unmanaged, IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( Contains(span, c) ) { return true; }
        }

        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny( this Span<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( Contains(span, c) ) { return true; }
        }

        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny( this ReadOnlySpan<char> span, in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( Contains(span, c) ) { return true; }
        }

        return false;
    }

    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>( this Span<T> span, in T value ) where T : unmanaged, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[0].Equals(value);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>( this ReadOnlySpan<T> span, in T value ) where T : unmanaged, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[0].Equals(value);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>( this Span<T> span, in T value ) where T : unmanaged, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[^1].Equals(value);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>( this ReadOnlySpan<T> span, in T value ) where T : unmanaged, IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[^1].Equals(value);
    }

}
