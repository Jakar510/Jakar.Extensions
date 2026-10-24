// Jakar.Extensions :: Jakar.Extensions
// 11/29/2023  1:49 PM

namespace Jakar.Extensions;


/// <summary>
///     Jakar.Json helpers. Everything is reflection-free and AOT-safe: <c> [JsonModel] </c> types (<see cref="IJsonSerializable{TSelf}"/>) carry their own generated readers and writers,
///     any other shape goes through an <see cref="IJsonConverter{T}"/>, and untyped JSON is a <see cref="JNode"/> tree.
///     <para> Models also get <c> value.ToJson() </c> / <c> T.FromJson(json) </c> from the generator; these are the generic versions. </para>
/// </summary>
public static partial class Json
{
    /// <summary> Lenient reading for untyped JSON (<see cref="JNode"/>): comments and trailing commas allowed, like the models' <c> [assembly: JsonDefaults] </c>. </summary>
    public static JsonReaderOptions ReaderOptions { get; } = new()
                                                             {
                                                                 AllowComments       = true,
                                                                 AllowTrailingCommas = true
                                                             };


    public static JsonWriterOptions GetWriterOptions( bool indented ) => indented
                                                                             ? JsonWriterOptions.Indent
                                                                             : JsonWriterOptions.Default;

    private static JsonWriterOptions? GetWriterOptions<T>( bool? indented )
        where T : IJsonSerializable<T> => indented is { } value
                                              ? T.DefaultWriterOptions with { Indented = value }
                                              : null;



    // ─── Serialize ────────────────────────────────────────────────────────────

    /// <summary> Serializes a model. Same as <c> value.ToJson() </c>; <paramref name="indented"/> overrides the type's layout for this call. </summary>
    public static string Serialize<T>( in T value, bool? indented = null )
        where T : IJsonSerializable<T> => JsonCodec.ToJson(in value, GetWriterOptions<T>(indented));
    public static byte[] SerializeToUtf8Bytes<T>( in T value )
        where T : IJsonSerializable<T> => JsonCodec.ToJsonUtf8(in value);
    public static JNode? SerializeToNode<T>( T value )
        where T : IJsonSerializable<T> => JNode.FromModel(value);
    public static string Serialize<T>( scoped ReadOnlySpan<T> values, bool? indented = null )
        where T : IJsonSerializable<T> => JsonCodec.ToJsonArray(values, GetWriterOptions<T>(indented));
    public static string Serialize<T>( IEnumerable<T> values, bool? indented = null )
        where T : IJsonSerializable<T> => JsonCodec.ToJsonArray(values, GetWriterOptions<T>(indented));
    public static ValueTask SerializeAsync<T>( Stream stream, T value, CancellationToken token = default )
        where T : IJsonSerializable<T> => JsonCodec.ToJsonAsync(value, stream, null, token);


    /// <summary> Any shape <typeparamref name="TConverter"/> handles, e.g. <c> JsonArrayConverter&lt;string, JsonStringConverter&gt; </c>. </summary>
    public static string Serialize<T, TConverter>( in T value, bool indented = false )
        where TConverter : IJsonConverter<T>
    {
        JsonWriter writer = new(stackalloc char[256], GetWriterOptions(indented));

        try
        {
            TConverter.Write(ref writer, in value);
            return writer.ToString();
        }
        finally { writer.Dispose(); }
    }

    /// <summary> A JSON array of strings. </summary>
    public static string Serialize( scoped ReadOnlySpan<string?> values, bool indented = false )
    {
        JArrayNode array = new(values.Length);
        foreach ( string? value in values ) { array.Add(value); }

        return array.ToJson(indented);
    }


    public static string ToJson( this JNode node, bool indented = true ) => node.ToJson(GetWriterOptions(indented));



    // ─── Deserialize ──────────────────────────────────────────────────────────

    /// <inheritdoc cref="Serialize{T, TConverter}"/>
    /// <exception cref="JsonReadException"> Invalid JSON, or JSON that doesn't match <typeparamref name="T"/>. </exception>
    public static T Deserialize<T, TConverter>( scoped ReadOnlySpan<char> json )
        where TConverter : IJsonConverter<T>
    {
        JsonReader<char> reader = new(json, ReaderOptions);

        try
        {
            return TConverter.TryRead(ref reader, out T? value) && reader.TryReadEnd()
                       ? value
                       : throw reader.CreateException();
        }
        finally { reader.Dispose(); }
    }

    /// <inheritdoc cref="Deserialize{T, TConverter}"/>
    public static bool TryDeserialize<T, TConverter>( scoped ReadOnlySpan<char> json, [MaybeNullWhen(false)] out T value )
        where TConverter : IJsonConverter<T>
    {
        JsonReader<char> reader = new(json, ReaderOptions);

        try
        {
            if ( TConverter.TryRead(ref reader, out value) && reader.TryReadEnd() ) { return true; }

            value = default;
            return false;
        }
        finally { reader.Dispose(); }
    }



    extension( string? self )
    {
        /// <summary> Parses JSON into a <see cref="JNode"/>; <see langword="null"/> for blank input, invalid JSON, or a JSON <c> null </c> root. </summary>
        public JNode? TryFromJson()
        {
            if ( string.IsNullOrWhiteSpace(self) ) { return null; }

            if ( JNode.TryParse(self, out JNode? node, out JsonError error, ReaderOptions) ) { return node; }

            SelfLogger.WriteLine("Json parsing error: {Error}", error);
            return null;
        }


        /// <summary> <c> default </c> for blank input, invalid JSON, or JSON that doesn't match <typeparamref name="TValue"/>. </summary>
        public TValue? TryFromJson<TValue>()
            where TValue : IJsonSerializable<TValue> => JsonCodec.TryFromJson<TValue>(self, out TValue? result)
                                                            ? result
                                                            : default;
    }



    extension( string self )
    {
        /// <exception cref="JsonReadException"> Invalid JSON. </exception>
        /// <exception cref="FormatException"> A JSON <c> null </c> root. </exception>
        public JNode FromJson() => JNode.Parse(self, ReaderOptions) ?? throw new FormatException("The JSON root is null.");

        /// <exception cref="JsonReadException"> Invalid JSON, or JSON that doesn't match <typeparamref name="TValue"/>. </exception>
        public TValue FromJson<TValue>()
            where TValue : IJsonSerializable<TValue> => JsonCodec.FromJson<TValue>(self);
    }



    extension( Stream self )
    {
        /// <summary> Reads a JSON document. The caller owns (and disposes) the stream. </summary>
        public async ValueTask<JNode> FromJson( CancellationToken token = default ) => await JNode.ParseAsync(self, ReaderOptions, token).ConfigureAwait(false) ?? throw new FormatException("The JSON root is null.");

        public ValueTask<T> FromJson<T>( CancellationToken token = default )
            where T : IJsonSerializable<T> => JsonCodec.FromJsonAsync<T>(self, null, token);
    }
}
