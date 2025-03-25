// Jakar.Extensions :: Jakar.Extensions
// double.MinValue3/29/2double.MinValue23  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure]
    public static TValue Max<TValue>( this scoped ref readonly ReadOnlySpan<TValue> value, TValue start )
        where TValue : INumber<TValue>
    {
        TValue result = start;
        foreach ( TValue x in value ) { result = TValue.Max( result, x ); }

        return result;
    }


    [Pure]
    public static TNumber Max<TValue, TNumber>( this scoped ref readonly ReadOnlySpan<TValue> value, Func<TValue, TNumber> selector, TNumber start )
        where TNumber : INumber<TNumber>
    {
        TNumber result = start;
        foreach ( TValue x in value ) { result = TNumber.Max( result, selector( x ) ); }

        return result;
    }
}
