// Jakar.Extensions :: Jakar.Extensions
// 09/08/2022  10:36 AM

namespace Jakar.Extensions.Maui;


[TypeConverter(typeof(DateTimeOffset))]
public class DateTimeOffsetConverter : TypeConverter, IValueConverter, IExtendedTypeConverter
{
    public string?         Format   { get; set; }
    public IFormatProvider Provider { get; set; } = CultureInfo.InvariantCulture;
    public DateTimeStyles  Styles   { get; set; } = DateTimeStyles.None;


    public override bool CanConvertFrom( ITypeDescriptorContext? context, Type  sourceType ) => sourceType == typeof(DateTimeOffset) || sourceType == typeof(DateTime) || sourceType == typeof(string);
    public override bool CanConvertTo( ITypeDescriptorContext?   context, Type? destinationType ) => destinationType == typeof(DateTimeOffset);


    public object Convert( object                              value,   Type         targetType, object  parameter, CultureInfo culture ) => Convert(value, Format, Provider, Styles);
    public override object? ConvertTo( ITypeDescriptorContext? context, CultureInfo? culture,    object? value,     Type        destinationType ) => ConvertBack(value, culture ?? Provider, Styles);


    public object ConvertBack( object                            value,   Type         targetType, object parameter, CultureInfo culture ) => ConvertBack(value, Provider, Styles);
    public override object? ConvertFrom( ITypeDescriptorContext? context, CultureInfo? culture,    object value ) => Convert(value, Format, culture ?? Provider, Styles);


    public object ConvertFromInvariantString( string value, IServiceProvider serviceProvider ) => Convert(value, Format, Provider, Styles);


    public static string Convert( object? value, string? format, IFormatProvider provider, DateTimeStyles styles ) => value switch
                                                                                                                      {
                                                                                                                          DateTime dt => dt.ToString(format, provider),
                                                                                                                          DateTimeOffset dt => dt.LocalDateTime.ToString(format, provider),
                                                                                                                          string s when DateTimeOffset.TryParse(s, provider, styles, out DateTimeOffset dt) =>
                                                                                                                              dt.LocalDateTime.ToString(format, provider),
                                                                                                                          string s when DateTime.TryParse(s, provider, styles, out DateTime dt) => dt.ToString(format, provider),
                                                                                                                          _ => throw new ExpectedValueTypeException(nameof(value), value, typeof(DateTime), typeof(DateTimeOffset), typeof(string))
                                                                                                                      };
    public static DateTimeOffset ConvertBack( object? value, IFormatProvider provider, DateTimeStyles styles ) => value switch
                                                                                                                  {
                                                                                                                      DateTime dt => dt,
                                                                                                                      DateTimeOffset dt => dt,
                                                                                                                      string s when DateTimeOffset.TryParse(s, provider, styles, out DateTimeOffset dt) => dt,
                                                                                                                      string s when DateTime.TryParse(s, provider, styles, out DateTime dt) => dt,
                                                                                                                      _ => throw new ExpectedValueTypeException(nameof(value), value, typeof(DateTime), typeof(DateTimeOffset), typeof(string))
                                                                                                                  };
}
