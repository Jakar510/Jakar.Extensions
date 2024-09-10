namespace Jakar.Extensions;


public static partial class Spans
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T As<T>( this ReadOnlySpan<byte> value, T defaultValue, IFormatProvider? provider = null )
        where T : INumber<T> => T.TryParse( value, provider, out T? d )
                                    ? d
                                    : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T As<T>( this ReadOnlySpan<byte> value, NumberStyles style, T defaultValue, IFormatProvider? provider = null )
        where T : INumber<T> => T.TryParse( value, style, provider, out T? d )
                                    ? d
                                    : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T As<T>( this ReadOnlySpan<char> value, T defaultValue, IFormatProvider? provider = null )
        where T : INumber<T> => T.TryParse( value, provider, out T? d )
                                    ? d
                                    : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T As<T>( this ReadOnlySpan<char> value, NumberStyles style, T defaultValue, IFormatProvider? provider = null )
        where T : INumber<T> => T.TryParse( value, style, provider, out T? d )
                                    ? d
                                    : defaultValue;
}
