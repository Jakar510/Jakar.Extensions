#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


[global::Xamarin.Forms.Internals.Preserve(true, false)]
[TypeConversion(typeof(Size))]
public class SizeConverter : TypeConverter, IValueConverter, IExtendedTypeConverter
{
    public static Size Convert( string? value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(Size).FullName}"); }

        string[] items = value.Split(',');

        switch ( items.Length )
        {
            case 1:
                double w = double.Parse(items[0]);
                return new Size(w, w);

            case 2:
                return new Size(double.Parse(items[0]), double.Parse(items[1]));
        }

        throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(Size).FullName}");
    }

    public override string ConvertToInvariantString( object value ) => value switch
                                                                       {
                                                                           Size size => $"{size.Width},{size.Height}",
                                                                           _         => value.ToString()
                                                                       };


    public override bool CanConvertFrom( Type? sourceType ) => sourceType is null || sourceType == typeof(string);

    public override object ConvertFromInvariantString( string value ) => Convert(value);

    public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) => Convert(value?.ToString());

    public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => value?.ToString();


    public object ConvertFrom( CultureInfo culture, object? value, IServiceProvider serviceProvider ) => Convert(value?.ToString());

    public object ConvertFromInvariantString( string? value, IServiceProvider serviceProvider ) => Convert(value);
}
