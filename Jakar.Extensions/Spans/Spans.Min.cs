// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure]
    public static TValue Min<TValue>( this scoped ref readonly ReadOnlySpan<TValue> value, TValue start )
        where TValue : INumber<TValue>
    {
        TValue result = start;
        foreach ( TValue x in value ) { result = TValue.Min( result, x ); }

        return result;
    }


    [Pure]
    public static TNumber Min<TValue, TNumber>( this scoped ref readonly ReadOnlySpan<TValue> value, Func<TValue, TNumber> selector, TNumber start )
        where TNumber : INumber<TNumber>
    {
        TNumber result = start;
        foreach ( TValue x in value ) { result = TNumber.Min( result, selector( x ) ); }

        return result;
    }
}
