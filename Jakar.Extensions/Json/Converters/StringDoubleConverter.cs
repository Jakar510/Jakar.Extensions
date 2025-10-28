namespace Jakar.Extensions;


[TypeConverter(typeof(double))]
public sealed class StringDoubleConverter : JsonConverter<double?>
{
    public static double? ConvertTo( string? value ) => string.IsNullOrWhiteSpace(value)
                                                            ? null
                                                            : double.TryParse(value, out double d)
                                                                ? d
                                                                : double.NaN;
    public static string? ConvertBack( object? value )
    {
        switch ( value )
        {
            case null:
                return null;

            case double n:
                return double.IsNaN(n)
                           ? null
                           : n.ToString(CultureInfo.CurrentUICulture);

            case string s:
            {
                double? t = ConvertTo(s);

                return t is null || double.IsNaN((double)t)
                           ? null
                           : ( (double)t ).ToString(CultureInfo.CurrentUICulture);
            }

            default:
                throw new ExpectedValueTypeException(nameof(value), value, typeof(double), typeof(string));
        }
    }
    public override double? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        return reader.TokenType switch
               {
                   JsonTokenType.Null                                          => null,
                   JsonTokenType.Number when reader.TryGetDouble(out double n) => n,
                   JsonTokenType.String                                        => ConvertTo(reader.GetString()),
                   _                                                           => throw new JsonException($"Unexpected token parsing double: {reader.TokenType}")
               };
    }

    public override void Write( Utf8JsonWriter writer, double? value, JsonSerializerOptions options )
    {
        if ( value is null )
        {
            writer.WriteNullValue();
            return;
        }

        if ( double.IsNaN(value.Value) )
        {
            // Could choose to emit `null` or `"NaN"`; here we emit null
            writer.WriteNullValue();
            return;
        }

        // Always write as string
        writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
    }
}
