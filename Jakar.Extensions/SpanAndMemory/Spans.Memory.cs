﻿// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  4:46 PM


namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" ) ]
public static partial class Spans
{
    public static bool AsMemory( this Span<char>         value, out Memory<char> result ) => MemoryMarshal.TryRead( value.AsBytes(), out result );
    public static bool AsMemory( this ReadOnlySpan<char> value, out Memory<char> result ) => MemoryMarshal.TryRead( value.AsBytes(), out result );
    public static bool AsMemory( this Span<byte>         value, out Memory<byte> result ) => MemoryMarshal.TryRead( value,           out result );
    public static bool AsMemory( this ReadOnlySpan<byte> value, out Memory<byte> result ) => MemoryMarshal.TryRead( value,           out result );


    public static bool AsSegment<T>( this ReadOnlyMemory<T> value, out ArraySegment<T> result ) => MemoryMarshal.TryGetArray( value, out result );
    public static bool AsSegment<T>( this Memory<T>         value, out ArraySegment<T> result ) => MemoryMarshal.TryGetArray( value, out result );


    public static Memory<T>         ToMemory<T>( this         IEnumerable<T>     value ) => value as T[] ?? value.ToArray();
    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>( this IEnumerable<T>     value ) => value as T[] ?? value.ToArray();
    public static Memory<byte>      AsMemory( this            Span<byte>         value ) => MemoryMarshal.Read<Memory<byte>>( value );
    public static Memory<byte>      AsMemory( this            ReadOnlySpan<byte> value ) => MemoryMarshal.Read<Memory<byte>>( value );
    public static Memory<char>      AsMemory( this            Span<char>         value ) => MemoryMarshal.Read<Memory<char>>( value.AsBytes() );
    public static Memory<char>      AsMemory( this            ReadOnlySpan<char> value ) => MemoryMarshal.Read<Memory<char>>( value.AsBytes() );


    public static ReadOnlySpan<T> AsReadOnlySpan<T>( this Memory<T>         value ) => value.Span;
    public static ReadOnlySpan<T> AsReadOnlySpan<T>( this ReadOnlyMemory<T> value ) => value.Span;
    public static Span<T>         AsSpan<T>( this         ReadOnlyMemory<T> value ) => value.Span.AsSpan();
    public static Span<T>         AsSpan<T>( this         Memory<T>         value ) => value.Span;


    public static string? ConvertToString( this Memory<char> value ) => MemoryMarshal.TryGetString( value, out string? result, out _, out _ )
                                                                            ? result
                                                                            : default;
    public static string? ConvertToString( this ReadOnlyMemory<char> value ) => MemoryMarshal.TryGetString( value, out string? result, out _, out _ )
                                                                                    ? result
                                                                                    : default;
    public static string? ConvertToString( this Memory<char> value, out int start, out int length ) => MemoryMarshal.TryGetString( value, out string? result, out start, out length )
                                                                                                           ? result
                                                                                                           : default;
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
