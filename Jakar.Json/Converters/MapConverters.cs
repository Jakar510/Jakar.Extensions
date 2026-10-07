// Jakar.Json
// 10/07/2026

namespace Jakar.Json.Converters;


// ─── Keys ────────────────────────────────────────────────────────────────────



public readonly struct JsonStringKeyConverter : IJsonKeyConverter<string>
{
    public static void WriteKey<TWriter>( ref TWriter writer, string key )
        where TWriter : IJsonWriter, allows ref struct => writer.WritePropertyName(key);

    public static bool TryParseKey( scoped ReadOnlySpan<char> name, [MaybeNullWhen(false)] out string key )
    {
        key = new string(name);
        return true;
    }
}



/// <summary> Any <see cref="ISpanFormattable"/> + <see cref="ISpanParsable{TSelf}"/> key (integers, <see cref="Guid"/>, dates, ...), invariant culture. </summary>
public readonly struct JsonParsableKeyConverter<T> : IJsonKeyConverter<T>
    where T : ISpanFormattable, ISpanParsable<T>
{
    public static void WriteKey<TWriter>( ref TWriter writer, T key )
        where TWriter : IJsonWriter, allows ref struct
    {
        Span<char> buffer = stackalloc char[128];

        if ( key.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture) )
        {
            writer.WritePropertyName(buffer[..written]);
            return;
        }

        ValueStringBuilder text = new(512);

        try
        {
            text.AppendSpanFormattable(key, default, CultureInfo.InvariantCulture);
            writer.WritePropertyName(text.Values);
        }
        finally { text.Dispose(); }
    }

    public static bool TryParseKey( scoped ReadOnlySpan<char> name, [MaybeNullWhen(false)] out T key ) => T.TryParse(name, CultureInfo.InvariantCulture, out key);
}



// ─── Shared map logic ────────────────────────────────────────────────────────



public static class JsonMaps
{
    /// <summary> Writes <c>{ key : value, ... }</c> in enumeration order, or sorted by key when <typeparamref name="TOrder"/> says so (through a pooled copy). </summary>
    public static void Write<TWriter, TKey, TValue, TKeyConverter, TValueConverter, TOrder>( ref TWriter writer, IEnumerable<KeyValuePair<TKey, TValue>> map, int count )
        where TWriter : IJsonWriter, allows ref struct
        where TKey : notnull
        where TKeyConverter : IJsonKeyConverter<TKey>
        where TValueConverter : IJsonConverter<TValue>
        where TOrder : IJsonOrder<TKey>
    {
        writer.WriteStartObject();

        if ( TOrder.Sorted && count > 1 )
        {
            JsonPooledList<KeyValuePair<TKey, TValue>> sorted = new(count);

            try
            {
                Collect(map, ref sorted);
                sorted.Items.Sort(default(KeyOrderComparer<TKey, TValue, TOrder>));
                foreach ( ref readonly KeyValuePair<TKey, TValue> pair in sorted.Span ) { WritePair<TWriter, TKey, TValue, TKeyConverter, TValueConverter>(ref writer, pair); }
            }
            finally { sorted.Dispose(); }
        }
        else
        {
            switch ( map )
            {
                case Dictionary<TKey, TValue> dictionary:
                    foreach ( KeyValuePair<TKey, TValue> pair in dictionary ) { WritePair<TWriter, TKey, TValue, TKeyConverter, TValueConverter>(ref writer, pair); }

                    break;

                case SortedDictionary<TKey, TValue> sorted:
                    foreach ( KeyValuePair<TKey, TValue> pair in sorted ) { WritePair<TWriter, TKey, TValue, TKeyConverter, TValueConverter>(ref writer, pair); }

                    break;

                case OrderedDictionary<TKey, TValue> ordered:
                    foreach ( KeyValuePair<TKey, TValue> pair in ordered ) { WritePair<TWriter, TKey, TValue, TKeyConverter, TValueConverter>(ref writer, pair); }

                    break;

                default:
                    foreach ( KeyValuePair<TKey, TValue> pair in map ) { WritePair<TWriter, TKey, TValue, TKeyConverter, TValueConverter>(ref writer, pair); }

                    break;
            }
        }

        writer.WriteEndObject();
    }

    private static void Collect<TKey, TValue>( IEnumerable<KeyValuePair<TKey, TValue>> map, ref JsonPooledList<KeyValuePair<TKey, TValue>> items )
        where TKey : notnull
    {
        if ( map is Dictionary<TKey, TValue> dictionary )
        {
            foreach ( KeyValuePair<TKey, TValue> pair in dictionary ) { items.Add(pair); }

            return;
        }

        foreach ( KeyValuePair<TKey, TValue> pair in map ) { items.Add(pair); }
    }

    private static void WritePair<TWriter, TKey, TValue, TKeyConverter, TValueConverter>( ref TWriter writer, in KeyValuePair<TKey, TValue> pair )
        where TWriter : IJsonWriter, allows ref struct
        where TKeyConverter : IJsonKeyConverter<TKey>
        where TValueConverter : IJsonConverter<TValue>
    {
        TKeyConverter.WriteKey(ref writer, pair.Key);
        TValue value = pair.Value;
        TValueConverter.Write(ref writer, in value);
    }


    /// <summary> Reads <c>{ ... }</c> into <paramref name="entries"/>. Duplicate keys are left for the caller to reject. </summary>
    internal static bool TryReadEntries<TReader, TKey, TValue, TKeyConverter, TValueConverter>( ref TReader reader, ref JsonPooledList<KeyValuePair<TKey, TValue>> entries )
        where TReader : IJsonReader, allows ref struct
        where TKeyConverter : IJsonKeyConverter<TKey>
        where TValueConverter : IJsonConverter<TValue>
    {
        if ( !reader.TryReadStartObject() ) { return false; }

        while ( true )
        {
            if ( !reader.TryReadProperty(out JsonSpan name, out bool end) ) { return false; }

            if ( end ) { return true; }

            if ( !TryParseKey<TKey, TKeyConverter>(name, out TKey? key) ) { return reader.Fail(JsonErrorKind.InvalidValue); }

            if ( !TValueConverter.TryRead(ref reader, out TValue? value) ) { return false; }

            entries.Add(new KeyValuePair<TKey, TValue>(key, value!));
        }
    }

    private static bool TryParseKey<TKey, TKeyConverter>( scoped in JsonSpan name, [MaybeNullWhen(false)] out TKey key )
        where TKeyConverter : IJsonKeyConverter<TKey>
    {
        if ( !name.IsUtf8 ) { return TKeyConverter.TryParseKey(name.Utf16, out key); }

        char[]? rented = null;

        Span<char> buffer = name.Utf8.Length <= 256
                                ? stackalloc char[256]
                                : rented = ArrayPool<char>.Shared.Rent(name.Utf8.Length);

        try { return TKeyConverter.TryParseKey(buffer[..name.CopyTo(buffer)], out key); }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }


    /// <summary> Reads into a <see cref="Dictionary{TKey,TValue}"/> sized exactly; a repeated key fails with <see cref="JsonErrorKind.DuplicateMember"/>. </summary>
    public static bool TryReadDictionary<TReader, TKey, TValue, TKeyConverter, TValueConverter>( ref TReader reader, [NotNullWhen(true)] out Dictionary<TKey, TValue>? value )
        where TReader : IJsonReader, allows ref struct
        where TKey : notnull
        where TKeyConverter : IJsonKeyConverter<TKey>
        where TValueConverter : IJsonConverter<TValue>
    {
        value = null;
        JsonReaderCheckpoint                       mark    = reader.Checkpoint();
        JsonPooledList<KeyValuePair<TKey, TValue>> entries = default;

        try
        {
            if ( !TryReadEntries<TReader, TKey, TValue, TKeyConverter, TValueConverter>(ref reader, ref entries) ) { return false; }

            Dictionary<TKey, TValue> map = new(entries.Count);

            foreach ( KeyValuePair<TKey, TValue> entry in entries.Span )
            {
                if ( map.TryAdd(entry.Key, entry.Value) ) { continue; }

                reader.Rewind(mark);
                return reader.Fail(JsonErrorKind.DuplicateMember);
            }

            value = map;
            return true;
        }
        finally { entries.Dispose(); }
    }
}



// ─── Concrete maps ───────────────────────────────────────────────────────────



/// <summary> <see cref="Dictionary{TKey,TValue}"/>; also <see cref="IDictionary{TKey,TValue}"/> and <see cref="IReadOnlyDictionary{TKey,TValue}"/> members, read as a <see cref="Dictionary{TKey,TValue}"/>. </summary>
public readonly struct JsonDictionaryConverter<TMap, TKey, TValue, TKeyConverter, TValueConverter, TOrder> : IJsonConverter<TMap>
    where TMap : class, IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
    where TKeyConverter : IJsonKeyConverter<TKey>
    where TValueConverter : IJsonConverter<TValue>
    where TOrder : IJsonOrder<TKey>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TMap value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else
        {
            JsonMaps.Write<TWriter, TKey, TValue, TKeyConverter, TValueConverter, TOrder>(ref writer,
                                                                                          value,
                                                                                          value is IReadOnlyCollection<KeyValuePair<TKey, TValue>> c
                                                                                              ? c.Count
                                                                                              : 16);
        }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TMap value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        if ( !JsonMaps.TryReadDictionary<TReader, TKey, TValue, TKeyConverter, TValueConverter>(ref reader, out Dictionary<TKey, TValue>? map) ) { return false; }

        value = (TMap)(object)map;
        return true;
    }
}



/// <summary> Maps with their own order (<see cref="SortedDictionary{TKey,TValue}"/>, <see cref="OrderedDictionary{TKey,TValue}"/>, <see cref="ImmutableSortedDictionary{TKey,TValue}"/>) and unordered ones built from a <see cref="Dictionary{TKey,TValue}"/> (<see cref="ImmutableDictionary{TKey,TValue}"/>, <see cref="FrozenDictionary{TKey,TValue}"/>, <see cref="ConcurrentDictionary{TKey,TValue}"/>). </summary>
public readonly struct JsonMapConverter<TMap, TKey, TValue, TKeyConverter, TValueConverter, TOrder, TFactory> : IJsonConverter<TMap>
    where TMap : class, IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
    where TKeyConverter : IJsonKeyConverter<TKey>
    where TValueConverter : IJsonConverter<TValue>
    where TOrder : IJsonOrder<TKey>
    where TFactory : IJsonMapFactory<TMap, TKey, TValue>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TMap value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else
        {
            JsonMaps.Write<TWriter, TKey, TValue, TKeyConverter, TValueConverter, TOrder>(ref writer,
                                                                                          value,
                                                                                          value is IReadOnlyCollection<KeyValuePair<TKey, TValue>> c
                                                                                              ? c.Count
                                                                                              : 16);
        }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TMap value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        if ( !JsonMaps.TryReadDictionary<TReader, TKey, TValue, TKeyConverter, TValueConverter>(ref reader, out Dictionary<TKey, TValue>? map) ) { return false; }

        value = TFactory.Create(map);
        return true;
    }
}



/// <summary> Builds a map type from the <see cref="Dictionary{TKey,TValue}"/> a reader produced. </summary>
public interface IJsonMapFactory<TMap, TKey, TValue>
    where TKey : notnull
{
    static abstract TMap Create( Dictionary<TKey, TValue> entries );
}



public static class JsonMapFactories
{
    public readonly struct Sorted<TKey, TValue> : IJsonMapFactory<SortedDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public static SortedDictionary<TKey, TValue> Create( Dictionary<TKey, TValue> entries ) => new(entries);
    }



    /// <summary> Keeps the document's member order. </summary>
    public readonly struct Ordered<TKey, TValue> : IJsonMapFactory<OrderedDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public static OrderedDictionary<TKey, TValue> Create( Dictionary<TKey, TValue> entries ) => new(entries); // Dictionary enumerates in insertion order when nothing was removed
    }



    public readonly struct Immutable<TKey, TValue> : IJsonMapFactory<ImmutableDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public static ImmutableDictionary<TKey, TValue> Create( Dictionary<TKey, TValue> entries ) => entries.ToImmutableDictionary();
    }



    public readonly struct ImmutableSorted<TKey, TValue> : IJsonMapFactory<ImmutableSortedDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public static ImmutableSortedDictionary<TKey, TValue> Create( Dictionary<TKey, TValue> entries ) => entries.ToImmutableSortedDictionary();
    }



    public readonly struct Frozen<TKey, TValue> : IJsonMapFactory<FrozenDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public static FrozenDictionary<TKey, TValue> Create( Dictionary<TKey, TValue> entries ) => entries.ToFrozenDictionary();
    }



    public readonly struct Concurrent<TKey, TValue> : IJsonMapFactory<ConcurrentDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public static ConcurrentDictionary<TKey, TValue> Create( Dictionary<TKey, TValue> entries ) => new(entries);
    }
}
