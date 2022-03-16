


namespace Jakar.Extensions.Xamarin.Forms.Extensions;


public static class PropertyChangedEventArgsExtensions
{
    public static bool IsOneOf( this PropertyChangedEventArgs e, params BindableProperty[] properties ) => e.IsOneOf(properties.Select(property => property.PropertyName));
    public static bool IsOneOf( this PropertyChangedEventArgs e, IEnumerable<string>       properties ) => e.IsOneOf(properties.ToArray());
    public static bool IsOneOf( this PropertyChangedEventArgs e, params string[]           properties ) => e.PropertyName.IsOneOf(properties);


    public static bool IsEqual( this PropertyChangedEventArgs e, BindableProperty property ) => e.IsEqual(property.PropertyName);
    public static bool IsEqual( this PropertyChangedEventArgs e, string           property ) => e.PropertyName == property;
}
