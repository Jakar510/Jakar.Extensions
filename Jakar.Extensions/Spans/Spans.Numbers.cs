namespace Jakar.Extensions;


public static partial class Spans
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TValue As<TValue>( this scoped ref readonly ReadOnlySpan<byte> value, TValue defaultValue, IFormatProvider? provider = null )
        where TValue : INumber<TValue> => TValue.TryParse( value, provider, out TValue? d )
                                    ? d
                                    : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TValue As<TValue>( this scoped ref readonly ReadOnlySpan<byte> value, NumberStyles style, TValue defaultValue, IFormatProvider? provider = null )
        where TValue : INumber<TValue> => TValue.TryParse( value, style, provider, out TValue? d )
                                    ? d
                                    : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TValue As<TValue>( this scoped ref readonly ReadOnlySpan<char> value, TValue defaultValue, IFormatProvider? provider = null )
        where TValue : INumber<TValue> => TValue.TryParse( value, provider, out TValue? d )
                                    ? d
                                    : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TValue As<TValue>( this scoped ref readonly ReadOnlySpan<char> value, NumberStyles style, TValue defaultValue, IFormatProvider? provider = null )
        where TValue : INumber<TValue> => TValue.TryParse( value, style, provider, out TValue? d )
                                    ? d
                                    : defaultValue;
}
