using System.Runtime.InteropServices;



namespace Jakar.Extensions.Spans;


[SuppressMessage("ReSharper", "OutParameterValueIsAlwaysDiscarded.Global")]
public static partial class Spans
{
    public static Span<T> AsSpan<T>( this   ReadOnlySpan<T> span ) => span.AsSpan(span.Length);
    public static Span<T> AsSpan<T>( this   ReadOnlySpan<T> span, in int length ) => MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(span), length);
    public static Memory<T> AsSpan<T>( this T[]             span ) => MemoryMarshal.CreateFromPinnedArray(span, 0, span.Length);


    public static int LastIndexOf( this ReadOnlySpan<char> value, in char c, in int endIndex )
    {
        Guard.IsInRangeFor(endIndex, value, nameof(value));

        return value[..endIndex].LastIndexOf(c);
    }


    /// <summary>
    /// Allocates a string
    /// </summary>
    public static ReadOnlySpan<T> Slice<T>( this ReadOnlySpan<T> value, in T startValue, in T endValue, in bool includeEnds ) where T : IEquatable<T>
    {
        int start = value.IndexOf(startValue);
        int end   = value.IndexOf(endValue);

        if ( start < 0 && end < 0 ) { return value; }

        start += includeEnds
                     ? 0
                     : 1;

        end += includeEnds
                   ? 1
                   : 0;

        int length = end - start;

        if ( start + length >= value.Length ) { return value[start..]; }

        Guard.IsInRangeFor(start, value, nameof(value));
        Guard.IsInRangeFor(end,   value, nameof(value));
        return value.Slice(start, length);
    }
    public static void Slice<T>( in ReadOnlySpan<T> value, in T startValue, in T endValue, in bool includeEnds, ref Span<T> buffer, out int charWritten ) where T : IEquatable<T>
    {
        int start = value.IndexOf(startValue);
        int end   = value.IndexOf(endValue);

        if ( start > 0 && end > 0 )
        {
            start += includeEnds
                         ? 0
                         : 1;

            end += includeEnds
                       ? 1
                       : 0;

            ReadOnlySpan<T> result = start + end - start >= value.Length
                                         ? value[start..]
                                         : value.Slice(start, end);

            result.CopyTo(buffer);
            charWritten = result.Length;
            return;
        }

        value.CopyTo(buffer);
        charWritten = value.Length;
    }


    public static bool Contains( this Span<char> span, ReadOnlySpan<char> value ) => span.Contains(value, StringComparison.Ordinal);
    public static bool Contains( this Span<char> span, ReadOnlySpan<char> value, StringComparison comparison )
    {
        ReadOnlySpan<char> temp = span;
        return temp.Contains(value, comparison);
    }
    public static bool Contains( this ReadOnlySpan<char> span, ReadOnlySpan<char> value ) => span.Contains(value, StringComparison.Ordinal);


    public static bool Contains<T>( this ReadOnlySpan<T> span, ReadOnlySpan<T> value ) where T : IEquatable<T>
    {
        for ( var i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            if ( span.Slice(i, value.Length).SequenceEqual(value) ) { return true; }
        }

        return false;
    }
    public static bool Contains<T>( this ReadOnlySpan<T> span, T value ) where T : IEquatable<T>
    {
        foreach ( T item in span )
        {
            if ( item.Equals(value) ) { return true; }
        }

        return false;
    }
    public static bool ContainsAll<T>( this ReadOnlySpan<T> span, params T[] values ) where T : IEquatable<T>
    {
        var result = true;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( T c in values ) { result &= span.Contains(c); }

        return result;
    }
    public static bool ContainsNone<T>( this ReadOnlySpan<T> span, params T[] values ) where T : IEquatable<T>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( T c in values )
        {
            if ( span.Contains(c) ) { return false; }
        }

        return true;
    }
    public static bool ContainsAny<T>( this ReadOnlySpan<T> span, params T[] values ) where T : IEquatable<T>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( T c in values )
        {
            if ( span.Contains(c) ) { return true; }
        }

        return false;
    }


    public static bool StartsWith<T>( this Span<T> span, in T value ) where T : IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[0].Equals(value);
    }
    public static bool StartsWith<T>( this ReadOnlySpan<T> span, in T value ) where T : IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[0].Equals(value);
    }


    public static bool EndsWith<T>( this Span<T> span, in T value ) where T : IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[^1].Equals(value);
    }
    public static bool EndsWith<T>( this ReadOnlySpan<T> span, in T value ) where T : IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        return span[^1].Equals(value);
    }


    /// <summary>
    /// Allocates a string
    /// </summary>
    public static ReadOnlySpan<char> Replace( this ReadOnlySpan<char> value, in ReadOnlySpan<char> oldValue, in ReadOnlySpan<char> newValue, in char defaultValue = '\0' )
    {
        Span<char> buffer = stackalloc char[value.Length + newValue.Length];
        Replace(value, oldValue, newValue, defaultValue, ref buffer, out _);
        return new string(buffer);
    }

    /// <summary>
    /// Allocates a string
    /// </summary>
    public static ReadOnlySpan<char> Replace( this ReadOnlySpan<char> value, in ReadOnlySpan<char> oldValue, in ReadOnlySpan<char> newValue, in char startValue, in char endValue, in char defaultValue = '\0' )
    {
        Span<char> buffer = stackalloc char[value.Length + newValue.Length];
        Replace(value, oldValue, newValue, startValue, endValue, defaultValue, ref buffer, out _);
        return new string(buffer);
    }


    public static void Replace<T>( in ReadOnlySpan<T> value, in ReadOnlySpan<T> oldValue, in ReadOnlySpan<T> newValue, in T defaultValue, ref Span<T> buffer, out int charWritten ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor(value.Length + newValue.Length - 1, buffer, nameof(buffer));

        if ( !value.Contains(oldValue) )
        {
            value.CopyTo(buffer);
            charWritten = value.Length;
            return;
        }

        int start = value.IndexOf(oldValue);

        int end = start + oldValue.Length;

        Guard.IsInRangeFor(start, value, nameof(value));
        Join(value[..start], newValue, defaultValue, ref buffer, out int first);
        ReadOnlySpan<T> temp = buffer[..first];

        Guard.IsInRangeFor(end, value, nameof(value));
        Join(temp, value[end..], defaultValue, ref buffer, out int second);
        charWritten = first + second;
    }
    public static void Replace<T>( in ReadOnlySpan<T> value, in ReadOnlySpan<T> oldValue, in ReadOnlySpan<T> newValue, in T startValue, in T endValue, in T defaultValue, ref Span<T> buffer, out int charWritten ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor(value.Length + newValue.Length - 1, buffer, nameof(buffer));

        if ( !value.Contains(oldValue) )
        {
            value.CopyTo(buffer);
            charWritten = value.Length;
            return;
        }

        int start = value.IndexOf(oldValue);

        int end = start + oldValue.Length;

        if ( !oldValue.StartsWith(startValue) && start > 0 ) { start--; }

        if ( !oldValue.EndsWith(endValue) ) { end++; }

        Guard.IsInRangeFor(start, value, nameof(value));
        Join(value[..start], newValue, defaultValue, ref buffer, out int first);
        ReadOnlySpan<T> temp = buffer[..first];

        Guard.IsInRangeFor(end, value, nameof(value));
        Join(temp, value[end..], defaultValue, ref buffer, out int second);
        charWritten = first + second;
    }


    /// <summary>
    /// Allocates a string
    /// </summary>
    public static ReadOnlySpan<char> Join( this ReadOnlySpan<char> value, in ReadOnlySpan<char> other )
    {
        int        size   = value.Length + other.Length;
        Span<char> buffer = stackalloc char[size];
        Join(value, other, '\0', ref buffer, out _);
        return new string(buffer);
    }
    public static void Join<T>( in ReadOnlySpan<T> first, in ReadOnlySpan<T> last, in T defaultValue, ref Span<T> buffer, out int charWritten ) where T : IEquatable<T>
    {
        charWritten = first.Length + last.Length;
        Guard.IsInRangeFor(charWritten - 1, buffer, nameof(buffer));

        for ( var i = 0; i < first.Length; i++ ) { buffer[i] = first[i]; }

        for ( var i = 0; i < last.Length; i++ ) { buffer[i + first.Length] = last[i]; }

        for ( int i = first.Length + last.Length; i < buffer.Length; i++ ) { buffer[i] = defaultValue; }
    }

    public static void CopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer, in T defaultValue ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor(value.Length - 1, buffer, nameof(buffer));

        for ( var i = 0; i < value.Length; i++ ) { buffer[i] = value[i]; }

        for ( int i = value.Length; i < buffer.Length; i++ ) { buffer[i] = defaultValue; }
    }


    /// <summary>
    /// Allocates a string
    /// </summary>
    public static ReadOnlySpan<char> RemoveAll( this ReadOnlySpan<char> value, in char c )
    {
        Span<char> buffer = stackalloc char[value.Length];
        RemoveAll(value, c, ref buffer);
        return new string(buffer);
    }
    public static void RemoveAll<T>( in ReadOnlySpan<T> value, in T c, ref Span<T> buffer ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor(value.Length - 1, buffer, nameof(buffer));

        var offset = 0;

        for ( var i = 0; i < value.Length; i++ )
        {
            if ( value[i].Equals(c) )
            {
                offset++;
                continue;
            }

            buffer[i - offset] = value[i];
        }
    }
    public static void RemoveAll<T>( in ReadOnlySpan<T> value, ref Span<T> buffer, params T[] removed ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor(value.Length - 1, buffer, nameof(buffer));

        var offset = 0;

        for ( var i = 0; i < value.Length; i++ )
        {
            var skip = false;

            foreach ( T item in removed )
            {
                if ( value[i].Equals(item) )
                {
                    offset++;
                    skip = true;
                }
            }

            if ( skip ) { continue; }

            buffer[i - offset] = value[i];
        }
    }
}
