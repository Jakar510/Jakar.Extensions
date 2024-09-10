// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure]
    public static T Min<T>( this ReadOnlySpan<T> value, T start )
        where T : INumber<T>
    {
        T result = start;
        foreach ( T x in value ) { result = T.Min( result, x ); }

        return result;
    }


    [Pure]
    public static TNumber Min<T, TNumber>( this ReadOnlySpan<T> value, Func<T, TNumber> selector, TNumber start )
        where TNumber : INumber<TNumber>
    {
        TNumber result = start;
        foreach ( T x in value ) { result = TNumber.Min( result, selector( x ) ); }

        return result;
    }
}
