namespace Jakar.Extensions;


public static partial class Spans
{
    extension( scoped ref readonly ReadOnlySpan<byte> value )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public TValue As<TValue>( TValue defaultValue, IFormatProvider? provider = null )
            where TValue : INumber<TValue> => TValue.TryParse(value, provider, out TValue? d)
                                                  ? d
                                                  : defaultValue;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public TValue As<TValue>( NumberStyles style, TValue defaultValue, IFormatProvider? provider = null )
            where TValue : INumber<TValue> => TValue.TryParse(value, style, provider, out TValue? d)
                                                  ? d
                                                  : defaultValue;
    }



    extension( scoped ref readonly ReadOnlySpan<char> value )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public TValue As<TValue>( TValue defaultValue, IFormatProvider? provider = null )
            where TValue : INumber<TValue> => TValue.TryParse(value, provider, out TValue? d)
                                                  ? d
                                                  : defaultValue;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public TValue As<TValue>( NumberStyles style, TValue defaultValue, IFormatProvider? provider = null )
            where TValue : INumber<TValue> => TValue.TryParse(value, style, provider, out TValue? d)
                                                  ? d
                                                  : defaultValue;
    }
}
