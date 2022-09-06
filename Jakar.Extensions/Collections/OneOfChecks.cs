#nullable enable
using Newtonsoft.Json.Linq;
using System;



namespace Jakar.Extensions;


public static class OneOfChecks
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOneOf<TValue>( this TValue value, ReadOnlySpan<TValue> items ) where TValue : notnull
    {
        foreach ( TValue span in items )
        {
            if ( value.Equals(span) ) { return true; }
        }

        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOneOf( this PropertyChangedEventArgs e, IEnumerable<string> properties )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( string property in properties )
        {
            if ( e.IsEqual(property) ) { return true; }
        }

        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOneOf<TValue>( this TValue value, params TValue[] items ) => value is not null && items.Any(other => value.Equals(other));


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOneOf( this PropertyChangedEventArgs e, params string[] properties ) => e.PropertyName?.IsOneOf(properties) ?? false;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEqual( this PropertyChangedEventArgs e, string property ) => string.Equals(e.PropertyName, property, StringComparison.Ordinal);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEqual( this PropertyChangedEventArgs e, string property1, string property2 ) => e.IsEqual(property1) || e.IsEqual(property2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEqual( this PropertyChangedEventArgs e, string property1, string property2, string property3 ) => e.IsEqual(property1) || e.IsEqual(property2) || e.IsEqual(property3);
}
