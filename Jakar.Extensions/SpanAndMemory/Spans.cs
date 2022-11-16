#nullable enable
namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public static partial class Spans
{
    [Pure] public static Span<T> AsSpan<T>( this ReadOnlySpan<T> span ) => MemoryMarshal.CreateSpan( ref MemoryMarshal.GetReference( span ), span.Length );
    [Pure]
    public static Span<T> AsSpan<T>( this ReadOnlySpan<T> span, int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateSpan( ref MemoryMarshal.GetReference( span ), length );
    }
    [Pure] public static Span<T> AsSpan<T>( this Span<T> span ) => MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    [Pure]
    public static Span<T> AsSpan<T>( this Span<T> span, int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), length );
    }
    [Pure] public static Memory<T> AsMemory<T>( this T[] span ) => MemoryMarshal.CreateFromPinnedArray( span, 0, span.Length );


    public static void CopyTo<T>( this ReadOnlySpan<T> value, Span<T> buffer ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );

        for ( int i = 0; i < value.Length; i++ ) { buffer[i] = value[i]; }
    }

    public static void CopyTo<T>( this ReadOnlySpan<T> value, Span<T> buffer, T defaultValue ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );

        for ( int i = 0; i < value.Length; i++ ) { buffer[i] = value[i]; }

        for ( int i = value.Length; i < buffer.Length; i++ ) { buffer[i] = defaultValue; }
    }

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


    [Pure]
    public static Span<T> Join<T>( this Span<T> value, Span<T> other ) where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }

    [Pure]
    public static Span<T> Join<T>( this Span<T> value, ReadOnlySpan<T> other ) where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }

    [Pure]
    public static ReadOnlySpan<T> Join<T>( this ReadOnlySpan<T> value, ReadOnlySpan<T> other ) where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, buffer, out int charWritten );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }
    
    public static void Join<T>( ReadOnlySpan<T> first, ReadOnlySpan<T> last, Span<T> buffer, out int charWritten ) where T : IEquatable<T>
    {
        charWritten = first.Length + last.Length;
        Guard.IsInRangeFor( charWritten - 1, buffer, nameof(buffer) );

        for ( int i = 0; i < first.Length; i++ ) { buffer[i] = first[i]; }

        for ( int i = 0; i < last.Length; i++ ) { buffer[i + first.Length] = last[i]; }
    }


    public static int LastIndexOf<T>( this Span<T> value, T c, int endIndex ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( endIndex, value, nameof(value) );

        return value[..endIndex]
           .LastIndexOf( c );
    }

    public static int LastIndexOf<T>( this ReadOnlySpan<T> value, T c, int endIndex ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( endIndex, value, nameof(value) );

        return value[..endIndex]
           .LastIndexOf( c );
    }
}
