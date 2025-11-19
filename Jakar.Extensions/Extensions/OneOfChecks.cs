namespace Jakar.Extensions;


public static class OneOfChecks
{
    extension( PropertyChangedEventArgs self )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsEqual( string             property )                                      => string.Equals(self.PropertyName, property, StringComparison.Ordinal);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsEqual( string             property1, string property2 )                   => self.IsEqual(property1) || self.IsEqual(property2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsEqual( string             property1, string property2, string property3 ) => self.IsEqual(property1) || self.IsEqual(property2) || self.IsEqual(property3);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsEqual( ReadOnlySpan<char> property )                                                              => property.SequenceEqual(self.PropertyName);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsEqual( ReadOnlySpan<char> property1, ReadOnlySpan<char> property2 )                               => self.IsEqual(property1) || self.IsEqual(property2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsEqual( ReadOnlySpan<char> property1, ReadOnlySpan<char> property2, ReadOnlySpan<char> property3 ) => self.IsEqual(property1) || self.IsEqual(property2) || self.IsEqual(property3);
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOneOf<TValue>( this TValue value, params ReadOnlySpan<TValue> items )
        where TValue : IEquatable<TValue>
    {
        foreach ( TValue item in items )
        {
            if ( value.Equals(item) ) { return true; }
        }

        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOneOf( this string value, params ReadOnlySpan<string> items ) => value.AsSpan()
                                                                                                                                                  .IsOneOf(items);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOneOf( this ReadOnlySpan<char> value, params ReadOnlySpan<string> items )
    {
        foreach ( string item in items )
        {
            if ( value.SequenceEqual(item) ) { return true; }
        }

        return false;
    }



    extension( PropertyChangedEventArgs self )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsOneOf<TValue>( TValue properties )
            where TValue : IEnumerable<string>
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( string property in properties )
            {
                if ( self.IsEqual(property) ) { return true; }
            }

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsOneOf( params ReadOnlySpan<string> properties )
        {
            if ( string.IsNullOrEmpty(self.PropertyName) ) { return false; }

            return self.PropertyName.IsOneOf(properties);
        }
    }
}
