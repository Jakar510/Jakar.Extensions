// unset

#nullable enable
using Xamarin.Forms.Internals;



namespace Jakar.Extensions.Xamarin.Forms;


[Preserve( true, false )]
[TypeConversion( typeof(double?) )]
public class NullableDoubleTypeConverter : FontSizeConverter, IValueConverter, IExtendedTypeConverter // IExtendedTypeConverter 
{
    public static double? Convert( string? value ) => double.TryParse( value, out double d )
                                                          ? d
                                                          : null;
    public override bool CanConvertFrom( Type?                  sourceType ) => sourceType is null || sourceType == typeof(string);
    public override object? ConvertFromInvariantString( string? value ) => Convert( value );

    public override string? ConvertToInvariantString( object? value ) => value?.ToString();

    public object? ConvertFrom( CultureInfo            culture, object?          value, IServiceProvider serviceProvider ) => Convert( value?.ToString() );
    public object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => Convert( value );


    public object? Convert( object?     value, Type targetType, object parameter, CultureInfo culture ) => Convert( value?.ToString() );
    public object? ConvertBack( object? value, Type targetType, object parameter, CultureInfo culture ) => value?.ToString();
}
