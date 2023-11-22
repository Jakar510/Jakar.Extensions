using System;



namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" ) ]
public static partial class Spans
{
    public static bool IsNullOrWhiteSpace( this Span<char> span )
    {
        if ( span.IsEmpty ) { return true; }

        foreach ( char t in span )
        {
            if ( !char.IsWhiteSpace( t ) ) { return false; }
        }

        return true;
    }

    public static bool IsNullOrWhiteSpace( this ReadOnlySpan<char> span )
    {
        if ( span.IsEmpty ) { return true; }

        foreach ( char t in span )
        {
            if ( !char.IsWhiteSpace( t ) ) { return false; }
        }

        return true;
    }

    public static bool IsNullOrWhiteSpace( this Buffer<char> span )
    {
        if ( span.IsEmpty ) { return true; }

        foreach ( char t in span )
        {
            if ( !char.IsWhiteSpace( t ) ) { return false; }
        }

        return true;
    }

    public static bool IsNullOrWhiteSpace( this ValueStringBuilder span )
    {
        if ( span.IsEmpty ) { return true; }

        foreach ( char t in span )
        {
            if ( !char.IsWhiteSpace( t ) ) { return false; }
        }

        return true;
    }


    public static bool SequenceEquals( this ReadOnlySpan<string> left, ReadOnlySpan<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( ReadOnlySpan<char> parameter in left )
        {
            foreach ( ReadOnlySpan<char> otherParameter in right )
            {
                if ( parameter.SequenceEqual( otherParameter ) is false ) { return false; }
            }
        }

        return true;
    }
    public static bool SequenceEquals( this ImmutableArray<string> left, ReadOnlySpan<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( ReadOnlySpan<char> parameter in left.AsSpan() )
        {
            foreach ( ReadOnlySpan<char> otherParameter in right )
            {
                if ( parameter.SequenceEqual( otherParameter ) is false ) { return false; }
            }
        }

        return true;
    }


    public static int LastIndexOf<T>( this Span<T> value, T c, int endIndex )
        where T : IEquatable<T>
    {
        Guard.IsInRangeFor( endIndex, value, nameof(value) );

        return value[..endIndex].LastIndexOf( c );
    }

    public static int LastIndexOf<T>( this ReadOnlySpan<T> value, T c, int endIndex )
        where T : IEquatable<T>
    {
        Guard.IsInRangeFor( endIndex, value, nameof(value) );

        return value[..endIndex].LastIndexOf( c );
    }


    [ Pure ] public static EnumerateEnumerator<T> Enumerate<T>( this ReadOnlySpan<T> span, int index = -1 ) => new(span, index);


    [ Pure ] public static Memory<T> AsMemory<T>( this T[] span ) => MemoryMarshal.CreateFromPinnedArray( span, 0, span.Length );

    [ Pure ]
    public static ReadOnlySpan<T> Join<T>( this ReadOnlySpan<T> value, ReadOnlySpan<T> other )
        where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, ref buffer, out int charWritten );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }
    [ Pure ] public static Span<T> AsSpan<T>( this ReadOnlySpan<T> span ) => MemoryMarshal.CreateSpan( ref MemoryMarshal.GetReference( span ), span.Length );
    [ Pure ]
    public static Span<T> AsSpan<T>( this ReadOnlySpan<T> span, int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateSpan( ref MemoryMarshal.GetReference( span ), length );
    }
    [ Pure ] public static Span<T> AsSpan<T>( this Span<T> span ) => MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    [ Pure ]
    public static Span<T> AsSpan<T>( this Span<T> span, int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), length );
    }


    [ Pure ]
    public static Span<T> Join<T>( this Span<T> value, Span<T> other )
        where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, ref buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }

    [ Pure ]
    public static Span<T> Join<T>( this Span<T> value, ReadOnlySpan<T> other )
        where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, ref buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }
    public static bool Join<T>( ReadOnlySpan<T> first, ReadOnlySpan<T> last, ref Span<T> buffer, out int charWritten )
    {
        charWritten = first.Length + last.Length;
        Guard.IsInRangeFor( charWritten - 1, buffer, nameof(buffer) );
        return first.TryCopyTo( buffer[..first.Length] ) && last.TryCopyTo( buffer[first.Length..] );
    }


    public static void CopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        value.CopyTo( buffer );
    }
    public static void CopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer, T defaultValue )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        value.CopyTo( buffer );

        buffer[value.Length..].Fill( defaultValue );

        // for ( int i = value.Length; i < buffer.Length; i++ ) { buffer[i] = defaultValue; }
    }


    public static bool TryCopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        return value.TryCopyTo( buffer );
    }
    public static bool TryCopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer, T defaultValue )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );

        if ( !value.TryCopyTo( buffer ) ) { return false; }

        if ( buffer.Length > value.Length ) { buffer[value.Length..].Fill( defaultValue ); }

        return true;
    }
}
