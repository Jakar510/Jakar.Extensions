#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


[TypeConverter( typeof(bool) )]
public class ObjectToBoolConverter : TypeConverter, IValueConverter, IExtendedTypeConverter
{
    public static bool Convert( object? value ) => value switch
                                                   {
                                                       null     => false,
                                                       bool b   => b,
                                                       int n    => n != 0,
                                                       uint n   => n != 0,
                                                       long n   => n != 0,
                                                       ulong n  => n != 0,
                                                       float n  => n != 0,
                                                       double n => n != 0,
                                                       string s => bool.TryParse( s, out bool result )
                                                                       ? result
                                                                       : !string.IsNullOrWhiteSpace( s ),
                                                       _ => true,
                                                   };
    private static readonly Type[] _types =
    {
        typeof(bool),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(string),
    };


    public override bool CanConvertFrom( Type? sourceType ) => sourceType is null || sourceType.IsOneOf( _types );

    protected virtual bool InternalConvert( object? value ) => Convert( value );


    public override object ConvertFromInvariantString( string? value ) => InternalConvert( value );


    public object ConvertFrom( CultureInfo            culture, object?          value, IServiceProvider serviceProvider ) => InternalConvert( value );
    public object ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => InternalConvert( value );

    public virtual object Convert( object?      value, Type targetType, object? parameter, CultureInfo culture ) => InternalConvert( value );
    public virtual object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => value?.ToString();
}
