#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public static class OneOfChecksXF
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, BindableProperty          property ) => e.IsEqual( property.PropertyName );
    public static bool IsOneOf( this                                                      PropertyChangedEventArgs e, params BindableProperty[] properties ) => e.IsOneOf( properties.Select( property => property.PropertyName ) );
}
