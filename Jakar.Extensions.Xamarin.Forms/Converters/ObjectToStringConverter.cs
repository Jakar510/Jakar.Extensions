


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Converters;


[TypeConverter(typeof(string))]
public class ObjectToStringConverter : TypeConverter, IValueConverter, IExtendedTypeConverter
{
    public override bool CanConvertFrom( Type? sourceType ) => true;

    public virtual object? Convert( object?     value, Type targetType, object? parameter, CultureInfo culture ) => value?.ToString();
    public virtual object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => value;


    public virtual object? ConvertFrom( CultureInfo            culture, object?          value, IServiceProvider serviceProvider ) => value?.ToString();
    public         object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => value;

    public override object? ConvertFromInvariantString( string? value ) => value;
}
