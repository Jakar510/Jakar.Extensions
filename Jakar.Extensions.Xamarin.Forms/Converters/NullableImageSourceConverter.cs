// unset

using Xamarin.Forms.Xaml;





namespace Jakar.Extensions.Xamarin.Forms.Converters;


[global::Xamarin.Forms.Internals.Preserve(true, false)]
[TypeConversion(typeof(ImageSource))]
public class NullableImageSourceConverter : TypeConverter, IValueConverter, IExtendedTypeConverter // IExtendedTypeConverter 
{
    private readonly ImageSourceConverter _converter = new();
    public override  bool                 CanConvertFrom( Type?               sourceType ) => sourceType is null || sourceType == typeof(string);
    public override  object?              ConvertFromInvariantString( string? value )      => Convert(value);

    public ImageSource? Convert( string? value ) => string.IsNullOrWhiteSpace(value)
                                                        ? null
                                                        : (ImageSource)_converter.ConvertFromInvariantString(value);

    public override string? ConvertToInvariantString( object? _ ) => throw new NotImplementedException();


    public object? Convert( object?     value, Type targetType, object parameter, CultureInfo culture ) => Convert(value?.ToString());
    public object? ConvertBack( object? value, Type targetType, object parameter, CultureInfo culture ) => value?.ToString();

    public object? ConvertFrom( CultureInfo            culture, object           value, IServiceProvider serviceProvider ) => Convert(value?.ToString());
    public object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => Convert(value);
}
