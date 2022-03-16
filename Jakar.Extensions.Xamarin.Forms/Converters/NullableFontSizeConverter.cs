// unset

using Xamarin.Forms.Xaml;





namespace Jakar.Extensions.Xamarin.Forms.Converters;


[global::Xamarin.Forms.Internals.Preserve(true, false)]
[TypeConversion(typeof(double))]
public class NullableFontSizeConverter : FontSizeConverter, IValueConverter, IExtendedTypeConverter // IExtendedTypeConverter 
{
    public override bool    CanConvertFrom( Type?               sourceType ) => sourceType is null || sourceType == typeof(string);
    public override object? ConvertFromInvariantString( string? value )      => Convert(value);

    public double? Convert( string? value ) => value switch
                                               {
                                                   null => default,
                                                   _    => (double)base.ConvertFromInvariantString(value)
                                               };

    public override string? ConvertToInvariantString( object? value ) => value?.ToString();

    public object? Convert( object?     value, Type targetType, object parameter, CultureInfo culture ) => Convert(value?.ToString());
    public object? ConvertBack( object? value, Type targetType, object parameter, CultureInfo culture ) => value?.ToString();

    public object? ConvertFrom( CultureInfo            culture, object           value, IServiceProvider serviceProvider ) => Convert(value?.ToString());
    public object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => Convert(value);
}
