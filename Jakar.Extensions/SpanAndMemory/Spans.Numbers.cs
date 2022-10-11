#nullable enable
namespace Jakar.Extensions;


public static partial class Spans
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static short As( this ReadOnlySpan<char> value, in short defaultValue ) => short.TryParse( value, out short result )
                                                                                          ? result
                                                                                          : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static short? As( this ReadOnlySpan<char> value, in short? defaultValue ) => short.TryParse( value, out short result )
                                                                                            ? result
                                                                                            : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ushort As( this ReadOnlySpan<char> value, in ushort defaultValue ) => ushort.TryParse( value, out ushort result )
                                                                                            ? result
                                                                                            : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ushort? As( this ReadOnlySpan<char> value, in ushort? defaultValue ) => ushort.TryParse( value, out ushort result )
                                                                                              ? result
                                                                                              : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int As( this ReadOnlySpan<char> value, in int defaultValue ) => int.TryParse( value, out int result )
                                                                                      ? result
                                                                                      : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int? As( this ReadOnlySpan<char> value, in int? defaultValue ) => int.TryParse( value, out int result )
                                                                                        ? result
                                                                                        : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint As( this ReadOnlySpan<char> value, in uint defaultValue ) => uint.TryParse( value, out uint result )
                                                                                        ? result
                                                                                        : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint? As( this ReadOnlySpan<char> value, in uint? defaultValue ) => uint.TryParse( value, out uint result )
                                                                                          ? result
                                                                                          : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long? As( this ReadOnlySpan<char> value, in long? defaultValue ) => long.TryParse( value, out long result )
                                                                                          ? result
                                                                                          : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long? As( this ReadOnlySpan<char> value, in long defaultValue ) => long.TryParse( value, out long result )
                                                                                         ? result
                                                                                         : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong As( this ReadOnlySpan<char> value, in ulong defaultValue ) => ulong.TryParse( value, out ulong result )
                                                                                          ? result
                                                                                          : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong? As( this ReadOnlySpan<char> value, in ulong? defaultValue ) => ulong.TryParse( value, out ulong result )
                                                                                            ? result
                                                                                            : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static float As( this ReadOnlySpan<char> value, in float defaultValue ) => float.TryParse( value, out float d )
                                                                                          ? d
                                                                                          : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static float? As( this ReadOnlySpan<char> value, in float? defaultValue ) => float.TryParse( value, out float d )
                                                                                            ? d
                                                                                            : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static double As( this ReadOnlySpan<char> value, in double defaultValue ) => double.TryParse( value, out double d )
                                                                                            ? d
                                                                                            : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static double? As( this ReadOnlySpan<char> value, in double? defaultValue ) => double.TryParse( value, out double d )
                                                                                              ? d
                                                                                              : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static decimal As( this ReadOnlySpan<char> value, in decimal defaultValue ) => decimal.TryParse( value, out decimal d )
                                                                                              ? d
                                                                                              : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static decimal? As( this ReadOnlySpan<char> value, in decimal? defaultValue ) => decimal.TryParse( value, out decimal d )
                                                                                                ? d
                                                                                                : defaultValue;
}
