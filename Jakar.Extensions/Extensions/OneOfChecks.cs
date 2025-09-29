namespace Jakar.Extensions;


public static class OneOfChecks
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, string property )                                      => string.Equals( e.PropertyName, property, StringComparison.Ordinal );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, string property1, string property2 )                   => e.IsEqual( property1 ) || e.IsEqual( property2 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, string property1, string property2, string property3 ) => e.IsEqual( property1 ) || e.IsEqual( property2 ) || e.IsEqual( property3 );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, ReadOnlySpan<char> property )                                                              => property.SequenceEqual( e.PropertyName );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, ReadOnlySpan<char> property1, ReadOnlySpan<char> property2 )                               => e.IsEqual( property1 ) || e.IsEqual( property2 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, ReadOnlySpan<char> property1, ReadOnlySpan<char> property2, ReadOnlySpan<char> property3 ) => e.IsEqual( property1 ) || e.IsEqual( property2 ) || e.IsEqual( property3 );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf<TValue>( this TValue value, params ReadOnlySpan<TValue> items )
        where TValue : IEquatable<TValue>
    {
        foreach ( TValue item in items )
        {
            if ( value.Equals( item ) ) { return true; }
        }

        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsOneOf( this string value, params ReadOnlySpan<string> items ) => value.AsSpan().IsOneOf( items );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf( this ReadOnlySpan<char> value, params ReadOnlySpan<string> items )
    {
        foreach ( string item in items )
        {
            if ( value.SequenceEqual( item ) ) { return true; }
        }

        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf<TValue>( this PropertyChangedEventArgs e, TValue properties )
        where TValue : IEnumerable<string>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( string property in properties )
        {
            if ( e.IsEqual( property ) ) { return true; }
        }

        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf( this PropertyChangedEventArgs e, params ReadOnlySpan<string> properties )
    {
        if ( string.IsNullOrEmpty( e.PropertyName ) ) { return false; }

        return e.PropertyName.IsOneOf( properties );
    }
}
