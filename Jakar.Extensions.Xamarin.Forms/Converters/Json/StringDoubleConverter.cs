


namespace Jakar.Extensions.Xamarin.Forms.Converters.Json;


[TypeConverter(typeof(double))]
public sealed class StringDoubleConverter : JsonConverter, IValueConverter, IExtendedTypeConverter
{
    public static string? ConvertBack( object? value )
    {
        switch ( value )
        {
            case null: return null;

            case double n:
                return double.IsNaN(n)
                           ? null
                           : n.ToString(CultureInfo.CurrentCulture);

            case string s:
            {
                double? t = ConvertTo(s);

                return t is null || double.IsNaN((double)t)
                           ? null
                           : ( (double)t ).ToString(CultureInfo.CurrentCulture);
            }

            default: throw new ExpectedValueTypeException(nameof(value), value, typeof(double), typeof(string));
        }
    }

    public static double? ConvertTo( string? value ) => string.IsNullOrWhiteSpace(value)
                                                            ? null
                                                            : double.TryParse(value, out double d)
                                                                ? d
                                                                : double.NaN;


#region json

    public override bool CanWrite => false;

    public override void WriteJson( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotSupportedException();

    public override bool CanRead => false;

    public override object? ReadJson( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer ) => ConvertTo(reader.Value?.ToString());

    public override bool CanConvert( Type objectType ) => objectType == typeof(string);

#endregion


    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) => ConvertTo(value?.ToString());

    public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => ConvertBack(value);


    public object? ConvertFrom( CultureInfo            culture, object?          value, IServiceProvider serviceProvider ) => ConvertTo(value?.ToString());
    public object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => ConvertTo(value);
}
