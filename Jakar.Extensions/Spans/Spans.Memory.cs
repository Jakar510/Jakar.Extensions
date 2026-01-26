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



    extension( MemoryStream stream )
    {
        [Pure] public Span<byte>           AsSpan()           => new(stream.GetBuffer(), 0, (int)stream.Length);
        [Pure] public ReadOnlySpan<byte>   AsReadOnlySpan()   => new(stream.GetBuffer(), 0, (int)stream.Length);
        [Pure] public Memory<byte>         AsMemory()         => new(stream.GetBuffer(), 0, (int)stream.Length);
        [Pure] public ReadOnlyMemory<byte> AsReadOnlyMemory() => new(stream.GetBuffer(), 0, (int)stream.Length);
        [Pure] public ArraySegment<byte>   AsArraySegment()   => new(stream.GetBuffer(), 0, (int)stream.Length);
    }



    /// <summary> USE WITH CAUTION </summary>
    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ImmutableArray<T> AsImmutableArray<T>( this T[] list ) => ImmutableCollectionsMarshal.AsImmutableArray(list);

    /// <summary> USE WITH CAUTION </summary>
    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Span<T> AsSpan<T>( this List<T> list ) => CollectionsMarshal.AsSpan(list);



    extension<T>( ReadOnlyMemory<T> self )
    {
        [Pure] public bool TryAsSegment( out ArraySegment<T> result ) => MemoryMarshal.TryGetArray(self, out result);

        [Pure] public ImmutableArray<T> AsImmutableArray() => [..self.Span];
    }



    extension<TValue>( Memory<TValue> self )
    {
        [Pure] public bool TryAsSegment( out ArraySegment<TValue> result ) => MemoryMarshal.TryGetArray(self, out result);

        [Pure] public ImmutableArray<TValue> AsImmutableArray() => [..self.Span];
    }



    /// <param name="self">The enumerable source.</param>
    /// <typeparam name="TValue">The type of elements in the sequence.</typeparam>
    extension<TValue>( IEnumerable<TValue> self )
    {
        [Pure] public Memory<TValue>         ToMemory()         => self as TValue[] ?? [..self];
        [Pure] public ReadOnlyMemory<TValue> ToReadOnlyMemory() => self as TValue[] ?? [..self];


        /// <summary>
        /// Tries to divine the number of elements in a sequence without actually enumerating each element.
        /// </summary>
        /// <param name="count">Receives the number of elements in the enumeration, if it could be determined.</param>
        /// <returns><c>true</c> if the count could be determined; <c>false</c> otherwise.</returns>
        public bool TryGetCount( out int count ) => ( (IEnumerable)self ).TryGetCount<TValue>(out count);
    }



    /// <summary>
    /// Tries to divine the number of elements in a sequence without actually enumerating each element.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="sequence">The enumerable source.</param>
    /// <param name="count">Receives the number of elements in the enumeration, if it could be determined.</param>
    /// <returns><c>true</c> if the count could be determined; <c>false</c> otherwise.</returns>
    public static bool TryGetCount<T>( this IEnumerable sequence, out int count )
    {
        switch ( sequence )
        {
            case ICollection collection:
                count = collection.Count;
                return true;

            case ICollection<T> collectionOfT:
                count = collectionOfT.Count;
                return true;

            case IReadOnlyCollection<T> readOnlyCollection:
                count = readOnlyCollection.Count;
                return true;

            default:
                count = 0;
                return false;
        }
    }


    [Pure] public static Memory<byte>         ToMemory( this scoped ref readonly         Span<byte>         value ) => value.ToArray();
    [Pure] public static ReadOnlyMemory<byte> ToReadOnlyMemory( this scoped ref readonly ReadOnlySpan<byte> value ) => value.ToArray();
    [Pure] public static Memory<char>         ToMemory( this scoped ref readonly         ReadOnlySpan<char> value ) => value.ToArray();
    [Pure] public static ReadOnlyMemory<char> ToReadOnlyMemory( this scoped ref readonly Span<char>         value ) => value.ToArray();


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
