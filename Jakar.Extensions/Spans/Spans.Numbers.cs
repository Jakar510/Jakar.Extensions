namespace Jakar.Extensions;


public static partial class Spans
{
#if NET8_0_OR_GREATER
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static T As<T>( this ReadOnlySpan<byte> value, T defaultValue, IFormatProvider? provider = default )
        where T : INumber<T> => T.TryParse( value, provider, out T? d )
                                    ? d
                                    : defaultValue;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static T As<T>( this ReadOnlySpan<byte> value, NumberStyles style, T defaultValue, IFormatProvider? provider = default )
        where T : INumber<T> => T.TryParse( value, style, provider, out T? d )
                                    ? d
                                    : defaultValue;
#endif


#if NET7_0_OR_GREATER
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static T As<T>( this ReadOnlySpan<char> value, T defaultValue, IFormatProvider? provider = default )
        where T : INumber<T> => T.TryParse( value, provider, out T? d )
                                    ? d
                                    : defaultValue;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static T As<T>( this ReadOnlySpan<char> value, NumberStyles style, T defaultValue, IFormatProvider? provider = default )
        where T : INumber<T> => T.TryParse( value, style, provider, out T? d )
                                    ? d
                                    : defaultValue;

#else
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static decimal As( this ReadOnlySpan<char> value, decimal defaultValue ) => decimal.TryParse( value, out decimal d )
                                                                                           ? d
                                                                                           : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static decimal? As( this ReadOnlySpan<char> value, decimal? defaultValue ) => decimal.TryParse( value, out decimal d )
                                                                                             ? d
                                                                                             : defaultValue;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static double As( this ReadOnlySpan<char> value, double defaultValue ) => double.TryParse( value, out double d )
                                                                                         ? d
                                                                                         : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static double? As( this ReadOnlySpan<char> value, double? defaultValue ) => double.TryParse( value, out double d )
                                                                                           ? d
                                                                                           : defaultValue;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static float As( this ReadOnlySpan<char> value, float defaultValue ) => float.TryParse( value, out float d )
                                                                                       ? d
                                                                                       : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static float? As( this ReadOnlySpan<char> value, float? defaultValue ) => float.TryParse( value, out float d )
                                                                                         ? d
                                                                                         : defaultValue;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static int As( this ReadOnlySpan<char> value, int defaultValue ) => int.TryParse( value, out int result )
                                                                                   ? result
                                                                                   : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static int? As( this ReadOnlySpan<char> value, int? defaultValue ) => int.TryParse( value, out int result )
                                                                                     ? result
                                                                                     : defaultValue;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static long? As( this ReadOnlySpan<char> value, long? defaultValue ) => long.TryParse( value, out long result )
                                                                                       ? result
                                                                                       : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static long? As( this ReadOnlySpan<char> value, long defaultValue ) => long.TryParse( value, out long result )
                                                                                      ? result
                                                                                      : defaultValue;
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static short As( this ReadOnlySpan<char> value, short defaultValue ) => short.TryParse( value, out short result )
                                                                                       ? result
                                                                                       : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static short? As( this ReadOnlySpan<char> value, short? defaultValue ) => short.TryParse( value, out short result )
                                                                                         ? result
                                                                                         : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static uint As( this ReadOnlySpan<char> value, uint defaultValue ) => uint.TryParse( value, out uint result )
                                                                                     ? result
                                                                                     : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static uint? As( this ReadOnlySpan<char> value, uint? defaultValue ) => uint.TryParse( value, out uint result )
                                                                                       ? result
                                                                                       : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static ulong As( this ReadOnlySpan<char> value, ulong defaultValue ) => ulong.TryParse( value, out ulong result )
                                                                                       ? result
                                                                                       : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static ulong? As( this ReadOnlySpan<char> value, ulong? defaultValue ) => ulong.TryParse( value, out ulong result )
                                                                                         ? result
                                                                                         : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static ushort As( this ReadOnlySpan<char> value, ushort defaultValue ) => ushort.TryParse( value, out ushort result )
                                                                                         ? result
                                                                                         : defaultValue;

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static ushort? As( this ReadOnlySpan<char> value, ushort? defaultValue ) => ushort.TryParse( value, out ushort result )
                                                                                           ? result
                                                                                           : defaultValue;
#endif
}
