// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  4:46 PM


namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "OutParameterValueIsAlwaysDiscarded.Global")]
public static partial class Spans
{
    [Pure] public static async ValueTask<MemoryStream> ToMemoryStream( this Stream stream )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        MemoryStream        buffer        = new((int)stream.Length);

        await stream.CopyToAsync(buffer)
                    .ConfigureAwait(false);

        buffer.Seek(0, SeekOrigin.Begin);
        return buffer;
    }
    [Pure] public static Memory<byte>         AsMemory( this         MemoryStream stream ) => new(stream.GetBuffer(), 0, (int)stream.Length);
    [Pure] public static ReadOnlyMemory<byte> AsReadOnlyMemory( this MemoryStream stream ) => new(stream.GetBuffer(), 0, (int)stream.Length);
    [Pure] public static ArraySegment<byte>   AsArraySegment( this   MemoryStream stream ) => new(stream.GetBuffer(), 0, (int)stream.Length);
    [Pure] public static ReadOnlySpan<byte>   AsReadOnlySpan( this   MemoryStream stream ) => new(stream.GetBuffer(), 0, (int)stream.Length);
    [Pure] public static Span<byte>           AsSpan( this           MemoryStream stream ) => new(stream.GetBuffer(), 0, (int)stream.Length);


    [Pure] public static bool TryAsSegment<TValue>( this ReadOnlyMemory<TValue> value, out ArraySegment<TValue> result ) => MemoryMarshal.TryGetArray(value, out result);
    [Pure] public static bool TryAsSegment<TValue>( this Memory<TValue>         value, out ArraySegment<TValue> result ) => MemoryMarshal.TryGetArray(value, out result);


    [Pure] public static Memory<TValue>         ToMemory<TValue>( this                     IEnumerable<TValue> value ) => value as TValue[] ?? value.ToArray();
    [Pure] public static ReadOnlyMemory<TValue> ToReadOnlyMemory<TValue>( this             IEnumerable<TValue> value ) => value as TValue[] ?? value.ToArray();
    [Pure] public static Memory<byte>           ToMemory( this scoped ref readonly         Span<byte>          value ) => value.ToArray();
    [Pure] public static ReadOnlyMemory<byte>   ToReadOnlyMemory( this scoped ref readonly ReadOnlySpan<byte>  value ) => value.ToArray();
    [Pure] public static Memory<char>           ToMemory( this scoped ref readonly         ReadOnlySpan<char>  value ) => value.ToArray();
    [Pure] public static ReadOnlyMemory<char>   ToReadOnlyMemory( this scoped ref readonly Span<char>          value ) => value.ToArray();


    [Pure] public static string? ConvertToString( this scoped ref readonly Memory<char> value ) => MemoryMarshal.TryGetString(value, out string? result, out _, out _)
                                                                                                       ? result
                                                                                                       : null;
    [Pure] public static string? ConvertToString( this scoped ref readonly ReadOnlyMemory<char> value ) => MemoryMarshal.TryGetString(value, out string? result, out _, out _)
                                                                                                               ? result
                                                                                                               : null;
    [Pure] public static string? ConvertToString( this scoped ref readonly Memory<char> value, out int start, out int length ) => MemoryMarshal.TryGetString(value, out string? result, out start, out length)
                                                                                                                                      ? result
                                                                                                                                      : null;
    [Pure] public static string? ConvertToString( this scoped ref readonly ReadOnlyMemory<char> value, out int start, out int length ) => MemoryMarshal.TryGetString(value, out string? result, out start, out length)
                                                                                                                                              ? result
                                                                                                                                              : null;


    public static void CopyTo<TValue>( this scoped ref readonly ReadOnlyMemory<TValue> value, scoped ref Span<TValue> buffer )
    {
        Guard.IsInRangeFor(value.Length, buffer, nameof(buffer));
        value.Span.CopyTo(buffer);
    }
    public static void CopyTo<TValue>( this scoped ref readonly Memory<TValue> value, scoped ref Span<TValue> buffer )
    {
        Guard.IsInRangeFor(value.Length, buffer, nameof(buffer));
        value.Span.CopyTo(buffer);
    }
}
