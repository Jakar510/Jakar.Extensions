#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


[global::Xamarin.Forms.Internals.Preserve(true, false)]
[TypeConversion(typeof(Color?))]
public class NullableColorTypeConverter : ColorTypeConverter, IValueConverter, IExtendedTypeConverter // IExtendedTypeConverter 
{
    public override bool CanConvertFrom( Type?                  sourceType ) => sourceType is null || sourceType == typeof(string);
    public override object? ConvertFromInvariantString( string? value ) => Convert(value);

    public Color? Convert( string? value ) => string.IsNullOrWhiteSpace(value)
                                                  ? null
                                                  : (Color)base.ConvertFromInvariantString(value);


    public object? Convert( object?     value, Type targetType, object parameter, CultureInfo culture ) => Convert(value?.ToString());
    public object? ConvertBack( object? value, Type targetType, object parameter, CultureInfo culture ) => value?.ToString();

    public object? ConvertFrom( CultureInfo            culture, object?          value, IServiceProvider serviceProvider ) => Convert(value?.ToString());
    public object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => Convert(value);
}
