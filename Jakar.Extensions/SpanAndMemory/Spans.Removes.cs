﻿// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:19 AM

namespace Jakar.Extensions;


public static partial class Spans
{
    public static ReadOnlySpan<char> RemoveAll( this ReadOnlySpan<char> value, ReadOnlySpan<char> removed )
    {
        Span<char> buffer = stackalloc char[value.Length];
        RemoveAll( value, buffer, out int charWritten, removed );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }
    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> value, T c ) where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length];
        RemoveAll( value, c, buffer, out int charWritten );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }


    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> value, ReadOnlySpan<T> removed ) where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length];
        RemoveAll( value, buffer, out int charWritten, removed );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }


    public static ReadOnlySpan<T> Replace<T>( this ReadOnlySpan<T> value, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue ) where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + newValue.Length];
        Replace( value, oldValue, newValue, buffer, out int charWritten );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }


    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Replace<T>( this ReadOnlySpan<T> value, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue, T startValue, T endValue ) where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + newValue.Length + 1];
        Replace( value, oldValue, newValue, startValue, endValue, buffer, out int charWritten );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }

    public static ReadOnlySpan<T> Slice<T>( this ReadOnlySpan<T> value, T startValue, T endValue, bool includeEnds ) where T : unmanaged, IEquatable<T>
    {
        int start = value.IndexOf( startValue );
        int end   = value.IndexOf( endValue );

        if ( start < 0 && end < 0 ) { return value; }

        start += includeEnds
                     ? 0
                     : 1;

        end += includeEnds
                   ? 1
                   : 0;

        int length = end - start;

        if ( start + length >= value.Length ) { return value[start..]; }

        Guard.IsInRangeFor( start, value, nameof(value) );
        Guard.IsInRangeFor( end,   value, nameof(value) );
        return value.Slice( start, length );
    }


    public static Span<char> RemoveAll( this Span<char> value, ReadOnlySpan<char> removed )
    {
        Span<char> buffer = stackalloc char[value.Length];
        RemoveAll( value, buffer, out int charWritten, removed );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }


    public static Span<T> RemoveAll<T>( this Span<T> value, ReadOnlySpan<T> removed ) where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length];
        RemoveAll( value, buffer, out int charWritten, removed );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }


    public static Span<T> Replace<T>( this Span<T> value, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue ) where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + newValue.Length];
        Replace( value, oldValue, newValue, buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }


    public static Span<T> Replace<T>( this Span<T> value, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue, T startValue, T endValue ) where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + newValue.Length];
        Replace( value, oldValue, newValue, startValue, endValue, buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }


    public static void RemoveAll<T>( ReadOnlySpan<T> value, T c, Span<T> buffer, out int charWritten ) where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        int offset = 0;

        for ( int i = 0; i < value.Length; i++ )
        {
            if ( value[i].Equals( c ) )
            {
                offset++;
                continue;
            }

            buffer[i - offset] = value[i];
        }

        charWritten = value.Length - offset;
    }


    public static void RemoveAll<T>( ReadOnlySpan<T> value, Span<T> buffer, out int charWritten, ReadOnlySpan<T> removed ) where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        int offset = 0;

        for ( int i = 0; i < value.Length; i++ )
        {
            bool skip = false;

            foreach ( T item in removed )
            {
                if ( !value[i].Equals( item ) ) { continue; }

                offset++;
                skip = true;
            }

            if ( skip ) { continue; }

            buffer[i - offset] = value[i];
        }

        charWritten = value.Length - offset;
    }


    public static void Replace<T>( ReadOnlySpan<T> value, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue, Span<T> buffer, out int charWritten ) where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length + newValue.Length - 1, buffer, nameof(buffer) );

        if ( !value.Contains( oldValue ) )
        {
            value.CopyTo( buffer );
            charWritten = value.Length;
            return;
        }

        // charWritten = 0;
        //
        // for ( int i = 0; i < value.Length; i++ )
        // {
        //     if ( value.Slice( i, i + oldValue.Length )
        //               .SequenceEqual( oldValue ) )
        //     {
        //
        //     }
        // }


        int start = value.IndexOf( oldValue );
        int end   = start + oldValue.Length;


        Guard.IsInRangeFor( start, value, nameof(value) );
        Join( value[..start], newValue, ref buffer, out int first );
        ReadOnlySpan<T> temp = buffer[..first];

        Guard.IsInRangeFor( end, value, nameof(value) );
        Join( temp, value[end..], ref buffer, out int second );
        charWritten = first + second;
    }


    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Replace<T>( ReadOnlySpan<T> value, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue, T startValue, T endValue, Span<T> buffer, out int charWritten ) where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length + newValue.Length - 1, buffer, nameof(buffer) );

        if ( !value.Contains( oldValue ) )
        {
            value.CopyTo( buffer );
            charWritten = value.Length;
            return;
        }

        int start = value.IndexOf( oldValue );

        int end = start + oldValue.Length;

        if ( !oldValue.StartsWith( startValue ) && start > 0 ) { start--; }

        if ( !oldValue.EndsWith( endValue ) ) { end++; }

        Guard.IsInRangeFor( start, value, nameof(value) );
        Join( value[..start], newValue, ref buffer, out int first );

        Guard.IsInRangeFor( end, value, nameof(value) );
        Join( buffer[..first], value[end..], ref buffer, out int second );
        charWritten = first + second;
    }


    public static void Slice<T>( ReadOnlySpan<T> value, T startValue, T endValue, bool includeEnds, Span<T> buffer, out int charWritten ) where T : unmanaged, IEquatable<T>
    {
        int start = value.IndexOf( startValue );
        int end   = value.IndexOf( endValue );

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
                                         : value.Slice( start, end );

            result.CopyTo( buffer );
            charWritten = result.Length;
            return;
        }

        value.CopyTo( buffer );
        charWritten = value.Length;
    }
}
