namespace Jakar.Extensions;


public static class OneOfChecks
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, string property ) => string.Equals( e.PropertyName, property, StringComparison.Ordinal );

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, string property1, string property2 ) => e.IsEqual( property1 ) || e.IsEqual( property2 );

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, string property1, string property2, string property3 ) => e.IsEqual( property1 ) || e.IsEqual( property2 ) || e.IsEqual( property3 );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, ReadOnlySpan<char> property ) => property.SequenceEqual( e.PropertyName );

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, ReadOnlySpan<char> property1, ReadOnlySpan<char> property2 ) => e.IsEqual( property1 ) || e.IsEqual( property2 );

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqual( this PropertyChangedEventArgs e, ReadOnlySpan<char> property1, ReadOnlySpan<char> property2, ReadOnlySpan<char> property3 ) => e.IsEqual( property1 ) || e.IsEqual( property2 ) || e.IsEqual( property3 );


#if NETSTANDARD2_1
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static bool IsOneOf<TValue>( this TValue value, params TValue[] items )
    {
        ReadOnlySpan<TValue> span = items;
        return value.IsOneOf( span );
    }
#endif

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf<TValue>( this TValue value, in ReadOnlySpan<TValue> items )
    {
        if ( value is null ) { return false; }

        if ( value is IEquatable<TValue> equatable )
        {
            foreach ( TValue item in items )
            {
                if ( equatable.Equals( item ) ) { return true; }
            }
        }
        else
        {
            foreach ( TValue item in items )
            {
                if ( value.Equals( item ) ) { return true; }
            }
        }


        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsOneOf( this string value, in ReadOnlySpan<string> items ) => value.AsSpan().IsOneOf( items );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf( this ReadOnlySpan<char> value, in ReadOnlySpan<string> items )
    {
        foreach ( string item in items )
        {
            if ( value.SequenceEqual( item ) ) { return true; }
        }

        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf<T>( this PropertyChangedEventArgs e, T properties )
        where T : IEnumerable<string>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( string property in properties )
        {
            if ( e.IsEqual( property ) ) { return true; }
        }

        return false;
    }


#if NETSTANDARD2_1
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static bool IsOneOf( this PropertyChangedEventArgs e, params string[] properties ) => e.PropertyName?.IsOneOf( properties ) ?? false;
#endif

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsOneOf( this PropertyChangedEventArgs e, in ReadOnlySpan<string> properties )
    {
        if ( string.IsNullOrEmpty( e.PropertyName ) ) { return false; }

        return e.PropertyName.IsOneOf( properties );
    }
}
