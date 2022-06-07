// unset


#nullable enable
namespace Jakar.Extensions.Strings;


public static class Numbers
{
    public static float AsFloat( this   decimal value ) => (float)value;
    public static int AsInt( this       decimal value ) => (int)value;
    public static uint AsUInt( this     decimal value ) => (uint)value;
    public static ushort AsUShort( this decimal value ) => (ushort)value;
    public static short AsShort( this   decimal value ) => (short)value;
    public static sbyte AsSByte( this   decimal value ) => (sbyte)value;
    public static byte AsByte( this     decimal value ) => (byte)value;


    public static float AsFloat( this   double value ) => (float)value;
    public static int AsInt( this       double value ) => (int)value;
    public static uint AsUInt( this     double value ) => (uint)value;
    public static ushort AsUShort( this double value ) => (ushort)value;
    public static short AsShort( this   double value ) => (short)value;
    public static sbyte AsSByte( this   double value ) => (sbyte)value;
    public static byte AsByte( this     double value ) => (byte)value;


    public static float AsFloat( this   float value ) => value;
    public static int AsInt( this       float value ) => (int)value;
    public static uint AsUInt( this     float value ) => (uint)value;
    public static ushort AsUShort( this float value ) => (ushort)value;
    public static short AsShort( this   float value ) => (short)value;
    public static sbyte AsSByte( this   float value ) => (sbyte)value;
    public static byte AsByte( this     float value ) => (byte)value;


    public static byte As( this string? value, in byte defaultValue ) => byte.TryParse(value, out byte result)
                                                                             ? result
                                                                             : defaultValue;

    public static byte? As( this string? value, in byte? defaultValue ) => byte.TryParse(value, out byte result)
                                                                               ? result
                                                                               : defaultValue;

    public static sbyte As( this string? value, in sbyte defaultValue ) => sbyte.TryParse(value, out sbyte result)
                                                                               ? result
                                                                               : defaultValue;

    public static sbyte? As( this string? value, in sbyte? defaultValue ) => sbyte.TryParse(value, out sbyte result)
                                                                                 ? result
                                                                                 : defaultValue;


    public static short As( this string? value, in short defaultValue ) => short.TryParse(value, out short result)
                                                                               ? result
                                                                               : defaultValue;

    public static short? As( this string? value, in short? defaultValue ) => short.TryParse(value, out short result)
                                                                                 ? result
                                                                                 : defaultValue;

    public static ushort As( this string? value, in ushort defaultValue ) => ushort.TryParse(value, out ushort result)
                                                                                 ? result
                                                                                 : defaultValue;

    public static ushort? As( this string? value, in ushort? defaultValue ) => ushort.TryParse(value, out ushort result)
                                                                                   ? result
                                                                                   : defaultValue;


    public static int As( this string? value, in int defaultValue ) => int.TryParse(value, out int result)
                                                                           ? result
                                                                           : defaultValue;

    public static int? As( this string? value, in int? defaultValue ) => int.TryParse(value, out int result)
                                                                             ? result
                                                                             : defaultValue;

    public static uint As( this string? value, in uint defaultValue ) => uint.TryParse(value, out uint result)
                                                                             ? result
                                                                             : defaultValue;

    public static uint? As( this string? value, in uint? defaultValue ) => uint.TryParse(value, out uint result)
                                                                               ? result
                                                                               : defaultValue;


    public static long? As( this string? value, in long? defaultValue ) => string.IsNullOrWhiteSpace(value)
                                                                               ? default
                                                                               : long.TryParse(value, out long result)
                                                                                   ? result
                                                                                   : defaultValue;

    public static long? As( this string? value, in long defaultValue ) => long.TryParse(value, out long result)
                                                                              ? result
                                                                              : defaultValue;

    public static ulong As( this string? value, in ulong defaultValue ) => ulong.TryParse(value, out ulong result)
                                                                               ? result
                                                                               : defaultValue;

    public static ulong? As( this string? value, in ulong? defaultValue ) => ulong.TryParse(value, out ulong result)
                                                                                 ? result
                                                                                 : defaultValue;


    public static float As( this string? value, in float defaultValue ) => float.TryParse(value, out float d)
                                                                               ? d
                                                                               : defaultValue;

    public static float? As( this string? value, in float? defaultValue ) => float.TryParse(value, out float d)
                                                                                 ? d
                                                                                 : defaultValue;


    public static double As( this string? value, in double defaultValue ) => double.TryParse(value, out double d)
                                                                                 ? d
                                                                                 : defaultValue;

    public static double? As( this string? value, in double? defaultValue ) => double.TryParse(value, out double d)
                                                                                   ? d
                                                                                   : defaultValue;


    public static decimal As( this string? value, in decimal defaultValue ) => decimal.TryParse(value, out decimal d)
                                                                                   ? d
                                                                                   : defaultValue;

    public static decimal? As( this string? value, in decimal? defaultValue ) => decimal.TryParse(value, out decimal d)
                                                                                     ? d
                                                                                     : defaultValue;


    public static decimal AsDecimal<TValue>( this TValue value ) where TValue : struct, Enum => value.AsLong();
    public static double AsDouble<TValue>( this   TValue value ) where TValue : struct, Enum => value.AsLong();
    public static float AsFloat<TValue>( this     TValue value ) where TValue : struct, Enum => value.AsLong();
    public static ulong AsULong<TValue>( this     TValue value ) where TValue : struct, Enum => Convert.ToUInt64(value);
    public static long AsLong<TValue>( this       TValue value ) where TValue : struct, Enum => Convert.ToInt64(value);
    public static int AsInt<TValue>( this         TValue value ) where TValue : struct, Enum => Convert.ToInt32(value);
    public static uint AsUInt<TValue>( this       TValue value ) where TValue : struct, Enum => Convert.ToUInt32(value);
    public static short AsShort<TValue>( this     TValue value ) where TValue : struct, Enum => Convert.ToInt16(value);
    public static ushort AsUShort<TValue>( this   TValue value ) where TValue : struct, Enum => Convert.ToUInt16(value);
    public static byte AsByte<TValue>( this       TValue value ) where TValue : struct, Enum => Convert.ToByte(value);
    public static sbyte AsSByte<TValue>( this     TValue value ) where TValue : struct, Enum => Convert.ToSByte(value);
}
