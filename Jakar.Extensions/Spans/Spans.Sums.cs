// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure]
    public static T Sum<T>( this ReadOnlySpan<T> value )
        where T : INumber<T>
    {
        T result = T.Zero;
        foreach ( T x in value ) { result += x; }

        return result;
    }


    [Pure]
    public static TNumber Sum<T, TNumber>( this ReadOnlySpan<T> value, Func<T, TNumber> selector )
        where TNumber : INumber<TNumber>
    {
        TNumber result = TNumber.Zero;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
}
