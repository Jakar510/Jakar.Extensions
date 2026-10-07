// Jakar.Json
// 10/07/2026

using System.Runtime.CompilerServices;
using Jakar.Json.Converters;



namespace Jakar.Json;


public static partial class JsonCodec
{
    // ─── Root arrays: no wrapper type or registration needed ─────────────────

    public static string ToJsonArray<T>( scoped ReadOnlySpan<T> values, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonWriter writer = new(stackalloc char[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            JsonSequences.WriteSpan<JsonWriter, T, JsonModelConverter<T>>(ref writer, values);
            return writer.ToString();
        }
        finally { writer.Dispose(); }
    }

    public static string ToJsonArray<T>( IEnumerable<T> values, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(values);
        JsonWriter writer = new(stackalloc char[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            JsonSequences.WriteEnumerable<JsonWriter, T, JsonModelConverter<T>>(ref writer, values);
            return writer.ToString();
        }
        finally { writer.Dispose(); }
    }

    public static byte[] ToJsonArrayUtf8<T>( scoped ReadOnlySpan<T> values, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonUtf8Writer writer = new(stackalloc byte[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            JsonSequences.WriteSpan<JsonUtf8Writer, T, JsonModelConverter<T>>(ref writer, values);
            return writer.ToArray();
        }
        finally { writer.Dispose(); }
    }

    public static void ToJsonArray<T>( scoped ReadOnlySpan<T> values, Stream stream, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonUtf8Writer writer = new(stream, options ?? T.DefaultWriterOptions);

        try
        {
            JsonSequences.WriteSpan<JsonUtf8Writer, T, JsonModelConverter<T>>(ref writer, values);
            writer.Flush();
        }
        finally { writer.Dispose(); }
    }


    /// <exception cref="JsonReadException"> Malformed JSON, an element that doesn't match <typeparamref name="T"/>, or a <c>null</c> element. </exception>
    public static T[] FromJsonArray<T>( scoped ReadOnlySpan<char> json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => ReadRoot<T[], JsonArrayConverter<T, JsonModelConverter<T>>, char>(json, options ?? T.DefaultReaderOptions);

    /// <inheritdoc cref="FromJsonArray{T}(ReadOnlySpan{char}, JsonReaderOptions?)"/>
    public static T[] FromJsonArray<T>( scoped ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => ReadRoot<T[], JsonArrayConverter<T, JsonModelConverter<T>>, byte>(utf8Json, options ?? T.DefaultReaderOptions);

    /// <inheritdoc cref="FromJsonArray{T}(ReadOnlySpan{char}, JsonReaderOptions?)"/>
    public static T[] FromJsonArray<T>( Stream utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonReaderOptions settings = options ?? T.DefaultReaderOptions;
        byte[]            buffer   = JsonStreams.ReadAll(utf8Json, settings.MaxDocumentBytes, out int length);

        try { return FromJsonArray<T>(buffer.AsSpan(0, length), settings); }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <inheritdoc cref="FromJsonArray{T}(ReadOnlySpan{char}, JsonReaderOptions?)"/>
    public static List<T> FromJsonList<T>( scoped ReadOnlySpan<char> json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => ReadRoot<List<T>, JsonListConverter<T, JsonModelConverter<T>>, char>(json, options ?? T.DefaultReaderOptions);

    /// <inheritdoc cref="FromJsonArray{T}(ReadOnlySpan{char}, JsonReaderOptions?)"/>
    public static List<T> FromJsonList<T>( scoped ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => ReadRoot<List<T>, JsonListConverter<T, JsonModelConverter<T>>, byte>(utf8Json, options ?? T.DefaultReaderOptions);

    /// <summary> The elements in a pooled array: no allocation besides the elements themselves. Dispose the result. </summary>
    /// <inheritdoc cref="FromJsonArray{T}(ReadOnlySpan{char}, JsonReaderOptions?)"/>
    public static JsonPooledArray<T> FromJsonArrayPooled<T>( scoped ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonReader<byte>         reader = new(utf8Json, options ?? T.DefaultReaderOptions);
        scoped JsonPooledList<T> items  = default;
        bool                     keep   = false;

        try
        {
            if ( JsonSequences.TryReadElements<JsonReader<byte>, T, JsonModelConverter<T>>(ref reader, ref items) && reader.TryReadEnd() )
            {
                keep = true;
                return JsonPooledArray<T>.Take(ref items);
            }

            throw reader.CreateException();
        }
        finally
        {
            if ( !keep ) { items.Dispose(); }

            reader.Dispose();
        }
    }

    private static TValue ReadRoot<TValue, TConverter, TChar>( scoped ReadOnlySpan<TChar> input, in JsonReaderOptions options )
        where TConverter : IJsonConverter<TValue>
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        JsonReader<TChar> reader = new(input, options);

        try
        {
            if ( TConverter.TryRead(ref reader, out TValue? value) && reader.TryReadEnd() ) { return value!; }

            throw reader.CreateException();
        }
        finally { reader.Dispose(); }
    }


    // ─── NDJSON: one document per line (SPEC.md §8.2) ────────────────────────

    /// <summary>
    ///     Streams one <typeparamref name="T"/> per line. Memory is bounded by the longest line (at most <see cref="JsonReaderOptions.MaxDocumentBytes"/>), not the stream.
    ///     <c>\n</c> and <c>\r\n</c> both work; blank lines are skipped; a final line without a newline is still read.
    /// </summary>
    /// <exception cref="JsonReadException"> A line is malformed (<see cref="JsonError.Line"/> is the line number) or too long. </exception>
    public static IEnumerable<T> ReadLines<T>( Stream utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(utf8Json);
        return ReadLinesIterator<T>(utf8Json, options ?? T.DefaultReaderOptions);
    }

    private static IEnumerable<T> ReadLinesIterator<T>( Stream stream, JsonReaderOptions options )
        where T : IJsonSerializable<T>
    {
        NdjsonBuffer buffer = new(options.MaxDocumentBytes);

        try
        {
            while ( true )
            {
                while ( buffer.TryTakeLine(out int start, out int length, out int line, out long offset) )
                {
                    if ( NdjsonBuffer.IsBlank(buffer.Bytes.AsSpan(start, length)) ) { continue; }

                    yield return ParseLine<T>(buffer.Bytes.AsSpan(start, length), options, line, offset);
                }

                if ( buffer.Completed ) { yield break; }

                buffer.Fill(stream);
            }
        }
        finally { buffer.Dispose(); }
    }

    /// <inheritdoc cref="ReadLines{T}(Stream, JsonReaderOptions?)"/>
    public static async IAsyncEnumerable<T> ReadLinesAsync<T>( Stream utf8Json, JsonReaderOptions? options = null, [EnumeratorCancellation] CancellationToken token = default )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(utf8Json);
        JsonReaderOptions settings = options ?? T.DefaultReaderOptions;
        NdjsonBuffer      buffer   = new(settings.MaxDocumentBytes);

        try
        {
            while ( true )
            {
                while ( buffer.TryTakeLine(out int start, out int length, out int line, out long offset) )
                {
                    if ( NdjsonBuffer.IsBlank(buffer.Bytes.AsSpan(start, length)) ) { continue; }

                    yield return ParseLine<T>(buffer.Bytes.AsSpan(start, length), settings, line, offset);
                }

                if ( buffer.Completed ) { yield break; }

                await buffer.FillAsync(utf8Json, token).ConfigureAwait(false);
            }
        }
        finally { buffer.Dispose(); }
    }

    private static T ParseLine<T>( scoped ReadOnlySpan<byte> line, in JsonReaderOptions options, int lineNumber, long offset )
        where T : IJsonSerializable<T>
    {
        if ( TryRead<T, byte>(line, options, out T? value, out JsonError error, out string? path, true) ) { return value; }

        // Positions are within the line; report the stream offset and the line number instead of the line's own line 1.
        throw new JsonReadException(new JsonError(error.Kind, (int)Math.Min(offset + error.Position, int.MaxValue), lineNumber, error.Column), path);
    }

    /// <summary> Writes each value as compact JSON followed by <c>\n</c>. </summary>
    public static void WriteLines<T>( Stream utf8Json, IEnumerable<T> values, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(utf8Json);
        ArgumentNullException.ThrowIfNull(values);
        JsonWriterOptions settings = ( options ?? T.DefaultWriterOptions ) with { Indented = false };

        foreach ( T value in values )
        {
            JsonUtf8Writer writer = new(utf8Json, settings);

            try
            {
                T.WriteJson(ref writer, in value);
                writer.Flush();
            }
            finally { writer.Dispose(); }

            utf8Json.WriteByte((byte)'\n');
        }
    }



    /// <summary> A pooled byte window over a stream that hands out complete lines. </summary>
    private sealed class NdjsonBuffer( int maxLineBytes ) : IDisposable
    {
        private int  __start;   // the next unread line starts here
        private int  __end;     // bytes [0, __end) are filled
        private int  __scanned; // [__start, __scanned) has no newline
        private int  __line;    // 1-based number of the next line
        private long __offset;  // stream offset of Bytes[0]
        private bool __eof;

        public byte[] Bytes     { get; private set; } = ArrayPool<byte>.Shared.Rent(16 * 1024);
        public bool   Completed => __eof && __start >= __end;


        public bool TryTakeLine( out int start, out int length, out int line, out long offset )
        {
            int newLine = Bytes.AsSpan(__scanned, __end - __scanned).IndexOf((byte)'\n');

            if ( newLine < 0 )
            {
                __scanned = __end;

                if ( !__eof || __start >= __end )
                {
                    start  = length = line = 0;
                    offset = 0;
                    return false;
                }

                newLine = __end - __scanned; // the final line has no newline
            }

            int stop = __scanned + newLine;
            start  = __start;
            length = stop - __start;
            if ( length > maxLineBytes ) { throw JsonStreams.TooLarge(length); }

            if ( length > 0 && Bytes[stop - 1] == (byte)'\r' ) { length--; }

            line   = ++__line;
            offset = __offset + __start;

            __start   = Math.Min(stop + 1, __end);
            __scanned = __start;
            return true;
        }

        public void Fill( Stream stream )
        {
            Prepare();
            if ( __end - __start > maxLineBytes ) { throw JsonStreams.TooLarge(__end - __start); } // one unfinished line is already too long

            int read = stream.Read(Bytes, __end, Bytes.Length - __end);
            if ( read == 0 ) { __eof = true; }

            __end += read;
        }

        public async ValueTask FillAsync( Stream stream, CancellationToken token )
        {
            Prepare();
            if ( __end - __start > maxLineBytes ) { throw JsonStreams.TooLarge(__end - __start); }

            int read = await stream.ReadAsync(Bytes.AsMemory(__end, Bytes.Length - __end), token).ConfigureAwait(false);
            if ( read == 0 ) { __eof = true; }

            __end += read;
        }

        /// <summary> Moves the unread tail to the front, or grows the buffer when one line fills it. </summary>
        private void Prepare()
        {
            if ( __start > 0 )
            {
                Bytes.AsSpan(__start, __end - __start).CopyTo(Bytes);
                __offset  += __start;
                __end     -= __start;
                __scanned -= __start;
                __start   =  0;
            }

            if ( __end < Bytes.Length ) { return; }

            if ( Bytes.Length > maxLineBytes ) { throw JsonStreams.TooLarge(__end); }

            byte[] larger = ArrayPool<byte>.Shared.Rent((int)Math.Min((long)Bytes.Length * 2, Array.MaxLength));
            Bytes.AsSpan(0, __end).CopyTo(larger);
            ArrayPool<byte>.Shared.Return(Bytes);
            Bytes = larger;
        }

        public static bool IsBlank( ReadOnlySpan<byte> line ) => line.IndexOfAnyExcept(" \t\r"u8) < 0;

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Bytes);
            Bytes = [];
        }
    }
}



/// <summary> Elements read into a pooled array (<see cref="JsonCodec.FromJsonArrayPooled{T}"/>). Dispose it to return the array; the span is invalid afterwards. </summary>
public struct JsonPooledArray<T> : IDisposable
{
    private T[]? __array;
    private int  __length;

    public readonly ReadOnlySpan<T> Span   => __array.AsSpan(0, __length);
    public readonly int             Length => __length;


    internal static JsonPooledArray<T> Take( ref JsonPooledList<T> items )
    {
        T[] array = ArrayPool<T>.Shared.Rent(Math.Max(items.Count, 1));
        items.Span.CopyTo(array);

        JsonPooledArray<T> result = new()
                                    {
                                        __array  = array,
                                        __length = items.Count
                                    };

        items.Dispose();
        return result;
    }

    public void Dispose()
    {
        T[]? array = __array;
        __array  = null;
        __length = 0;
        if ( array is not null ) { ArrayPool<T>.Shared.Return(array, RuntimeHelpers.IsReferenceOrContainsReferences<T>()); }
    }
}
