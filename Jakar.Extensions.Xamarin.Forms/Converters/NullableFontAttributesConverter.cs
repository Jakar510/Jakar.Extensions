#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Converters;


[global::Xamarin.Forms.Internals.Preserve(true, false)]
[TypeConversion(typeof(FontAttributes?))]
public class NullableFontAttributesConverter : TypeConverter, IValueConverter, IExtendedTypeConverter // IExtendedTypeConverter 
{
    protected static readonly FontAttributesConverter _converter = new();

    public override bool CanConvertFrom( Type?                  sourceType ) => sourceType is null || sourceType == typeof(string);
    public override object? ConvertFromInvariantString( string? value ) => Convert(value);

    public static FontAttributes? Convert( string? value ) => value switch
                                                              {
                                                                  null => null,
                                                                  _    => (FontAttributes)_converter.ConvertFromInvariantString(value)
                                                              };

    public object? Convert( object?     value, Type targetType, object parameter, CultureInfo culture ) => Convert(value?.ToString());
    public object? ConvertBack( object? value, Type targetType, object parameter, CultureInfo culture ) => value?.ToString();

    public object? ConvertFrom( CultureInfo            culture, object?          value, IServiceProvider serviceProvider ) => Convert(value?.ToString());
    public object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => Convert(value);
}
