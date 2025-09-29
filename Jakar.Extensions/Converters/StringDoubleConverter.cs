namespace Jakar.Extensions;


[TypeConverter( typeof(double) )]
public sealed class StringDoubleConverter : JsonConverter
{
    public static double? ConvertTo( string? value ) => string.IsNullOrWhiteSpace( value )
                                                            ? null
                                                            : double.TryParse( value, out double d )
                                                                ? d
                                                                : double.NaN;
    public static string? ConvertBack( object? value )
    {
        switch ( value )
        {
            case null: return null;

            case double n:
                return double.IsNaN( n )
                           ? null
                           : n.ToString( CultureInfo.CurrentUICulture );

            case string s:
            {
                double? t = ConvertTo( s );

                return t is null || double.IsNaN( (double)t )
                           ? null
                           : ((double)t).ToString( CultureInfo.CurrentUICulture );
            }

            default: throw new ExpectedValueTypeException( nameof(value), value, typeof(double), typeof(string) );
        }
    }



    #region json

    public override bool CanWrite => false;
    public override bool CanRead  => false;

    public override void WriteJson( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotSupportedException();

    public override object? ReadJson( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer ) => ConvertTo( reader.Value?.ToString() );

    public override bool CanConvert( Type objectType ) => objectType == typeof(string);

    #endregion
}
