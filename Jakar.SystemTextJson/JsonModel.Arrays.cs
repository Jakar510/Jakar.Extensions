// Jakar.SystemTextJson
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     Implemented by collections that define their own JSON array contents: e.g. the unfiltered backing store rather than a filtered enumeration,
///     or a consistent snapshot taken under a lock. <see cref="JsonModel.WriteArray{T}(Utf8JsonWriter, IEnumerable{T}, JsonTypeInfo{T})"/> checks for it first.
/// </summary>
public interface IJsonArraySource<T>
{
    void WriteJsonArray( Utf8JsonWriter writer, JsonTypeInfo<T> info );
}



/// <remarks>
///     Root-level JSON arrays are read and written <b> element by element </b> with only the element's <see cref="JsonTypeInfo{T}"/>, so no collection type
///     (<c> List&lt;T&gt; </c>, <c> T[] </c>, <c> ImmutableArray&lt;T&gt; </c>, ...) ever has to be registered in a context. JSON <c> null </c> elements are stored as <c> default(T) </c>,
///     matching System.Text.Json's own collection converters.
/// </remarks>
public static partial class JsonModel
{
    // ─── Reading ──────────────────────────────────────────────────────────────

    /// <summary> Reads the elements of the JSON array the reader is positioned on (<see cref="JsonTokenType.StartArray"/>) and leaves it on the matching <see cref="JsonTokenType.EndArray"/>, as a <see cref="JsonConverter{T}.Read"/> implementation must. </summary>
    /// <exception cref="JsonException"> The reader isn't on a JSON array, or an element is invalid. </exception>
    [MustDisposeResource] public static RentedArray<T> ReadArrayElements<T>( ref Utf8JsonReader reader, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(info);
        if ( reader.TokenType != JsonTokenType.StartArray ) { throw new JsonException($"Expected a JSON array, but found {reader.TokenType}."); }

        RentedArrayBuilder<T> builder = default;

        try
        {
            while ( true )
            {
                if ( !reader.Read() ) { throw new JsonException("Unexpected end of input inside a JSON array."); }

                if ( reader.TokenType == JsonTokenType.EndArray ) { break; }

                // Deserialize consumes exactly one value and leaves the reader on its last token.
                builder.Add(JsonSerializer.Deserialize(ref reader, info)!);
            }

            return builder.ToRentedArray();
        }
        finally { builder.Dispose(); } // no-op once ownership has been transferred
    }


    /// <summary> Reads a root-level JSON array into a pooled buffer, using only <paramref name="info"/> (the element's metadata). Dispose the result when done. </summary>
    /// <exception cref="JsonException"> A JSON <c> null </c> root, a non-array root, an invalid element, or trailing content. </exception>
    [MustDisposeResource] public static RentedArray<T> FromJsonArrayPooled<T>( ReadOnlySpan<byte> utf8Json, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(info);
        Utf8JsonReader reader = new(SkipBom(utf8Json), info.Options.GetReaderOptions());

        if ( !reader.Read() ) { throw new JsonException("Expected a JSON array, but the input is empty."); }

        RentedArray<T> values = ReadArrayElements(ref reader, info);

        try
        {
            if ( reader.Read() ) { throw new JsonException($"Unexpected {reader.TokenType} after the end of the JSON array."); }

            return values;
        }
        catch
        {
            values.Dispose();
            throw;
        }
    }

    /// <inheritdoc cref="FromJsonArrayPooled{T}(ReadOnlySpan{byte}, JsonTypeInfo{T})"/>
    [MustDisposeResource] public static RentedArray<T> FromJsonArrayPooled<T>( string json, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(json);
        return FromJsonArrayPooled(json.AsSpan(), info);
    }

    /// <inheritdoc cref="FromJsonArrayPooled{T}(ReadOnlySpan{byte}, JsonTypeInfo{T})"/>
    [MustDisposeResource] public static RentedArray<T> FromJsonArrayPooled<T>( ReadOnlySpan<char> json, JsonTypeInfo<T> info )
    {
        byte[] utf8 = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(json.Length));

        try
        {
            int length = Encoding.UTF8.GetBytes(json, utf8);
            return FromJsonArrayPooled(new ReadOnlySpan<byte>(utf8, 0, length), info);
        }
        finally { ArrayPool<byte>.Shared.Return(utf8); }
    }

    /// <inheritdoc cref="FromJsonArrayPooled{T}(ReadOnlySpan{byte}, JsonTypeInfo{T})"/>
    [MustDisposeResource] public static async ValueTask<RentedArray<T>> FromJsonArrayPooledAsync<T>( Stream utf8Json, JsonTypeInfo<T> info, CancellationToken token = default )
    {
        ArgumentNullException.ThrowIfNull(utf8Json);
        ArgumentNullException.ThrowIfNull(info);
        RentedArrayBuilder<T> builder = default; // lives in the state machine; mutated in place, never copied

        try
        {
            await foreach ( T? item in JsonSerializer.DeserializeAsyncEnumerable(utf8Json, info, token).ConfigureAwait(false) ) { builder.Add(item!); }

            return builder.ToRentedArray();
        }
        finally { builder.Dispose(); }
    }


    /// <summary> Reads a root-level JSON array and builds <typeparamref name="TCollection"/> from the elements in one call (e.g. <c> static span =&gt; [.. span] </c>). The pooled buffer is returned before this method returns. </summary>
    public static TCollection FromJsonArray<T, TCollection>( ReadOnlySpan<byte> utf8Json, JsonTypeInfo<T> info, Func<ReadOnlySpan<T>, TCollection> build )
    {
        ArgumentNullException.ThrowIfNull(build);
        using RentedArray<T> values = FromJsonArrayPooled(utf8Json, info);
        return build(values.ReadOnly);
    }

    /// <inheritdoc cref="FromJsonArray{T, TCollection}(ReadOnlySpan{byte}, JsonTypeInfo{T}, Func{ReadOnlySpan{T}, TCollection})"/>
    public static TCollection FromJsonArray<T, TCollection>( string json, JsonTypeInfo<T> info, Func<ReadOnlySpan<T>, TCollection> build )
    {
        ArgumentNullException.ThrowIfNull(build);
        using RentedArray<T> values = FromJsonArrayPooled(json, info);
        return build(values.ReadOnly);
    }

    /// <inheritdoc cref="FromJsonArray{T, TCollection}(ReadOnlySpan{byte}, JsonTypeInfo{T}, Func{ReadOnlySpan{T}, TCollection})"/>
    public static async ValueTask<TCollection> FromJsonArrayAsync<T, TCollection>( Stream utf8Json, JsonTypeInfo<T> info, Func<ReadOnlySpan<T>, TCollection> build, CancellationToken token = default )
    {
        ArgumentNullException.ThrowIfNull(build);
        using RentedArray<T> values = await FromJsonArrayPooledAsync(utf8Json, info, token).ConfigureAwait(false);
        return build(values.ReadOnly);
    }


    public static T[]     FromJsonArray<T>( ReadOnlySpan<byte> utf8Json, JsonTypeInfo<T> info ) => FromJsonArray(utf8Json, info, static span => span.ToArray());
    public static T[]     FromJsonArray<T>( string             json,     JsonTypeInfo<T> info ) => FromJsonArray(json,     info, static span => span.ToArray());
    public static List<T> FromJsonList<T>( ReadOnlySpan<byte>  utf8Json, JsonTypeInfo<T> info ) => FromJsonArray(utf8Json, info, static List<T> ( ReadOnlySpan<T> span ) => [.. span]);
    public static List<T> FromJsonList<T>( string              json,     JsonTypeInfo<T> info ) => FromJsonArray(json,     info, static List<T> ( ReadOnlySpan<T> span ) => [.. span]);

    public static ValueTask<T[]>     FromJsonArrayAsync<T>( Stream utf8Json, JsonTypeInfo<T> info, CancellationToken token = default ) => FromJsonArrayAsync(utf8Json, info, static span => span.ToArray(),                        token);
    public static ValueTask<List<T>> FromJsonListAsync<T>( Stream  utf8Json, JsonTypeInfo<T> info, CancellationToken token = default ) => FromJsonArrayAsync(utf8Json, info, static List<T> ( ReadOnlySpan<T> span ) => [.. span], token);


    // ─── Writing ──────────────────────────────────────────────────────────────

    public static void WriteArray<T>( Utf8JsonWriter writer, scoped ReadOnlySpan<T> values, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(info);
        writer.WriteStartArray();
        foreach ( ref readonly T value in values ) { JsonSerializer.Serialize(writer, value, info); }

        writer.WriteEndArray();
    }

    /// <remarks> Serializes over the list's backing array. Don't modify the list while this runs: unlike enumeration, a span doesn't detect modification. </remarks>
    public static void WriteArray<T>( Utf8JsonWriter writer, List<T> values, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(values);
        WriteArray(writer, CollectionsMarshal.AsSpan(values), info);
    }

    /// <summary> Writes <paramref name="values"/> as a JSON array. </summary>
    /// <remarks>
    ///     Runtime fast paths, only for types whose contents are exactly what enumerating them yields: <c> T[] </c>, <c> List&lt;T&gt; </c>, <c> ImmutableArray&lt;T&gt; </c>;
    ///     plus <see cref="IJsonArraySource{T}"/>, checked first. Anything else is enumerated, which keeps its own semantics (locks held for the whole loop, filters, ...).
    /// </remarks>
    public static void WriteArray<T>( Utf8JsonWriter writer, IEnumerable<T> values, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(info);

        switch ( values )
        {
            case IJsonArraySource<T> source:
                source.WriteJsonArray(writer, info);
                return;

            case T[] array:
                WriteArray(writer, new ReadOnlySpan<T>(array), info);
                return;

            case List<T> list:
                WriteArray(writer, CollectionsMarshal.AsSpan(list), info);
                return;

            case ImmutableArray<T> immutable: // boxed
                WriteArray(writer, immutable.AsSpan(), info);
                return;
        }

        writer.WriteStartArray();
        foreach ( T value in values ) { JsonSerializer.Serialize(writer, value, info); }

        writer.WriteEndArray();
    }


    public static string ToJson<T>( scoped ReadOnlySpan<T> values, JsonTypeInfo<T> info, bool? indented = null )
    {
        ArgumentNullException.ThrowIfNull(info);
        ArrayBufferWriter<byte> buffer = new(256);
        using ( Utf8JsonWriter writer = new(buffer, info.Options.GetWriterOptions(indented)) ) { WriteArray(writer, values, info); }

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    public static string ToJson<T>( List<T> values, JsonTypeInfo<T> info, bool? indented = null )
    {
        ArgumentNullException.ThrowIfNull(values);
        return ToJson(CollectionsMarshal.AsSpan(values), info, indented);
    }

    public static string ToJson<T>( IEnumerable<T> values, JsonTypeInfo<T> info, bool? indented = null )
    {
        ArgumentNullException.ThrowIfNull(info);
        ArrayBufferWriter<byte> buffer = new(256);
        using ( Utf8JsonWriter writer = new(buffer, info.Options.GetWriterOptions(indented)) ) { WriteArray(writer, values, info); }

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }



    extension<TSelf>( ReadOnlySpan<TSelf> self )
        where TSelf : IJsonModel<TSelf>
    {
        public string ToJson( bool? indented = null ) => ToJson(self, TSelf.JsonTypeInfo, indented);
    }



    extension<TSelf>( TSelf[] self )
        where TSelf : IJsonModel<TSelf>
    {
        public string ToJson( bool? indented = null ) => ToJson(new ReadOnlySpan<TSelf>(self), TSelf.JsonTypeInfo, indented);
    }



    extension<TSelf>( List<TSelf> self )
        where TSelf : IJsonModel<TSelf>
    {
        /// <remarks> Serializes over the list's backing array. Don't modify the list while this runs: unlike enumeration, a span doesn't detect modification. </remarks>
        public string ToJson( bool? indented = null ) => ToJson(CollectionsMarshal.AsSpan(self), TSelf.JsonTypeInfo, indented);

        public void WriteTo( Utf8JsonWriter writer ) => WriteArray(writer, CollectionsMarshal.AsSpan(self), TSelf.JsonTypeInfo);
    }



    extension<TSelf>( ImmutableArray<TSelf> self )
        where TSelf : IJsonModel<TSelf>
    {
        /// <remarks> No copy; a <see langword="default"/> <see cref="ImmutableArray{T}"/> writes <c> [] </c>. </remarks>
        public string ToJson( bool? indented = null ) => ToJson(self.AsSpan(), TSelf.JsonTypeInfo, indented);
    }



    extension<TSelf>( IEnumerable<TSelf> self )
        where TSelf : IJsonModel<TSelf>
    {
        /// <inheritdoc cref="WriteArray{T}(Utf8JsonWriter, IEnumerable{T}, JsonTypeInfo{T})"/>
        public string ToJson( bool? indented = null ) => ToJson(self, TSelf.JsonTypeInfo, indented);
    }
}
