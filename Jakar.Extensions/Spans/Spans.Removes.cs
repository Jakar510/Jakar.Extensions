// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:19 AM

namespace Jakar.Extensions;


public static partial class Spans
{
    public static Span<T> Replace<T>( this ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> old, scoped in ReadOnlySpan<T> value )
        where T : unmanaged, IEquatable<T>
    {
        int                   count  = source.Count( old );
        using IMemoryOwner<T> owner  = MemoryPool<T>.Shared.Rent( source.Length + count * value.Length - count * old.Length + 1 );
        Span<T>               result = owner.Memory.Span;
        int                   length = Replace( source, old, value, result );
        return result[..length].ToArray();
    }


    public static int Replace<T>( scoped in ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> old, scoped in ReadOnlySpan<T> value, scoped in Span<T> result )
        where T : unmanaged, IEquatable<T>
    {
        if ( source.Contains( old ) is false )
        {
            source.CopyTo( result );
            return source.Length;
        }

        int length = 0;

        while ( length < source.Length )
        {
            ReadOnlySpan<T> span = source[length..];

            if ( span.StartsWith( old ) )
            {
                value.CopyTo( result[length..] );
                length += value.Length;
            }
            else
            {
                result[length] = source[length];
                length++;
            }
        }

        return length;
    }


    public static Span<T> Replace<T>( this ReadOnlySpan<T> source, scoped ref ReadOnlySpan<T> old, scoped ref ReadOnlySpan<T> value, T startValue, T endValue )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> result = stackalloc T[source.Length + value.Length + 1];
        Replace( source, old, value, startValue, endValue, ref result, out int length );
        return MemoryMarshal.CreateSpan( ref result.GetPinnableReference(), length );
    }


    public static void Replace<T>( scoped in ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> old, scoped in ReadOnlySpan<T> value, scoped in T startValue, scoped in T endValue, scoped ref Span<T> result, out int length )
        where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( source.Length + value.Length - 1, result, nameof(result) );

        if ( source.Contains( old ) is false )
        {
            source.CopyTo( result );
            length = source.Length;
            return;
        }

        do
        {
            int start = source.IndexOf( old );
            int end   = start + old.Length;

            if ( old.StartsWith( startValue ) is false && start > 0 ) { start--; }

            if ( old.EndsWith( endValue ) is false ) { end++; }

            Guard.IsInRangeFor( start, source, nameof(source) );
            ReadOnlySpan<T> sourceStart  = source[..start];
            ReadOnlySpan<T> tempNewValue = value;
            Join( in sourceStart, in tempNewValue, ref result, out int first );

            Guard.IsInRangeFor( end, source, nameof(source) );
            ReadOnlySpan<T> resultStart = result[..first];
            ReadOnlySpan<T> sourceEnd   = source[end..];
            Join( in resultStart, in sourceEnd, ref result, out int second );
            length = first + second;
        }
        while ( source.Contains( old ) );
    }


    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> source, T c )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> result = stackalloc T[source.Length];
        RemoveAll( source, c, result, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref result.GetPinnableReference(), length );
    }


    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> removed )
        where T : unmanaged, IEquatable<T>
    {
        Span<T>         result = stackalloc T[source.Length];
        ReadOnlySpan<T> temp   = removed;
        RemoveAll( source, temp, result, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref result.GetPinnableReference(), length );
    }


    public static Span<T> RemoveAll<T>( this Span<T> source, scoped in ReadOnlySpan<T> removed )
        where T : unmanaged, IEquatable<T>
    {
        using IMemoryOwner<T> owner  = MemoryPool<T>.Shared.Rent( source.Length );
        Span<T>               result = owner.Memory.Span;
        ReadOnlySpan<T>       temp   = removed;
        RemoveAll( source, temp, result, out int length );
        return result[..length].ToArray();
    }


    public static void RemoveAll<T>( scoped in ReadOnlySpan<T> source, scoped in T c, scoped in Span<T> result, out int length )
        where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( source.Length - 1, result, nameof(result) );
        int offset = 0;

        for ( int i = 0; i < source.Length; i++ )
        {
            if ( source[i].Equals( c ) )
            {
                offset++;
                continue;
            }

            result[i - offset] = source[i];
        }

        length = source.Length - offset;
    }


    public static void RemoveAll<T>( scoped in ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> removed, scoped in Span<T> result, out int length )
        where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( source.Length - 1, result, nameof(result) );
        int offset = 0;

        for ( int i = 0; i < source.Length; i++ )
        {
            bool skip = false;

            foreach ( T item in removed )
            {
                if ( !source[i].Equals( item ) ) { continue; }

                offset++;
                skip = true;
            }

            if ( skip ) { continue; }

            result[i - offset] = source[i];
        }

        length = source.Length - offset;
    }


    public static ReadOnlySpan<T> Slice<T>( this ReadOnlySpan<T> source, T startValue, T endValue, bool includeEnds )
        where T : unmanaged, IEquatable<T>
    {
        int start = source.IndexOf( startValue );
        int end   = source.IndexOf( endValue );

        if ( start < 0 && end < 0 ) { return source; }

        start += includeEnds
                     ? 0
                     : 1;

        end += includeEnds
                   ? 1
                   : 0;

        int length = end - start;

        if ( start + length >= source.Length ) { return source[start..]; }

        Guard.IsInRangeFor( start, source, nameof(source) );
        Guard.IsInRangeFor( end,   source, nameof(source) );
        ReadOnlySpan<T> result = source.Slice( start, length );
        return result.AsReadOnlySpan();

        // return MemoryMarshal.CreateReadOnlySpan( in result.GetPinnableReference(), result.Length );
    }
    public static void Slice<T>( scoped in ReadOnlySpan<T> source, T startValue, T endValue, bool includeEnds, scoped ref Span<T> result, out int length )
        where T : unmanaged, IEquatable<T>
    {
        int start = source.IndexOf( startValue );
        int end   = source.IndexOf( endValue );

        if ( start > 0 && end > 0 )
        {
            start += includeEnds
                         ? 0
                         : 1;

            end += includeEnds
                       ? 1
                       : 0;

            ReadOnlySpan<T> span = start + end - start >= source.Length
                                       ? source[start..]
                                       : source.Slice( start, end );

            span.CopyTo( result );
            length = span.Length;
            return;
        }

        source.CopyTo( result );
        length = source.Length;
    }
}
