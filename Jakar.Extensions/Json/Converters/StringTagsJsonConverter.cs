// Jakar.Extensions :: Jakar.Extensions
// System.Text.Json contracts for the RFC 7807 detail bag.

namespace Jakar.Extensions;


/// <summary>
///     <see cref="Pair"/> and <see cref="StringTags"/> carry their payload in public <c> readonly </c> fields, and System.Text.Json ignores fields unless <c> JsonSerializerOptions.IncludeFields </c> is set. Without these converters STJ writes <see cref="Error.Details"/> as <c> {"IsEmpty":true} </c> and reads it back empty, so every tag attached to an error is dropped on the way through Minimal API - Newtonsoft is unaffected because it serializes public fields by default.
///     <para> The wire shape is deliberately identical to Newtonsoft's ( <c> {"Tags":[{"Key":…,"Value":…}],"Entries":[…]} </c> ) so the same payload survives either serializer. Property names honour <c> JsonSerializerOptions.PropertyNamingPolicy </c> on write and are matched case-insensitively on read, which is what keeps output consistent under <c> JsonSerializerDefaults.Web </c> . </para>
///     <para> Everything here reads and writes the reader/writer directly rather than calling back into <c> JsonSerializer </c> , so no reflection is involved and the assembly stays trim- and AOT-clean. </para>
/// </summary>
public sealed class PairJsonConverter : JsonConverter<Pair>
{
    public static readonly PairJsonConverter Instance = new();


    internal static string GetName( JsonSerializerOptions options, string name ) => options.PropertyNamingPolicy?.ConvertName(name) ?? name;


    public override Pair Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType is JsonTokenType.Null ) { return default; }

        if ( reader.TokenType is not JsonTokenType.StartObject ) { throw new JsonException($"Expected {nameof(JsonTokenType.StartObject)} when parsing {nameof(Pair)}, found {reader.TokenType}."); }

        string  key   = EMPTY;
        string? value = null;

        while ( reader.Read() )
        {
            if ( reader.TokenType is JsonTokenType.EndObject ) { return new Pair(key, value); }

            if ( reader.TokenType is not JsonTokenType.PropertyName ) { throw new JsonException($"Expected a property name when parsing {nameof(Pair)}, found {reader.TokenType}."); }

            string name = reader.GetString() ?? EMPTY;
            reader.Read();

            if ( string.Equals(name,      nameof(Pair.Key),   StringComparison.OrdinalIgnoreCase) ) { key   = reader.GetString() ?? EMPTY; }
            else if ( string.Equals(name, nameof(Pair.Value), StringComparison.OrdinalIgnoreCase) ) { value = reader.GetString(); }
            else { reader.Skip(); }
        }

        throw new JsonException($"Unexpected end of input when parsing {nameof(Pair)}.");
    }
    public override void Write( Utf8JsonWriter writer, Pair value, JsonSerializerOptions options )
    {
        writer.WriteStartObject();
        writer.WriteString(GetName(options, nameof(Pair.Key)), value.Key);

        if ( value.Value is null ) { writer.WriteNull(GetName(options, nameof(Pair.Value))); }
        else { writer.WriteString(GetName(options,                     nameof(Pair.Value)), value.Value); }

        writer.WriteEndObject();
    }
}



/// <inheritdoc cref="PairJsonConverter"/>
public sealed class StringTagsJsonConverter : JsonConverter<StringTags>
{
    public static readonly StringTagsJsonConverter Instance = new();


    public override StringTags Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType is JsonTokenType.Null ) { return StringTags.Empty; }

        if ( reader.TokenType is not JsonTokenType.StartObject ) { throw new JsonException($"Expected {nameof(JsonTokenType.StartObject)} when parsing {nameof(StringTags)}, found {reader.TokenType}."); }

        Pair[]?   tags    = null;
        string[]? entries = null;

        while ( reader.Read() )
        {
            if ( reader.TokenType is JsonTokenType.EndObject ) { return new StringTags(tags ?? [], entries ?? []); }

            if ( reader.TokenType is not JsonTokenType.PropertyName ) { throw new JsonException($"Expected a property name when parsing {nameof(StringTags)}, found {reader.TokenType}."); }

            string name = reader.GetString() ?? EMPTY;
            reader.Read();

            if ( string.Equals(name,      nameof(StringTags.Tags),    StringComparison.OrdinalIgnoreCase) ) { tags    = ReadTags(ref reader, options); }
            else if ( string.Equals(name, nameof(StringTags.Entries), StringComparison.OrdinalIgnoreCase) ) { entries = ReadEntries(ref reader); }
            else { reader.Skip(); }
        }

        throw new JsonException($"Unexpected end of input when parsing {nameof(StringTags)}.");
    }
    private static Pair[] ReadTags( ref Utf8JsonReader reader, JsonSerializerOptions options )
    {
        if ( reader.TokenType is JsonTokenType.Null ) { return []; }

        if ( reader.TokenType is not JsonTokenType.StartArray ) { throw new JsonException($"Expected {nameof(JsonTokenType.StartArray)} when parsing {nameof(StringTags)}.{nameof(StringTags.Tags)}, found {reader.TokenType}."); }

        List<Pair> tags = [];

        while ( reader.Read() )
        {
            if ( reader.TokenType is JsonTokenType.EndArray ) { return [.. tags]; }

            tags.Add(PairJsonConverter.Instance.Read(ref reader, typeof(Pair), options));
        }

        throw new JsonException($"Unexpected end of input when parsing {nameof(StringTags)}.{nameof(StringTags.Tags)}.");
    }
    private static string[] ReadEntries( ref Utf8JsonReader reader )
    {
        if ( reader.TokenType is JsonTokenType.Null ) { return []; }

        if ( reader.TokenType is not JsonTokenType.StartArray ) { throw new JsonException($"Expected {nameof(JsonTokenType.StartArray)} when parsing {nameof(StringTags)}.{nameof(StringTags.Entries)}, found {reader.TokenType}."); }

        List<string> entries = [];

        while ( reader.Read() )
        {
            if ( reader.TokenType is JsonTokenType.EndArray ) { return [.. entries]; }

            entries.Add(reader.GetString() ?? EMPTY);
        }

        throw new JsonException($"Unexpected end of input when parsing {nameof(StringTags)}.{nameof(StringTags.Entries)}.");
    }
    public override void Write( Utf8JsonWriter writer, StringTags value, JsonSerializerOptions options )
    {
        ReadOnlySpan<Pair>   tags    = value.Tags    ?? [];
        ReadOnlySpan<string> entries = value.Entries ?? [];

        writer.WriteStartObject();

        writer.WriteStartArray(PairJsonConverter.GetName(options, nameof(StringTags.Tags)));
        foreach ( ref readonly Pair pair in tags ) { PairJsonConverter.Instance.Write(writer, pair, options); }

        writer.WriteEndArray();

        writer.WriteStartArray(PairJsonConverter.GetName(options, nameof(StringTags.Entries)));
        foreach ( string entry in entries ) { writer.WriteStringValue(entry); }

        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}
