// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure]
    public static TValue Sum<TValue>( this scoped ref readonly ReadOnlySpan<TValue> value )
        where TValue : INumber<TValue>
    {
        TValue result = TValue.Zero;
        foreach ( TValue x in value ) { result += x; }

        return result;
    }


    [Pure]
    public static TNumber Sum<TValue, TNumber>( this scoped ref readonly ReadOnlySpan<TValue> value, Func<TValue, TNumber> selector )
        where TNumber : INumber<TNumber>
    {
        TNumber result = TNumber.Zero;
        foreach ( TValue x in value ) { result += selector( x ); }

        return result;
    }
}
