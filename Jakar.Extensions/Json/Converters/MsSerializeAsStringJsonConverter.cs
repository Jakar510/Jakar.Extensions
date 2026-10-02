// Jakar.Extensions :: Jakar.Extensions
// System.Text.Json counterpart of SerializeAsStringJsonConverter.

namespace Jakar.Extensions;


/// <summary>
///     <see cref="System.Text.Json"/> counterpart of <see cref="SerializeAsStringJsonConverter{TSelf, T}"/> .
///     <para> Newtonsoft honours <c> [JsonConverter] </c> from its own namespace only, so any type that round-trips as a string for Newtonsoft needs this one as well before it can cross the wire through Minimal API model binding. </para>
/// </summary>
public abstract class MsSerializeAsStringJsonConverter<TSelf, TValue> : System.Text.Json.Serialization.JsonConverter<TValue>
    where TValue : class, IParsable<TValue>, IFormattable
    where TSelf : MsSerializeAsStringJsonConverter<TSelf, TValue>, new()
{
    public static readonly TSelf Instance = new();


    public override TValue? Read( ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, MsJsonSerializerOptions options )
    {
        switch ( reader.TokenType )
        {
            case System.Text.Json.JsonTokenType.Null:
                return null;

            case System.Text.Json.JsonTokenType.String:
            {
                string? value = reader.GetString();

                return string.IsNullOrWhiteSpace(value)
                           ? null
                           : TValue.Parse(value, CultureInfo.InvariantCulture);
            }

            case System.Text.Json.JsonTokenType.Number:
            {
                string value = reader.GetDouble().ToString(CultureInfo.InvariantCulture);
                return TValue.Parse(value, CultureInfo.InvariantCulture);
            }

            default:
                throw new System.Text.Json.JsonException($"Unexpected token {reader.TokenType} when parsing {typeof(TValue).Name}.");
        }
    }
    public override void Write( System.Text.Json.Utf8JsonWriter writer, TValue? value, MsJsonSerializerOptions options )
    {
        if ( value is null )
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.ToString(null, CultureInfo.InvariantCulture));
    }
}



public sealed class AppVersionMsJsonConverter : MsSerializeAsStringJsonConverter<AppVersionMsJsonConverter, AppVersion>;
