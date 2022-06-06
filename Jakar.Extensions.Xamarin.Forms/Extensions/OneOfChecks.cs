// ReSharper disable once CheckNamespace

namespace Jakar.Extensions.Collections;


public static partial class OneOfChecks
{
    public static bool IsOneOf( this PropertyChangedEventArgs e, params BindableProperty[] properties ) => e.IsOneOf(properties.Select(property => property.PropertyName));

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEqual( this PropertyChangedEventArgs e, BindableProperty property ) => e.IsEqual(property.PropertyName);
}
