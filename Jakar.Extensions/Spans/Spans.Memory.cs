// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  4:46 PM


namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public static partial class Spans
{
    [Pure] public static bool TryAsSegment<T>( this ReadOnlyMemory<T> value, out ArraySegment<T> result ) => MemoryMarshal.TryGetArray( value, out result );
    [Pure] public static bool TryAsSegment<T>( this Memory<T>         value, out ArraySegment<T> result ) => MemoryMarshal.TryGetArray( value, out result );


    [Pure] public static Memory<T>            ToMemory<T>( this         IEnumerable<T>     value ) => value as T[] ?? value.ToArray();
    [Pure] public static ReadOnlyMemory<T>    ToReadOnlyMemory<T>( this IEnumerable<T>     value ) => value as T[] ?? value.ToArray();
    [Pure] public static Memory<byte>         ToMemory( this            Span<byte>         value ) => value.ToArray();
    [Pure] public static ReadOnlyMemory<byte> ToReadOnlyMemory( this    ReadOnlySpan<byte> value ) => value.ToArray();
    [Pure] public static Memory<char>         ToMemory( this            ReadOnlySpan<char> value ) => value.ToArray();
    [Pure] public static ReadOnlyMemory<char> ToReadOnlyMemory( this    Span<char>         value ) => value.ToArray();
    [Pure] public static ReadOnlySpan<T>      AsReadOnlySpan<T>( this   Memory<T>          value ) => value.Span;
    [Pure] public static ReadOnlySpan<T>      AsReadOnlySpan<T>( this   ReadOnlyMemory<T>  value ) => value.Span;
    [Pure] public static Span<T>              AsSpan<T>( this           ReadOnlyMemory<T>  value ) => value.Span.AsSpan();
    [Pure] public static Span<T>              AsSpan<T>( this           Memory<T>          value ) => value.Span;


    [Pure]
    public static string? ConvertToString( this Memory<char> value ) => MemoryMarshal.TryGetString( value, out string? result, out _, out _ )
                                                                            ? result
                                                                            : default;
    [Pure]
    public static string? ConvertToString( this ReadOnlyMemory<char> value ) => MemoryMarshal.TryGetString( value, out string? result, out _, out _ )
                                                                                    ? result
                                                                                    : default;
    [Pure]
    public static string? ConvertToString( this Memory<char> value, out int start, out int length ) => MemoryMarshal.TryGetString( value, out string? result, out start, out length )
                                                                                                           ? result
                                                                                                           : default;
    [Pure]
    public static string? ConvertToString( this ReadOnlyMemory<char> value, out int start, out int length ) => MemoryMarshal.TryGetString( value, out string? result, out start, out length )
                                                                                                                   ? result
                                                                                                                   : default;


    public static void CopyTo<T>( this ReadOnlyMemory<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length, buffer, nameof(buffer) );
        value.Span.CopyTo( buffer );
    }
    public static void CopyTo<T>( this Memory<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length, buffer, nameof(buffer) );
        value.Span.CopyTo( buffer );
    }
}
