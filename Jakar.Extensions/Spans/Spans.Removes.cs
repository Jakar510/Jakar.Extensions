// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:19 AM

namespace Jakar.Extensions;


public static partial class Spans
{
    public static ReadOnlySpan<T> RemoveAll<T>( this scoped ref readonly ReadOnlySpan<T> source, T c )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> result = stackalloc T[source.Length];
        RemoveAll( in source, in c, in result, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref result.GetPinnableReference(), length );
    }


    public static ReadOnlySpan<T> RemoveAll<T>( this scoped ref readonly ReadOnlySpan<T> source, scoped ref readonly ReadOnlySpan<T> removed )
        where T : unmanaged, IEquatable<T>
    {
        Span<T>         result = stackalloc T[source.Length];
        ReadOnlySpan<T> temp   = removed;
        RemoveAll( in source, in temp, in result, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref result.GetPinnableReference(), length );
    }


    public static Span<T> RemoveAll<T>( this scoped ref readonly Span<T> source, scoped ref readonly ReadOnlySpan<T> removed )
        where T : unmanaged, IEquatable<T>
    {
        using IMemoryOwner<T> owner  = MemoryPool<T>.Shared.Rent( source.Length );
        Span<T>               result = owner.Memory.Span;
        ReadOnlySpan<T>       temp   = removed;
        ReadOnlySpan<T>       span   = source;
        RemoveAll( in span, in temp, in result, out int length );
        return result[..length].ToArray();
    }


    public static void RemoveAll<T>( scoped ref readonly ReadOnlySpan<T> source, scoped ref readonly T value, scoped ref readonly Span<T> result, out int length )
        where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( source.Length - 1, result, nameof(result) );
        int offset = 0;

        for ( int i = 0; i < source.Length; i++ )
        {
            if ( source[i].Equals( value ) )
            {
                offset++;
                continue;
            }

            result[i - offset] = source[i];
        }

        length = source.Length - offset;
    }


    public static void RemoveAll<T>( scoped ref readonly ReadOnlySpan<T> source, scoped ref readonly ReadOnlySpan<T> removed, scoped ref readonly Span<T> result, out int length )
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


    [Pure]
    public static ReadOnlySpan<T> Slice<T>( this scoped ref readonly ReadOnlySpan<T> source, T startValue, T endValue, bool includeEnds )
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
        return source.Slice( start, length );
    }
    public static void Slice<T>( scoped ref readonly ReadOnlySpan<T> source, T startValue, T endValue, bool includeEnds, scoped ref Span<T> result, out int length )
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
