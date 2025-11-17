// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:19 AM

namespace Jakar.Extensions;


public static partial class Spans
{
    extension<TValue>( scoped ref readonly ReadOnlySpan<TValue> source )
        where TValue : unmanaged, IEquatable<TValue>
    {
        public ReadOnlySpan<TValue> RemoveAll( TValue c )
        {
            Span<TValue> result = stackalloc TValue[source.Length];
            RemoveAll(in source, in c, in result, out int length);

            return result[..length]
               .ToArray();
        }
        public ReadOnlySpan<TValue> RemoveAll( params ReadOnlySpan<TValue> removed )
        {
            Span<TValue>         result = stackalloc TValue[source.Length];
            ReadOnlySpan<TValue> temp   = removed;
            RemoveAll(in source, in temp, in result, out int length);

            return result[..length]
               .ToArray();
        }
    }



    public static Span<TValue> RemoveAll<TValue>( this scoped ref readonly Span<TValue> source, params ReadOnlySpan<TValue> removed )
        where TValue : unmanaged, IEquatable<TValue>
    {
        using IMemoryOwner<TValue> owner  = MemoryPool<TValue>.Shared.Rent(source.Length);
        Span<TValue>               result = owner.Memory.Span;
        ReadOnlySpan<TValue>       temp   = removed;
        ReadOnlySpan<TValue>       span   = source;
        RemoveAll(in span, in temp, in result, out int length);

        return result[..length]
           .ToArray();
    }


    public static void RemoveAll<TValue>( scoped ref readonly ReadOnlySpan<TValue> source, scoped ref readonly TValue value, scoped ref readonly Span<TValue> result, out int length )
        where TValue : unmanaged, IEquatable<TValue>
    {
        Guard.IsInRangeFor(source.Length - 1, result, nameof(result));
        int offset = 0;

        for ( int i = 0; i < source.Length; i++ )
        {
            if ( source[i]
               .Equals(value) )
            {
                offset++;
                continue;
            }

            result[i - offset] = source[i];
        }

        length = source.Length - offset;
    }


    public static void RemoveAll<TValue>( scoped ref readonly ReadOnlySpan<TValue> source, scoped ref readonly ReadOnlySpan<TValue> removed, scoped ref readonly Span<TValue> result, out int length )
        where TValue : unmanaged, IEquatable<TValue>
    {
        Guard.IsInRangeFor(source.Length - 1, result, nameof(result));
        int offset = 0;

        for ( int i = 0; i < source.Length; i++ )
        {
            bool skip = false;

            foreach ( TValue item in removed )
            {
                if ( !source[i]
                        .Equals(item) ) { continue; }

                offset++;
                skip = true;
            }

            if ( skip ) { continue; }

            result[i - offset] = source[i];
        }

        length = source.Length - offset;
    }


    [Pure] public static ReadOnlySpan<TValue> Slice<TValue>( this ReadOnlySpan<TValue> source, TValue startValue, TValue endValue, bool includeEnds )
        where TValue : unmanaged, IEquatable<TValue>
    {
        int start = source.IndexOf(startValue);
        int end   = source.IndexOf(endValue);

        if ( start < 0 && end < 0 ) { return source; }

        start += includeEnds
                     ? 0
                     : 1;

        end += includeEnds
                   ? 1
                   : 0;

        int length = end - start;

        if ( start + length >= source.Length ) { return source[start..]; }

        Guard.IsInRangeFor(start, source, nameof(source));
        Guard.IsInRangeFor(end,   source, nameof(source));
        return source.Slice(start, length);
    }
    public static void Slice<TValue>( scoped ref readonly ReadOnlySpan<TValue> source, TValue startValue, TValue endValue, bool includeEnds, scoped ref Span<TValue> result, out int length )
        where TValue : unmanaged, IEquatable<TValue>
    {
        int start = source.IndexOf(startValue);
        int end   = source.IndexOf(endValue);

        if ( start > 0 && end > 0 )
        {
            start += includeEnds
                         ? 0
                         : 1;

            end += includeEnds
                       ? 1
                       : 0;

            ReadOnlySpan<TValue> span = start + end - start >= source.Length
                                            ? source[start..]
                                            : source.Slice(start, end);

            span.CopyTo(result);
            length = span.Length;
            return;
        }

        source.CopyTo(result);
        length = source.Length;
    }
}
