// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:19 AM

namespace Jakar.Extensions;


public static partial class Spans
{
    public static Span<T> Replace<T>( this ReadOnlySpan<T> value, scoped in ReadOnlySpan<T> oldValue, scoped in ReadOnlySpan<T> newValue )
        where T : unmanaged, IEquatable<T>
    {
        var buffer = new Buffer<T>( value.Length );

        try
        {
            Replace( value, oldValue, newValue, ref buffer );
            int length = buffer.Length;
            T[] array  = AsyncLinq.GetArray<T>( length );
            buffer.Span.CopyTo( array );
            Debug.Assert( length <= array.Length );
            return new Span<T>( array, 0, length );
        }
        finally { buffer.Dispose(); }
    }


    public static void Replace<T>( scoped in ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> oldValue, scoped in ReadOnlySpan<T> newValue, scoped ref Buffer<T> buffer )
        where T : unmanaged, IEquatable<T>
    {
        if ( source.Contains( oldValue ) is false )
        {
            buffer.Append( source );
            return;
        }

        int sourceIndex = 0;

        while ( sourceIndex < source.Length )
        {
            ReadOnlySpan<T> window = source[sourceIndex..];

            if ( window.StartsWith( oldValue ) )
            {
                buffer.EnsureCapacity( newValue.Length );
                buffer.Append( newValue );
                sourceIndex += oldValue.Length;
            }
            else
            {
                buffer.Append( source[sourceIndex] );
                sourceIndex++;
            }
        }
    }


    public static void Replace<T>( scoped in ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> oldValue, scoped in ReadOnlySpan<T> newValue, scoped ref Span<T> buffer, out int length )
        where T : unmanaged, IEquatable<T>
    {
        if ( source.Contains( oldValue ) is false )
        {
            source.CopyTo( buffer );
            length = source.Length;
            return;
        }

        length = 0;
        int sourceIndex = 0;

        while ( sourceIndex < source.Length )
        {
            ReadOnlySpan<T> window = source[sourceIndex..];

            if ( window.StartsWith( oldValue ) )
            {
                if ( length + newValue.Length >= buffer.Length )
                {
                    var newBuffer = new T[buffer.Length * 2];
                    buffer.CopyTo( newBuffer );
                    buffer = newBuffer;
                }

                newValue.CopyTo( buffer[length..] );
                length      += newValue.Length;
                sourceIndex += oldValue.Length;
            }
            else
            {
                if ( length >= buffer.Length )
                {
                    var newBuffer = new T[buffer.Length * 2];
                    buffer.CopyTo( newBuffer );
                    buffer = newBuffer;
                }

                buffer[length] = source[sourceIndex];
                length++;
                sourceIndex++;
            }
        }
    }


    public static Span<T> Replace<T>( this ReadOnlySpan<T> value, scoped ref ReadOnlySpan<T> oldValue, scoped ref ReadOnlySpan<T> newValue, T startValue, T endValue )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length + newValue.Length + 1];
        Replace( value, oldValue, newValue, startValue, endValue, ref buffer, out int length );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), length );
    }


    public static void Replace<T>( scoped in ReadOnlySpan<T> source, scoped in ReadOnlySpan<T> oldValue, scoped in ReadOnlySpan<T> newValue, scoped in T startValue, scoped in T endValue, scoped ref Span<T> buffer, out int length )
        where T : unmanaged, IEquatable<T>
    {
        Guard.IsInRangeFor( source.Length + newValue.Length - 1, buffer, nameof(buffer) );

        if ( source.Contains( oldValue ) is false )
        {
            source.CopyTo( buffer );
            length = source.Length;
            return;
        }

        do
        {
            int start = source.IndexOf( oldValue );
            int end   = start + oldValue.Length;

            if ( oldValue.StartsWith( startValue ) is false && start > 0 ) { start--; }

            if ( oldValue.EndsWith( endValue ) is false ) { end++; }

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
        while ( source.Contains( oldValue ) );
    }


    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> value, T c )
        where T : unmanaged, IEquatable<T>
    {
        Span<T> buffer = stackalloc T[value.Length];
        RemoveAll( value, c, buffer, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), length );
    }


    public static ReadOnlySpan<T> RemoveAll<T>( this ReadOnlySpan<T> value, scoped in ReadOnlySpan<T> removed )
        where T : unmanaged, IEquatable<T>
    {
        Span<T>         buffer = stackalloc T[value.Length];
        ReadOnlySpan<T> temp   = removed;
        RemoveAll( value, temp, buffer, out int length );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), length );
    }


    public static Span<T> RemoveAll<T>( this Span<T> value, scoped in ReadOnlySpan<T> removed )
        where T : unmanaged, IEquatable<T>
    {
        T[]             array  = AsyncLinq.GetArray<T>( value.Length );
        ReadOnlySpan<T> span   = value;
        Span<T>         buffer = array;
        ReadOnlySpan<T> temp   = removed;
        RemoveAll( span, temp, buffer, out int length );
        return new Span<T>( array, 0, length );
    }


    public static void RemoveAll<T>( scoped in ReadOnlySpan<T> value, scoped in T c, scoped in Span<T> buffer, out int length )
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


    public static void RemoveAll<T>( scoped in ReadOnlySpan<T> value, scoped in ReadOnlySpan<T> removed, scoped in Span<T> buffer, out int length )
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
    public static void Slice<T>( scoped in ReadOnlySpan<T> value, T startValue, T endValue, bool includeEnds, scoped ref Span<T> buffer, out int length )
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
