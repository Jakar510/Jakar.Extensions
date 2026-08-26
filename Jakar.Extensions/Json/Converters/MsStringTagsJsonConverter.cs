// Jakar.Extensions :: Jakar.Extensions
// System.Text.Json contracts for the RFC 7807 detail bag.

namespace Jakar.Extensions;


/// <summary>
///     <see cref="Pair"/> and <see cref="StringTags"/> carry their payload in public <c> readonly </c> fields, and System.Text.Json ignores fields unless <c> JsonSerializerOptions.IncludeFields </c> is set. Without these converters STJ writes <see cref="Error.Details"/> as <c> {"IsEmpty":true} </c> and reads it back empty, so every tag attached to an error is dropped on the way through Minimal API - Newtonsoft is unaffected because it serializes public fields by default.
///     <para> The wire shape is deliberately identical to Newtonsoft's ( <c> {"Tags":[{"Key":…,"Value":…}],"Entries":[…]} </c> ) so the same payload survives either serializer. Property names honour <c> JsonSerializerOptions.PropertyNamingPolicy </c> on write and are matched case-insensitively on read, which is what keeps output consistent under <c> JsonSerializerDefaults.Web </c> . </para>
///     <para> Everything here reads and writes the reader/writer directly rather than calling back into <c> JsonSerializer </c> , so no reflection is involved and the assembly stays trim- and AOT-clean. </para>
/// </summary>
public sealed class PairMsJsonConverter : System.Text.Json.Serialization.JsonConverter<Pair>
{
    public static readonly PairMsJsonConverter Instance = new();


    internal static string GetName( MsJsonSerializerOptions options, string name ) => options.PropertyNamingPolicy?.ConvertName(name) ?? name;


    public override Pair Read( ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, MsJsonSerializerOptions options )
    {
        if ( reader.TokenType is System.Text.Json.JsonTokenType.Null ) { return default; }

        if ( reader.TokenType is not System.Text.Json.JsonTokenType.StartObject ) { throw new System.Text.Json.JsonException($"Expected {nameof(System.Text.Json.JsonTokenType.StartObject)} when parsing {nameof(Pair)}, found {reader.TokenType}."); }

        string  key   = EMPTY;
        string? value = null;

        while ( reader.Read() )
        {
            if ( reader.TokenType is System.Text.Json.JsonTokenType.EndObject ) { return new Pair(key, value); }

            if ( reader.TokenType is not System.Text.Json.JsonTokenType.PropertyName ) { throw new System.Text.Json.JsonException($"Expected a property name when parsing {nameof(Pair)}, found {reader.TokenType}."); }

            string name = reader.GetString() ?? EMPTY;
            reader.Read();

            if ( string.Equals(name,      nameof(Pair.Key),   StringComparison.OrdinalIgnoreCase) ) { key   = reader.GetString() ?? EMPTY; }
            else if ( string.Equals(name, nameof(Pair.Value), StringComparison.OrdinalIgnoreCase) ) { value = reader.GetString(); }
            else { reader.Skip(); }
        }

        throw new System.Text.Json.JsonException($"Unexpected end of input when parsing {nameof(Pair)}.");
    }
    public override void Write( System.Text.Json.Utf8JsonWriter writer, Pair value, MsJsonSerializerOptions options )
    {
        writer.WriteStartObject();
        writer.WriteString(GetName(options, nameof(Pair.Key)), value.Key);

        if ( value.Value is null ) { writer.WriteNull(GetName(options, nameof(Pair.Value))); }
        else { writer.WriteString(GetName(options,                     nameof(Pair.Value)), value.Value); }

        writer.WriteEndObject();
    }
}



/// <inheritdoc cref="PairMsJsonConverter"/>
public sealed class StringTagsMsJsonConverter : System.Text.Json.Serialization.JsonConverter<StringTags>
{
    public static readonly StringTagsMsJsonConverter Instance = new();


    public override StringTags Read( ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, MsJsonSerializerOptions options )
    {
        if ( reader.TokenType is System.Text.Json.JsonTokenType.Null ) { return StringTags.Empty; }

        if ( reader.TokenType is not System.Text.Json.JsonTokenType.StartObject ) { throw new System.Text.Json.JsonException($"Expected {nameof(System.Text.Json.JsonTokenType.StartObject)} when parsing {nameof(StringTags)}, found {reader.TokenType}."); }

        Pair[]?   tags    = null;
        string[]? entries = null;

        while ( reader.Read() )
        {
            if ( reader.TokenType is System.Text.Json.JsonTokenType.EndObject ) { return new StringTags(tags ?? [], entries ?? []); }

            if ( reader.TokenType is not System.Text.Json.JsonTokenType.PropertyName ) { throw new System.Text.Json.JsonException($"Expected a property name when parsing {nameof(StringTags)}, found {reader.TokenType}."); }

            string name = reader.GetString() ?? EMPTY;
            reader.Read();

            if ( string.Equals(name,      nameof(StringTags.Tags),    StringComparison.OrdinalIgnoreCase) ) { tags    = ReadTags(ref reader, options); }
            else if ( string.Equals(name, nameof(StringTags.Entries), StringComparison.OrdinalIgnoreCase) ) { entries = ReadEntries(ref reader); }
            else { reader.Skip(); }
        }

        throw new System.Text.Json.JsonException($"Unexpected end of input when parsing {nameof(StringTags)}.");
    }
    private static Pair[] ReadTags( ref System.Text.Json.Utf8JsonReader reader, MsJsonSerializerOptions options )
    {
        if ( reader.TokenType is System.Text.Json.JsonTokenType.Null ) { return []; }

        if ( reader.TokenType is not System.Text.Json.JsonTokenType.StartArray ) { throw new System.Text.Json.JsonException($"Expected {nameof(System.Text.Json.JsonTokenType.StartArray)} when parsing {nameof(StringTags)}.{nameof(StringTags.Tags)}, found {reader.TokenType}."); }

        List<Pair> tags = [];

        while ( reader.Read() )
        {
            if ( reader.TokenType is System.Text.Json.JsonTokenType.EndArray ) { return [.. tags]; }

            tags.Add(PairMsJsonConverter.Instance.Read(ref reader, typeof(Pair), options));
        }

        throw new System.Text.Json.JsonException($"Unexpected end of input when parsing {nameof(StringTags)}.{nameof(StringTags.Tags)}.");
    }
    private static string[] ReadEntries( ref System.Text.Json.Utf8JsonReader reader )
    {
        if ( reader.TokenType is System.Text.Json.JsonTokenType.Null ) { return []; }

        if ( reader.TokenType is not System.Text.Json.JsonTokenType.StartArray ) { throw new System.Text.Json.JsonException($"Expected {nameof(System.Text.Json.JsonTokenType.StartArray)} when parsing {nameof(StringTags)}.{nameof(StringTags.Entries)}, found {reader.TokenType}."); }

        List<string> entries = [];

        while ( reader.Read() )
        {
            if ( reader.TokenType is System.Text.Json.JsonTokenType.EndArray ) { return [.. entries]; }

            entries.Add(reader.GetString() ?? EMPTY);
        }

        throw new System.Text.Json.JsonException($"Unexpected end of input when parsing {nameof(StringTags)}.{nameof(StringTags.Entries)}.");
    }
    public override void Write( System.Text.Json.Utf8JsonWriter writer, StringTags value, MsJsonSerializerOptions options )
    {
        ReadOnlySpan<Pair>   tags    = value.Tags    ?? [];
        ReadOnlySpan<string> entries = value.Entries ?? [];

        writer.WriteStartObject();

        writer.WriteStartArray(PairMsJsonConverter.GetName(options, nameof(StringTags.Tags)));
        foreach ( ref readonly Pair pair in tags ) { PairMsJsonConverter.Instance.Write(writer, pair, options); }

        writer.WriteEndArray();

        writer.WriteStartArray(PairMsJsonConverter.GetName(options, nameof(StringTags.Entries)));
        foreach ( string entry in entries ) { writer.WriteStringValue(entry); }

        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}
