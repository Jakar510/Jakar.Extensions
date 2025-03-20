// Jakar.Extensions :: Jakar.Extensions
// double.MinValue3/29/2double.MinValue23  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure]
    public static T Max<T>( this scoped ref readonly ReadOnlySpan<T> value, T start )
        where T : INumber<T>
    {
        T result = start;
        foreach ( T x in value ) { result = T.Max( result, x ); }

        return result;
    }


    [Pure]
    public static TNumber Max<T, TNumber>( this scoped ref readonly ReadOnlySpan<T> value, Func<T, TNumber> selector, TNumber start )
        where TNumber : INumber<TNumber>
    {
        TNumber result = start;
        foreach ( T x in value ) { result = TNumber.Max( result, selector( x ) ); }

        return result;
    }
}
