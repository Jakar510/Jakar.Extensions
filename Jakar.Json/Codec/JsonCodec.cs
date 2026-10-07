// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     Reading and writing <see cref="IJsonSerializable{TSelf}"/> types (SPEC.md §8). Generated types expose the same helpers as their own members
///     (<c>invoice.ToJson()</c>, <c>Invoice.FromJson(json)</c>); these are the generic versions they forward to.
/// </summary>
/// <remarks>
///     Allocation budget (§6): writes allocate only the result (<c>string</c> / <c>byte[]</c>) or nothing at all (spans, buffer writers, streams);
///     reads allocate only the result graph. <c>Try*</c> methods never throw for malformed JSON.
/// </remarks>
public static partial class JsonCodec
{
    private const int STACK_CHARS = 512;


    // ─── Write ───────────────────────────────────────────────────────────────

    /// <summary> The JSON (compact unless the type's or <paramref name="options"/>' settings say otherwise): exactly one allocation, the string. </summary>
    /// <exception cref="JsonWriteException"> A value JSON can't hold (NaN, too deep). </exception>
    public static string ToJson<T>( in T value, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonWriter writer = new(stackalloc char[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            T.WriteJson(ref writer, in value);
            return writer.ToString();
        }
        finally { writer.Dispose(); }
    }

    /// <summary> <see cref="IFormattable"/> support: <c>null</c>/<c>""</c> → the type's defaults, <c>"c"</c> → compact, <c>"i"</c> → indented. </summary>
    public static string ToJson<T>( in T value, scoped ReadOnlySpan<char> format )
        where T : IJsonSerializable<T> => ToJson(in value, JsonWriterOptions.FromFormat(format, T.DefaultWriterOptions));

    /// <summary> The JSON as UTF-8: exactly one allocation, the array. </summary>
    public static byte[] ToJsonUtf8<T>( in T value, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonUtf8Writer writer = new(stackalloc byte[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            T.WriteJson(ref writer, in value);
            return writer.ToArray();
        }
        finally { writer.Dispose(); }
    }

    /// <summary> <see cref="ISpanFormattable"/>: writes into <paramref name="destination"/>; <see langword="false"/> (and 0) if it doesn't fit. </summary>
    public static bool TryFormat<T>( in T value, Span<char> destination, out int charsWritten, scoped ReadOnlySpan<char> format = default )
        where T : IJsonSerializable<T>
    {
        JsonWriter writer = new(destination, JsonWriterOptions.FromFormat(format, T.DefaultWriterOptions));

        try
        {
            T.WriteJson(ref writer, in value);

            if ( writer.IsStillIn(destination) )
            {
                charsWritten = writer.Written.Length;
                return true;
            }

            charsWritten = 0;
            return false;
        }
        finally { writer.Dispose(); }
    }

    /// <summary> <see cref="IUtf8SpanFormattable"/>: writes UTF-8 into <paramref name="utf8Destination"/>; <see langword="false"/> (and 0) if it doesn't fit. </summary>
    public static bool TryFormat<T>( in T value, Span<byte> utf8Destination, out int bytesWritten, scoped ReadOnlySpan<char> format = default )
        where T : IJsonSerializable<T>
    {
        JsonUtf8Writer writer = new(utf8Destination, JsonWriterOptions.FromFormat(format, T.DefaultWriterOptions));

        try
        {
            T.WriteJson(ref writer, in value);

            if ( writer.IsStillIn(utf8Destination) )
            {
                bytesWritten = writer.Written.Length;
                return true;
            }

            bytesWritten = 0;
            return false;
        }
        finally { writer.Dispose(); }
    }

    /// <summary> Writes UTF-8 into <paramref name="output"/> (one copy from pooled scratch space). </summary>
    public static void WriteJson<T>( in T value, IBufferWriter<byte> output, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(output);
        JsonUtf8Writer writer = new(stackalloc byte[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            T.WriteJson(ref writer, in value);
            output.Write(writer.Written);
        }
        finally { writer.Dispose(); }
    }

    /// <summary> Writes UTF-16 into <paramref name="output"/>. </summary>
    public static void WriteJson<T>( in T value, IBufferWriter<char> output, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(output);
        JsonWriter writer = new(stackalloc char[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            T.WriteJson(ref writer, in value);
            output.Write(writer.Written);
        }
        finally { writer.Dispose(); }
    }

    /// <summary> Appends the JSON to <paramref name="builder"/>. </summary>
    public static void WriteJson<T>( in T value, ref ValueStringBuilder builder, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonWriter writer = new(stackalloc char[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            T.WriteJson(ref writer, in value);
            builder.Append(writer.Written);
        }
        finally { writer.Dispose(); }
    }

    /// <summary> Writes UTF-8 to <paramref name="stream"/> in <see cref="JsonUtf8Writer.FLUSH_THRESHOLD"/>-byte chunks through a pooled buffer. </summary>
    public static void ToJson<T>( in T value, Stream stream, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonUtf8Writer writer = new(stream, options ?? T.DefaultWriterOptions);

        try
        {
            T.WriteJson(ref writer, in value);
            writer.Flush();
        }
        finally { writer.Dispose(); }
    }

    /// <summary> Serializes into pooled memory synchronously (ref struct writers can't cross an <see langword="await"/>), then awaits the stream write. </summary>
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))] public static async ValueTask ToJsonAsync<T>( T value, Stream stream, JsonWriterOptions? options = null, CancellationToken token = default )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(stream);
        ( byte[] buffer, int length ) = Buffer(value, options ?? T.DefaultWriterOptions);

        try { await stream.WriteAsync(buffer.AsMemory(0, length), token).ConfigureAwait(false); }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    private static (byte[] Buffer, int Length) Buffer<T>( in T value, in JsonWriterOptions options )
        where T : IJsonSerializable<T>
    {
        JsonUtf8Writer writer = new(stackalloc byte[STACK_CHARS], options);

        try
        {
            T.WriteJson(ref writer, in value);
            ReadOnlySpan<byte> written = writer.Written;
            byte[]             buffer  = ArrayPool<byte>.Shared.Rent(Math.Max(written.Length, 1));
            written.CopyTo(buffer);
            return ( buffer, written.Length );
        }
        finally { writer.Dispose(); }
    }

    public static void ToJson<T>( in T value, TextWriter output, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(output);
        JsonWriter writer = new(stackalloc char[STACK_CHARS], options ?? T.DefaultWriterOptions);

        try
        {
            T.WriteJson(ref writer, in value);
            output.Write(writer.Written);
        }
        finally { writer.Dispose(); }
    }


    // ─── Read ────────────────────────────────────────────────────────────────

    /// <exception cref="JsonReadException"> The JSON is malformed, doesn't match <typeparamref name="T"/>, or its root is <c>null</c>. </exception>
    public static T FromJson<T>( string json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(json);
        return FromJson<T>(json.AsSpan(), options);
    }

    /// <inheritdoc cref="FromJson{T}(string, JsonReaderOptions?)"/>
    public static T FromJson<T>( scoped ReadOnlySpan<char> json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => TryRead<T, char>(json, options ?? T.DefaultReaderOptions, out T? value, out JsonError error, out string? path, true)
                                              ? value
                                              : throw new JsonReadException(error, path);

    /// <inheritdoc cref="FromJson{T}(string, JsonReaderOptions?)"/>
    public static T FromJson<T>( scoped ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => TryRead<T, byte>(utf8Json, options ?? T.DefaultReaderOptions, out T? value, out JsonError error, out string? path, true)
                                              ? value
                                              : throw new JsonReadException(error, path);

    /// <inheritdoc cref="FromJson{T}(string, JsonReaderOptions?)"/>
    public static T FromJson<T>( in ReadOnlySequence<byte> utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        if ( utf8Json.IsSingleSegment ) { return FromJson<T>(utf8Json.FirstSpan, options); }

        int    length = checked ((int)utf8Json.Length);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

        try
        {
            utf8Json.CopyTo(buffer);
            return FromJson<T>(buffer.AsSpan(0, length), options);
        }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <summary> Reads <paramref name="utf8Json"/> to the end into pooled memory (at most <see cref="JsonReaderOptions.MaxDocumentBytes"/>), then parses it. </summary>
    /// <inheritdoc cref="FromJson{T}(string, JsonReaderOptions?)"/>
    public static T FromJson<T>( Stream utf8Json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonReaderOptions settings = options ?? T.DefaultReaderOptions;
        byte[]            buffer   = JsonStreams.ReadAll(utf8Json, settings.MaxDocumentBytes, out int length);

        try { return FromJson<T>(buffer.AsSpan(0, length), settings); }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <inheritdoc cref="FromJson{T}(Stream, JsonReaderOptions?)"/>
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))] public static async ValueTask<T> FromJsonAsync<T>( Stream utf8Json, JsonReaderOptions? options = null, CancellationToken token = default )
        where T : IJsonSerializable<T>
    {
        JsonReaderOptions settings = options ?? T.DefaultReaderOptions;
        ( byte[] buffer, int length ) = await JsonStreams.ReadAllAsync(utf8Json, settings.MaxDocumentBytes, token).ConfigureAwait(false);

        try { return FromJson<T>(buffer.AsSpan(0, length), settings); }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <inheritdoc cref="FromJson{T}(string, JsonReaderOptions?)"/>
    public static T FromJson<T>( TextReader json, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        JsonReaderOptions settings = options ?? T.DefaultReaderOptions;
        char[]            buffer   = JsonStreams.ReadAll(json, settings.MaxDocumentBytes, out int length);

        try { return FromJson<T>(buffer.AsSpan(0, length), settings); }
        finally { ArrayPool<char>.Shared.Return(buffer); }
    }


    /// <summary> Exception-free: <see langword="false"/> for <see langword="null"/>/blank input, malformed JSON, or JSON that doesn't match <typeparamref name="T"/>. </summary>
    public static bool TryFromJson<T>( [NotNullWhen(true)] string? json, [MaybeNullWhen(false)] out T value, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T>
    {
        if ( json is null )
        {
            value = default;
            return false;
        }

        return TryRead(json.AsSpan(), options ?? T.DefaultReaderOptions, out value, out _, out _, false);
    }

    /// <inheritdoc cref="TryFromJson{T}(string, out T, JsonReaderOptions?)"/>
    public static bool TryFromJson<T>( scoped ReadOnlySpan<char> json, [MaybeNullWhen(false)] out T value, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => TryRead(json, options ?? T.DefaultReaderOptions, out value, out _, out _, false);

    /// <inheritdoc cref="TryFromJson{T}(string, out T, JsonReaderOptions?)"/>
    public static bool TryFromJson<T>( scoped ReadOnlySpan<byte> utf8Json, [MaybeNullWhen(false)] out T value, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => TryRead(utf8Json, options ?? T.DefaultReaderOptions, out value, out _, out _, false);

    /// <summary> Exception-free, with the error's kind and position. </summary>
    public static bool TryFromJson<T>( scoped ReadOnlySpan<char> json, [MaybeNullWhen(false)] out T value, out JsonError error, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => TryRead(json, options ?? T.DefaultReaderOptions, out value, out error, out _, false);

    /// <inheritdoc cref="TryFromJson{T}(ReadOnlySpan{char}, out T, out JsonError, JsonReaderOptions?)"/>
    public static bool TryFromJson<T>( scoped ReadOnlySpan<byte> utf8Json, [MaybeNullWhen(false)] out T value, out JsonError error, JsonReaderOptions? options = null )
        where T : IJsonSerializable<T> => TryRead(utf8Json, options ?? T.DefaultReaderOptions, out value, out error, out _, false);


    /// <summary> The core read: one root value of <typeparamref name="T"/>, then only trivia. </summary>
    internal static bool TryRead<T, TChar>( scoped ReadOnlySpan<TChar> input, in JsonReaderOptions options, [MaybeNullWhen(false)] out T value, out JsonError error, out string? path, bool wantPath )
        where T : IJsonSerializable<T>
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        JsonReader<TChar> reader = new(input, options);

        try
        {
            if ( T.TryReadJson(ref reader, out value) && reader.TryReadEnd() )
            {
                error = default;
                path  = null;
                return true;
            }

            value = default;

            error = reader.Error.IsError
                        ? reader.Error
                        : JsonLexer<TChar>.CreateError(JsonErrorKind.UnexpectedToken, reader.Position, input);

            path = wantPath
                       ? reader.GetPath()
                       : null; // the path allocates: only for exceptions

            return false;
        }
        finally { reader.Dispose(); }
    }
}
