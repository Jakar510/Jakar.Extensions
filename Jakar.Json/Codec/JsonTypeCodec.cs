// Jakar.Json
// 10/14/2026

namespace Jakar.Json;


/// <summary>
///     Reads and writes one type through <see cref="JsonCodec"/> when only its <see cref="System.Type"/> is known at run time, for frameworks that pick a serializer per type (ASP.NET Core
///     formatters, message buses). Get one from <see cref="JsonCodecRegistry"/>. Reflection-free: each codec is created by generic code for its type when the type is registered.
/// </summary>
public abstract class JsonTypeCodec
{
    private protected JsonTypeCodec( Type type ) => Type = type;


    /// <summary> The type this codec reads and writes. </summary>
    public Type Type { get; }

    /// <summary> The type's own reader settings (or its element type's, for a collection): <see cref="JsonReaderOptions.MaxDocumentBytes"/> bounds what a caller should buffer. </summary>
    public abstract JsonReaderOptions DefaultReaderOptions { get; }

    /// <summary> The type's own writer settings (or its element type's, for a collection). </summary>
    public abstract JsonWriterOptions DefaultWriterOptions { get; }


    /// <summary> Reads one <see cref="Type"/> value; <see langword="null"/> for a JSON <c>null</c> root when <see cref="Type"/> is a reference type. </summary>
    /// <exception cref="JsonReadException"> The JSON is malformed or doesn't match <see cref="Type"/>. </exception>
    public abstract object? Read( scoped ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null );

    /// <inheritdoc cref="Read(ReadOnlySpan{byte}, JsonReaderOptions?)"/>
    public object? Read( in ReadOnlySequence<byte> utf8Json, JsonReaderOptions? options = null )
    {
        if ( utf8Json.IsSingleSegment ) { return Read(utf8Json.FirstSpan, options); }

        int    length = checked ((int)utf8Json.Length);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

        try
        {
            utf8Json.CopyTo(buffer);
            return Read(buffer.AsSpan(0, length), options);
        }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <summary> Reads <paramref name="utf8Json"/> to the end into pooled memory (at most <see cref="JsonReaderOptions.MaxDocumentBytes"/>), then parses it. </summary>
    /// <inheritdoc cref="Read(ReadOnlySpan{byte}, JsonReaderOptions?)"/>
    public async ValueTask<object?> ReadAsync( Stream utf8Json, JsonReaderOptions? options = null, CancellationToken token = default )
    {
        JsonReaderOptions settings = options ?? DefaultReaderOptions;
        ( byte[] buffer, int length ) = await JsonStreams.ReadAllAsync(utf8Json, settings.MaxDocumentBytes, token).ConfigureAwait(false);

        try { return Read(buffer.AsSpan(0, length), settings); }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <summary> Writes <paramref name="value"/> (a <see cref="Type"/>, or <see langword="null"/>) as UTF-8 into <paramref name="output"/>, e.g. an ASP.NET Core <c>PipeWriter</c>. </summary>
    /// <exception cref="ArgumentException"> <paramref name="value"/> isn't a <see cref="Type"/>. </exception>
    public abstract void Write( object? value, IBufferWriter<byte> output, JsonWriterOptions? options = null );


    /// <summary> The codec for a sequence of this type (<see cref="Type"/> is the element type). </summary>
    internal abstract JsonTypeCodec CreateSequence( Type sequenceType, bool asArray );


    private protected ArgumentException NotThisType( object value ) => new($"{value.GetType()} isn't a {Type}.", nameof(value));

    /// <summary> A root <c>null</c>, allowing whitespace around it. </summary>
    private protected static bool IsNullLiteral( ReadOnlySpan<byte> utf8Json ) => utf8Json.Length <= 64 && utf8Json.Trim(" \t\r\n"u8).SequenceEqual("null"u8);
}



/// <summary> One <c>[JsonModel]</c> type. </summary>
internal sealed class JsonModelCodec<T>() : JsonTypeCodec(typeof(T))
    where T : IJsonSerializable<T>
{
    public override JsonReaderOptions DefaultReaderOptions => T.DefaultReaderOptions;
    public override JsonWriterOptions DefaultWriterOptions => T.DefaultWriterOptions;


    public override object? Read( scoped ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null ) => !typeof(T).IsValueType && IsNullLiteral(utf8Json)
                                                                                                                 ? null
                                                                                                                 : JsonCodec.FromJson<T>(utf8Json, options);

    public override void Write( object? value, IBufferWriter<byte> output, JsonWriterOptions? options = null )
    {
        ArgumentNullException.ThrowIfNull(output);

        switch ( value )
        {
            case null:
                output.Write("null"u8);
                return;

            case T model:
                JsonCodec.WriteJson(in model, output, options);
                return;

            default: throw NotThisType(value);
        }
    }

    internal override JsonTypeCodec CreateSequence( Type sequenceType, bool asArray ) => new JsonSequenceCodec<T>(sequenceType, asArray);
}



/// <summary> <c>T[]</c>, <c>List&lt;T&gt;</c> or a collection interface <c>List&lt;T&gt;</c> implements, of a <c>[JsonModel]</c> type: read as a JSON array; written from any <see cref="IEnumerable{T}"/>. </summary>
internal sealed class JsonSequenceCodec<T>( Type sequenceType, bool asArray ) : JsonTypeCodec(sequenceType)
    where T : IJsonSerializable<T>
{
    public override JsonReaderOptions DefaultReaderOptions => T.DefaultReaderOptions;
    public override JsonWriterOptions DefaultWriterOptions => T.DefaultWriterOptions;


    public override object? Read( scoped ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
    {
        if ( IsNullLiteral(utf8Json) ) { return null; }

        return asArray
                   ? JsonCodec.FromJsonArray<T>(utf8Json, options)
                   : JsonCodec.FromJsonList<T>(utf8Json, options);
    }

    public override void Write( object? value, IBufferWriter<byte> output, JsonWriterOptions? options = null )
    {
        ArgumentNullException.ThrowIfNull(output);

        switch ( value )
        {
            case null:
                output.Write("null"u8);
                return;

            case T[] array:
                JsonCodec.WriteJsonArray(array.AsSpan(), output, options);
                return;

            case List<T> list:
                JsonCodec.WriteJsonArray((ReadOnlySpan<T>)CollectionsMarshal.AsSpan(list), output, options);
                return;

            case IEnumerable<T> values:
                JsonCodec.WriteJsonArray(values, output, options);
                return;

            default: throw NotThisType(value);
        }
    }

    internal override JsonTypeCodec CreateSequence( Type type, bool array ) => throw new NotSupportedException("Nested sequences of models aren't resolved at run time; wrap them in a [JsonModel] type.");
}



/// <summary>
///     The <see cref="JsonTypeCodec"/> of every <c>[JsonModel]</c> type, registered by a generated module initializer in each assembly that declares them (non-generic types that code
///     outside the type can name). Lookups also resolve <c>T[]</c>, <c>List&lt;T&gt;</c>, <see cref="IEnumerable{T}"/>, <see cref="IReadOnlyList{T}"/>, <see cref="IReadOnlyCollection{T}"/>,
///     <see cref="IList{T}"/> and <see cref="ICollection{T}"/> of a registered <c>T</c>.
/// </summary>
/// <remarks> Reflection-free and trimming-safe: registration runs generic code for each type; collection lookups only inspect the requested <see cref="Type"/>. </remarks>
public static class JsonCodecRegistry
{
    private static readonly ConcurrentDictionary<Type, JsonTypeCodec>  __models      = new();
    private static readonly ConcurrentDictionary<Type, JsonTypeCodec?> __resolved    = new();
    private static readonly ConcurrentDictionary<System.Reflection.Module, bool> __initialized = new();


    /// <summary> Called by generated code. The first registration of a type wins. </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] public static void Register<T>()
        where T : IJsonSerializable<T>
    {
        if ( __models.TryAdd(typeof(T), new JsonModelCodec<T>()) ) { __resolved.TryRemove(typeof(T), out _); } // forget a miss cached before this assembly loaded
    }


    public static bool IsRegistered( Type type ) => TryGet(type, out _);


    /// <summary> The codec for <paramref name="type"/>: a registered model, or a supported collection of one. </summary>
    public static bool TryGet( Type type, [NotNullWhen(true)] out JsonTypeCodec? codec )
    {
        ArgumentNullException.ThrowIfNull(type);

        if ( __models.TryGetValue(type, out codec) ) { return true; }

        codec = __resolved.GetOrAdd(type, static type => Resolve(type));
        return codec is not null;
    }


    private static JsonTypeCodec? Resolve( Type type )
    {
        if ( type.IsSZArray )
        {
            return type.GetElementType() is { } itemType && TryGetModel(itemType, out JsonTypeCodec? model)
                       ? model.CreateSequence(type, true)
                       : null;
        }

        if ( TryGetModel(type, out JsonTypeCodec? self) ) { return self; }

        if ( !type.IsConstructedGenericType ) { return null; }

        Type definition = type.GetGenericTypeDefinition();

        bool isSequence = definition == typeof(List<>)              ||
                          definition == typeof(IEnumerable<>)       ||
                          definition == typeof(IReadOnlyList<>)     ||
                          definition == typeof(IReadOnlyCollection<>) ||
                          definition == typeof(IList<>)             ||
                          definition == typeof(ICollection<>);

        return isSequence && TryGetModel(type.GenericTypeArguments[0], out JsonTypeCodec? element)
                   ? element.CreateSequence(type, false)
                   : null;
    }

    /// <summary>
    ///     A registered model. A module initializer runs only once code in its module runs, and a framework can meet a model's <see cref="Type"/> first (an action's parameter type, say),
    ///     so a miss runs the type's module initializer once and looks again.
    /// </summary>
    private static bool TryGetModel( Type type, [NotNullWhen(true)] out JsonTypeCodec? codec )
    {
        if ( __models.TryGetValue(type, out codec) ) { return true; }

        if ( !__initialized.TryAdd(type.Module, true) ) { return false; }

        RuntimeHelpers.RunModuleConstructor(type.Module.ModuleHandle);
        return __models.TryGetValue(type, out codec);
    }
}
