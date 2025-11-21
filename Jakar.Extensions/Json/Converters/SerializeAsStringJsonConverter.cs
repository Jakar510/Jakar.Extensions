// Jakar.Extensions :: Jakar.Extensions
// 09/21/2025  16:31

namespace Jakar.Extensions;


public abstract class SerializeAsStringJsonConverter<TSelf, T> : JsonConverter<T>
    where T : IParsable<T>, IFormattable
    where TSelf : SerializeAsStringJsonConverter<TSelf, T>, new()
{
    public static readonly TSelf Instance = new();


    protected T? ParseValue( string? s, T? existingValue, bool useExistingValueOnError )
    {
        if ( string.IsNullOrWhiteSpace(s) ) { return default!; }

        try { return T.Parse(s, CultureInfo.InvariantCulture); }
        catch ( Exception ex )
        {
            if ( useExistingValueOnError ) { return existingValue; }

            throw new JsonSerializationException($"Cannot parse '{s}' as {typeof(T).Name}.", ex);
        }
    }


    public override T? ReadJson( JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        bool useExistingValueOnError = ( serializer.DefaultValueHandling & DefaultValueHandling.Populate ) != 0;

        if ( reader.TokenType is JsonToken.Null ) { return default; }

        if ( reader.Value is T t ) { return t; }

        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch ( reader.TokenType )
        {
            case JsonToken.String:
                return ParseValue((string?)reader.Value, existingValue, useExistingValueOnError);

            case JsonToken.Integer:
            case JsonToken.Float:
            case JsonToken.Date:
            case JsonToken.Boolean:
                // Some IParsable types can parse numbers (e.g., DateOnly, Guid cannot, but double/decimal can)
                return ParseValue(Convert.ToString(reader.Value, CultureInfo.InvariantCulture), existingValue, useExistingValueOnError);

            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing {typeof(T).Name}.");
        }
    }
    public override void WriteJson( JsonWriter writer, T? value, JsonSerializer serializer )
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(value.ToString(null, CultureInfo.InvariantCulture));
    }
}
