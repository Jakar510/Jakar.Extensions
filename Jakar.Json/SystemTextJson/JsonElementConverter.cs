// Jakar.Json
// 10/07/2026

using System.Text.Json;



namespace Jakar.Json;


/// <summary>
///     A System.Text.Json <see cref="JsonElement"/> as Jakar.Json reads and writes it: the bridge for types shared with System.Text.Json, such as the
///     <c>Dictionary&lt;string, JsonElement&gt;</c> extension-data bag of <c>IJsonModel</c>.
/// </summary>
/// <remarks>
///     Writing walks the element (numbers verbatim). Reading goes through the <see cref="JNode"/> DOM, which keeps number text verbatim, so values round-trip exactly;
///     it allocates the element's document, like any <see cref="JsonElement"/>.
/// </remarks>
public readonly struct JsonElementConverter : IJsonConverter<JsonElement>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in JsonElement value )
        where TWriter : IJsonWriter, allows ref struct
    {
        switch ( value.ValueKind )
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();

                foreach ( JsonProperty property in value.EnumerateObject() )
                {
                    writer.WritePropertyName(property.Name);
                    JsonElement member = property.Value;
                    Write(ref writer, in member);
                }

                writer.WriteEndObject();
                return;

            case JsonValueKind.Array:
                writer.WriteStartArray();

                foreach ( JsonElement element in value.EnumerateArray() )
                {
                    JsonElement item = element;
                    Write(ref writer, in item);
                }

                writer.WriteEndArray();
                return;

            case JsonValueKind.String:
                writer.WriteString(value.GetString());
                return;

            case JsonValueKind.Number:
                writer.WriteRawNumber(value.GetRawText());
                return;

            case JsonValueKind.True:
                writer.WriteBoolean(true);
                return;

            case JsonValueKind.False:
                writer.WriteBoolean(false);
                return;

            default: // Null, and Undefined (a default(JsonElement)): there's no value, so null.
                writer.WriteNull();
                return;
        }
    }

    public static bool TryRead<TReader>( ref TReader reader, out JsonElement value )
        where TReader : IJsonReader, allows ref struct
    {
        value = default;
        if ( !JNode.TryRead(ref reader, out JNode? node) ) { return false; }

        value = node is null
                    ? JsonElement.Parse("null"u8)
                    : JsonElement.Parse(node.ToJsonUtf8());

        return true;
    }
}
