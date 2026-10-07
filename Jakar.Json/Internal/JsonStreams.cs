// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> Reads whole documents into pooled buffers (SPEC.md §8.3): bounded by <see cref="JsonReaderOptions.MaxDocumentBytes"/>, returned by the caller. </summary>
internal static class JsonStreams
{
    public static byte[] ReadAll( Stream stream, int maxBytes, out int length )
    {
        ArgumentNullException.ThrowIfNull(stream);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(InitialSize(stream, maxBytes));
        length = 0;

        try
        {
            while ( true )
            {
                if ( length == buffer.Length ) { buffer = Grow(buffer, length, maxBytes); }

                int read = stream.Read(buffer, length, buffer.Length - length);
                if ( read == 0 ) { break; }

                length += read;
            }

            if ( length > maxBytes ) { throw TooLarge(length); }

            return buffer;
        }
        catch
        {
            ArrayPool<byte>.Shared.Return(buffer);
            throw;
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))] public static async ValueTask<(byte[] Buffer, int Length)> ReadAllAsync( Stream stream, int maxBytes, CancellationToken token )
    {
        ArgumentNullException.ThrowIfNull(stream);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(InitialSize(stream, maxBytes));
        int    length = 0;

        try
        {
            while ( true )
            {
                if ( length == buffer.Length ) { buffer = Grow(buffer, length, maxBytes); }

                int read = await stream.ReadAsync(buffer.AsMemory(length), token).ConfigureAwait(false);
                if ( read == 0 ) { break; }

                length += read;
            }

            if ( length > maxBytes ) { throw TooLarge(length); }

            return ( buffer, length );
        }
        catch
        {
            ArrayPool<byte>.Shared.Return(buffer);
            throw;
        }
    }

    public static char[] ReadAll( TextReader reader, int maxChars, out int length )
    {
        ArgumentNullException.ThrowIfNull(reader);
        char[] buffer = ArrayPool<char>.Shared.Rent(4096);
        length = 0;

        try
        {
            while ( true )
            {
                if ( length == buffer.Length )
                {
                    if ( buffer.Length > maxChars ) { throw TooLarge(length); }

                    char[] larger = ArrayPool<char>.Shared.Rent((int)Math.Min((long)buffer.Length * 2, Array.MaxLength));
                    buffer.AsSpan(0, length).CopyTo(larger);
                    ArrayPool<char>.Shared.Return(buffer);
                    buffer = larger;
                }

                int read = reader.Read(buffer.AsSpan(length));
                if ( read == 0 ) { break; }

                length += read;
            }

            if ( length > maxChars ) { throw TooLarge(length); }

            return buffer;
        }
        catch
        {
            ArrayPool<char>.Shared.Return(buffer);
            throw;
        }
    }


    private static int InitialSize( Stream stream, int maxBytes )
    {
        long cap = Math.Min((long)maxBytes + 1, Array.MaxLength); // one extra byte, so a document of exactly maxBytes reaches EOF without growing

        long size = stream.CanSeek
                        ? stream.Length - stream.Position + 1
                        : 16 * 1024;

        return (int)Math.Clamp(size, 1, cap);
    }

    private static byte[] Grow( byte[] buffer, int length, int maxBytes )
    {
        if ( buffer.Length > maxBytes ) { throw TooLarge(buffer.Length); }

        byte[] larger = ArrayPool<byte>.Shared.Rent((int)Math.Min(Math.Min((long)buffer.Length * 2, (long)maxBytes + 1), Array.MaxLength));
        buffer.AsSpan(0, length).CopyTo(larger);
        ArrayPool<byte>.Shared.Return(buffer);
        return larger;
    }

    public static JsonReadException TooLarge( int length ) => new(new JsonError(JsonErrorKind.DocumentTooLarge, length, 0, 0));
}
