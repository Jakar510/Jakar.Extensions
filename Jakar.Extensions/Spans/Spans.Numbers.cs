namespace Jakar.Extensions.Spans;


public static partial class Spans
{
    public static short As( this ReadOnlySpan<char> value, in short defaultValue ) => short.TryParse(value, out short result)
                                                                                          ? result
                                                                                          : defaultValue;

    public static short? As( this ReadOnlySpan<char> value, in short? defaultValue ) => short.TryParse(value, out short result)
                                                                                            ? result
                                                                                            : defaultValue;

    public static ushort As( this ReadOnlySpan<char> value, in ushort defaultValue ) => ushort.TryParse(value, out ushort result)
                                                                                            ? result
                                                                                            : defaultValue;

    public static ushort? As( this ReadOnlySpan<char> value, in ushort? defaultValue ) => ushort.TryParse(value, out ushort result)
                                                                                              ? result
                                                                                              : defaultValue;


    public static int As( this ReadOnlySpan<char> value, in int defaultValue ) => int.TryParse(value, out int result)
                                                                                      ? result
                                                                                      : defaultValue;

    public static int? As( this ReadOnlySpan<char> value, in int? defaultValue ) => int.TryParse(value, out int result)
                                                                                        ? result
                                                                                        : defaultValue;

    public static uint As( this ReadOnlySpan<char> value, in uint defaultValue ) => uint.TryParse(value, out uint result)
                                                                                        ? result
                                                                                        : defaultValue;

    public static uint? As( this ReadOnlySpan<char> value, in uint? defaultValue ) => uint.TryParse(value, out uint result)
                                                                                          ? result
                                                                                          : defaultValue;


    public static long? As( this ReadOnlySpan<char> value, in long? defaultValue ) => long.TryParse(value, out long result)
                                                                                          ? result
                                                                                          : defaultValue;

    public static long? As( this ReadOnlySpan<char> value, in long defaultValue ) => long.TryParse(value, out long result)
                                                                                         ? result
                                                                                         : defaultValue;

    public static ulong As( this ReadOnlySpan<char> value, in ulong defaultValue ) => ulong.TryParse(value, out ulong result)
                                                                                          ? result
                                                                                          : defaultValue;

    public static ulong? As( this ReadOnlySpan<char> value, in ulong? defaultValue ) => ulong.TryParse(value, out ulong result)
                                                                                            ? result
                                                                                            : defaultValue;


    public static float As( this ReadOnlySpan<char> value, in float defaultValue ) => float.TryParse(value, out float d)
                                                                                          ? d
                                                                                          : defaultValue;

    public static float? As( this ReadOnlySpan<char> value, in float? defaultValue ) => float.TryParse(value, out float d)
                                                                                            ? d
                                                                                            : defaultValue;


    public static double As( this ReadOnlySpan<char> value, in double defaultValue ) => double.TryParse(value, out double d)
                                                                                            ? d
                                                                                            : defaultValue;

    public static double? As( this ReadOnlySpan<char> value, in double? defaultValue ) => double.TryParse(value, out double d)
                                                                                              ? d
                                                                                              : defaultValue;


    public static decimal As( this ReadOnlySpan<char> value, in decimal defaultValue ) => decimal.TryParse(value, out decimal d)
                                                                                              ? d
                                                                                              : defaultValue;

    public static decimal? As( this ReadOnlySpan<char> value, in decimal? defaultValue ) => decimal.TryParse(value, out decimal d)
                                                                                                ? d
                                                                                                : defaultValue;
}
