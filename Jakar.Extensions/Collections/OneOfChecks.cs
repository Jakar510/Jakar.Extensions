#nullable enable
namespace Jakar.Extensions;


public static class OneOfChecks
{
    public static bool IsOneOf<TValue>( this TValue value, params TValue[] items )
    {
        if ( value is null ) { return false; }

        return items.Any(other => value.Equals(other));
    }


    public static bool IsOneOf( this PropertyChangedEventArgs e, IEnumerable<string> properties ) => e.IsOneOf(properties.ToArray());
    public static bool IsOneOf( this PropertyChangedEventArgs e, params string[]     properties ) => IsOneOf(e.PropertyName, properties);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEqual( this PropertyChangedEventArgs e, string property ) => string.Equals(e.PropertyName, property, StringComparison.Ordinal);
}
