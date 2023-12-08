// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:19 AM

namespace Jakar.Extensions;


public static partial class Spans
{
    public static Span<T> Replace<T>( this Span<T> value,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          in ReadOnlySpan<T> oldValue,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          in ReadOnlySpan<T> newValue
    )
        where T : unmanaged, IEquatable<T>
    {
        ReadOnlySpan<T> span = value;
        return span.Replace( oldValue, newValue );
    }
    public static Span<T> Replace<T>( this ReadOnlySpan<T> value,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          in ReadOnlySpan<T> oldValue,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          in ReadOnlySpan<T> newValue
    )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + newValue.Length];
        Replace( value, oldValue, newValue, ref buffer, out int length );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), length );
    }
    public static void Replace<T>(
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> value,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> oldValue,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> newValue,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            ref Span<T> buffer,
        out int length
    )
        where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length + newValue.Length - 1, buffer, nameof(buffer) );

        if ( value.Contains( oldValue ) is false )
        {
            value.CopyTo( buffer );
            length = value.Length;
            return;
        }

        int start = value.IndexOf( oldValue );
        int end   = start + oldValue.Length;

        Guard.IsInRangeFor( start, value, nameof(value) );
        ReadOnlySpan<T> valueStart   = value[..start];
        ReadOnlySpan<T> lastNewValue = newValue;
        Join( in valueStart, in lastNewValue, ref buffer, out int first );

        Guard.IsInRangeFor( end, value, nameof(value) );
        ReadOnlySpan<T> firstBuffer = buffer[..first];
        ReadOnlySpan<T> lastEnd     = value[end..];
        Join( in firstBuffer, in lastEnd, ref buffer, out int second );
        length = first + second;
    }


    public static Span<T> Replace<T>( this Span<T> value,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          ref ReadOnlySpan<T> oldValue,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          ref ReadOnlySpan<T> newValue,
                                      T startValue,
                                      T endValue
    )
        where T : unmanaged, IEquatable<T>
    {
        ReadOnlySpan<T> span = value;
        return span.Replace( ref oldValue, ref newValue, startValue, endValue );
    }
    public static Span<T> Replace<T>( this ReadOnlySpan<T> value,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          ref ReadOnlySpan<T> oldValue,
                                  #if NET6_0_OR_GREATER
                                      scoped
                                      #endif
                                          ref ReadOnlySpan<T> newValue,
                                      T startValue,
                                      T endValue
    )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + newValue.Length + 1];
        Replace( value, oldValue, newValue, startValue, endValue, ref buffer, out int length );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), length );
    }
    public static void Replace<T>(
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> source,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> oldValue,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> newValue,

    #if NET6_0_OR_GREATER
        scoped
        #endif
            in T startValue,

    #if NET6_0_OR_GREATER
        scoped
        #endif
            in T endValue,

    #if NET6_0_OR_GREATER
        scoped
        #endif
            ref Span<T> buffer,
        out int length
    )
        where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( source.Length + newValue.Length - 1, buffer, nameof(buffer) );

        if ( !source.Contains( oldValue ) )
        {
            source.CopyTo( buffer );
            length = source.Length;
            return;
        }

        int start = source.IndexOf( oldValue );
        int end   = start + oldValue.Length;

        if ( !oldValue.StartsWith( startValue ) && start > 0 ) { start--; }

        if ( !oldValue.EndsWith( endValue ) ) { end++; }

        Guard.IsInRangeFor( start, source, nameof(source) );
        ReadOnlySpan<T> sourceStart  = source[..start];
        ReadOnlySpan<T> tempNewValue = newValue;
        Join( in sourceStart, in tempNewValue, ref buffer, out int first );

        Guard.IsInRangeFor( end, source, nameof(source) );
        ReadOnlySpan<T> bufferStart = buffer[..first];
        ReadOnlySpan<T> sourceEnd   = source[end..];
        Join( in bufferStart, in sourceEnd, ref buffer, out int second );
        length = first + second;
    }


    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> value, T c )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length];
        RemoveAll( value, c, buffer, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), length );
    }
    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> value,
                                            #if NET6_0_OR_GREATER
                                                scoped
                                                #endif
                                                    in ReadOnlySpan<T> removed
    )
        where T : unmanaged, IEquatable<T>
    {
        Span<T>         buffer = stackalloc T[value.Length];
        ReadOnlySpan<T> temp   = removed;
        RemoveAll( value, temp, buffer, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), length );
    }
    public static Span<T> RemoveAll<T>( this Span<T> value,
                                    #if NET6_0_OR_GREATER
                                        scoped
                                        #endif
                                            in ReadOnlySpan<T> removed
    )
        where T : unmanaged, IEquatable<T>
    {
        ReadOnlySpan<T> span   = value;
        Span<T>         buffer = stackalloc T[value.Length];
        ReadOnlySpan<T> temp   = removed;
        RemoveAll( span, temp, buffer, out int length );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), length );
    }


    public static void RemoveAll<T>(
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> value,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in T c,

    #if NET6_0_OR_GREATER
        scoped
        #endif
            in Span<T> buffer,
        out int length
    )
        where T : unmanaged, IEquatable<T>
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

        length = value.Length - offset;
    }
    public static void RemoveAll<T>(
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> value,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> removed,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in Span<T> buffer,
        out int length
    )
        where T : unmanaged, IEquatable<T>
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

        length = value.Length - offset;
    }


    public static ReadOnlySpan<T> Slice<T>( this ReadOnlySpan<T> value, T startValue, T endValue, bool includeEnds )
        where T : unmanaged, IEquatable<T>
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
        ReadOnlySpan<T> result = value.Slice( start, length );
        return result.AsReadOnlySpan();
        // return MemoryMarshal.CreateReadOnlySpan( in result.GetPinnableReference(), result.Length );
    }
    public static void Slice<T>(
    #if NET6_0_OR_GREATER
        scoped
        #endif
            in ReadOnlySpan<T> value,
        T    startValue,
        T    endValue,
        bool includeEnds,
    #if NET6_0_OR_GREATER
        scoped
        #endif
            ref Span<T> buffer,
        out int length
    )
        where T : unmanaged, IEquatable<T>
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
            length = result.Length;
            return;
        }

        value.CopyTo( buffer );
        length = value.Length;
    }
}
