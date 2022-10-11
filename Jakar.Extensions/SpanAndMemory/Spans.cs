#nullable enable
namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public static partial class Spans
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNullOrWhiteSpace( this Span<char> span )
    {
        if (span.IsEmpty) { return true; }

        foreach (char t in span)
        {
            if (!char.IsWhiteSpace( t )) { return false; }
        }

        return true;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNullOrWhiteSpace( this ReadOnlySpan<char> span )
    {
        if (span.IsEmpty) { return true; }

        foreach (char t in span)
        {
            if (!char.IsWhiteSpace( t )) { return false; }
        }

        return true;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNullOrWhiteSpace( this Buffer<char> span )
    {
        if (span.IsEmpty) { return true; }

        foreach (char t in span)
        {
            if (!char.IsWhiteSpace( t )) { return false; }
        }

        return true;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNullOrWhiteSpace( this ValueStringBuilder span )
    {
        if (span.IsEmpty) { return true; }

        foreach (char t in span)
        {
            if (!char.IsWhiteSpace( t )) { return false; }
        }

        return true;
    }


    [Pure] [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Span<T> AsSpan<T>( this ReadOnlySpan<T> span ) => span.AsSpan( span.Length );
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Span<T> AsSpan<T>( this ReadOnlySpan<T> span, in int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateSpan( ref MemoryMarshal.GetReference( span ), length );
    }
    [Pure] [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ReadOnlySpan<T> AsSpan<T>( this Span<T> span ) => span.AsSpan( span.Length );
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ReadOnlySpan<T> AsSpan<T>( this Span<T> span, in int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), length );
    }
    [Pure] [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Memory<T> AsSpan<T>( this T[] span ) => MemoryMarshal.CreateFromPinnedArray( span, 0, span.Length );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOf<T>( this Span<T> value, in T c, in int endIndex ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( endIndex, value, nameof(value) );

        return value[..endIndex]
           .LastIndexOf( c );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOf<T>( this ReadOnlySpan<T> value, in T c, in int endIndex ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( endIndex, value, nameof(value) );

        return value[..endIndex]
           .LastIndexOf( c );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Span<T> Join<T>( this Span<T> value, in Span<T> other ) where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, in buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Span<T> Join<T>( this Span<T> value, in ReadOnlySpan<T> other ) where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, in buffer, out int charWritten );
        return MemoryMarshal.CreateSpan( ref buffer.GetPinnableReference(), charWritten );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ReadOnlySpan<T> Join<T>( this ReadOnlySpan<T> value, in ReadOnlySpan<T> other ) where T : unmanaged, IEquatable<T>
    {
        int     size   = value.Length + other.Length;
        Span<T> buffer = stackalloc T[size];
        Join( value, other, in buffer, out int charWritten );
        return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), charWritten );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Join<T>( in ReadOnlySpan<T> first, in ReadOnlySpan<T> last, in Span<T> buffer, out int charWritten ) where T : IEquatable<T>
    {
        charWritten = first.Length + last.Length;
        Guard.IsInRangeFor( charWritten - 1, buffer, nameof(buffer) );

        for (int i = 0; i < first.Length; i++) { buffer[i] = first[i]; }

        for (int i = 0; i < last.Length; i++) { buffer[i + first.Length] = last[i]; }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void CopyTo<T>( this ReadOnlySpan<T> value, in Span<T> buffer ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );

        for (int i = 0; i < value.Length; i++) { buffer[i] = value[i]; }
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void CopyTo<T>( this ReadOnlySpan<T> value, in Span<T> buffer, in T defaultValue ) where T : IEquatable<T>
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );

        for (int i = 0; i < value.Length; i++) { buffer[i] = value[i]; }

        for (int i = value.Length; i < buffer.Length; i++) { buffer[i] = defaultValue; }
    }
}
