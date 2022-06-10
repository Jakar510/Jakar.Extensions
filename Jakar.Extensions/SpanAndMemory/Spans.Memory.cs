// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  4:46 PM


using System.Runtime.InteropServices;


#nullable enable
namespace Jakar.Extensions.SpanAndMemory;


[SuppressMessage("ReSharper", "OutParameterValueIsAlwaysDiscarded.Global")]
public static partial class Spans
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ConvertToString( this Memory<char> value ) => MemoryMarshal.TryGetString(value, out string? result, out _, out _)
                                                                                ? result
                                                                                : default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ConvertToString( this ReadOnlyMemory<char> value ) => MemoryMarshal.TryGetString(value, out string? result, out _, out _)
                                                                                        ? result
                                                                                        : default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ConvertToString( this Memory<char> value, out int start, out int length ) => MemoryMarshal.TryGetString(value, out string? result, out start, out length)
                                                                                                               ? result
                                                                                                               : default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ConvertToString( this ReadOnlyMemory<char> value, out int start, out int length ) => MemoryMarshal.TryGetString(value, out string? result, out start, out length)
                                                                                                                       ? result
                                                                                                                       : default;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ReadOnlySpan<T> AsReadOnlySpan<T>( this ReadOnlyMemory<T> value ) => value.Span;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>( this ReadOnlyMemory<T> value )
    {
        var temp = new Memory<T>();
        value.CopyTo(temp);
        return temp.Span;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool AsSegment<T>( this ReadOnlyMemory<T> value, out ArraySegment<T> result ) => MemoryMarshal.TryGetArray(value, out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool AsSegment<T>( this Memory<T>         value, out ArraySegment<T> result ) => MemoryMarshal.TryGetArray(value, out result);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>( this ReadOnlyMemory<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor(value.Length, buffer, nameof(buffer));
        value.Span.CopyTo(buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>( this Memory<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor(value.Length, buffer, nameof(buffer));
        value.Span.CopyTo(buffer);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Memory<char> AsMemory( this Span<char>         value ) => MemoryMarshal.Read<Memory<char>>(MemoryMarshal.AsBytes(value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Memory<char> AsMemory( this ReadOnlySpan<char> value ) => MemoryMarshal.Read<Memory<char>>(MemoryMarshal.AsBytes(value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool AsMemory( this         Span<char>         value, out Memory<char> result ) => MemoryMarshal.TryRead(MemoryMarshal.AsBytes(value), out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool AsMemory( this         ReadOnlySpan<char> value, out Memory<char> result ) => MemoryMarshal.TryRead(MemoryMarshal.AsBytes(value), out result);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Memory<byte> AsMemory( this Span<byte>         value ) => MemoryMarshal.Read<Memory<byte>>(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Memory<byte> AsMemory( this ReadOnlySpan<byte> value ) => MemoryMarshal.Read<Memory<byte>>(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool AsMemory( this     Span<byte>         value, out Memory<byte> result ) => MemoryMarshal.TryRead(value, out result);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool AsMemory( this     ReadOnlySpan<byte> value, out Memory<byte> result ) => MemoryMarshal.TryRead(value, out result);
}
