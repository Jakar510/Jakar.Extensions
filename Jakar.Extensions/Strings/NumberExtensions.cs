// unset


namespace Jakar.Extensions.Strings;


public static class Numbers
{
    public static float  ToFloat( this  decimal value ) => (float)value;
    public static int    ToInt( this    decimal value ) => (int)value;
    public static uint   ToUInt( this   decimal value ) => (uint)value;
    public static ushort ToUShort( this decimal value ) => (ushort)value;
    public static short  ToShort( this  decimal value ) => (short)value;
    public static sbyte  ToSByte( this  decimal value ) => (sbyte)value;
    public static byte   ToByte( this   decimal value ) => (byte)value;


    public static float  ToFloat( this  double value ) => (float)value;
    public static int    ToInt( this    double value ) => (int)value;
    public static uint   ToUInt( this   double value ) => (uint)value;
    public static ushort ToUShort( this double value ) => (ushort)value;
    public static short  ToShort( this  double value ) => (short)value;
    public static sbyte  ToSByte( this  double value ) => (sbyte)value;
    public static byte   ToByte( this   double value ) => (byte)value;


    public static float  ToFloat( this  float value ) => (float)value;
    public static int    ToInt( this    float value ) => (int)value;
    public static uint   ToUInt( this   float value ) => (uint)value;
    public static ushort ToUShort( this float value ) => (ushort)value;
    public static short  ToShort( this  float value ) => (short)value;
    public static sbyte  ToSByte( this  float value ) => (sbyte)value;
    public static byte   ToByte( this   float value ) => (byte)value;


    public static byte To( this string? value, in byte defaultValue ) => byte.TryParse(value, out byte result)
                                                                             ? result
                                                                             : defaultValue;

    public static byte? To( this string? value, in byte? defaultValue ) => byte.TryParse(value, out byte result)
                                                                               ? result
                                                                               : defaultValue;

    public static sbyte To( this string? value, in sbyte defaultValue ) => sbyte.TryParse(value, out sbyte result)
                                                                               ? result
                                                                               : defaultValue;

    public static sbyte? To( this string? value, in sbyte? defaultValue ) => sbyte.TryParse(value, out sbyte result)
                                                                                 ? result
                                                                                 : defaultValue;


    public static short To( this string? value, in short defaultValue ) => short.TryParse(value, out short result)
                                                                               ? result
                                                                               : defaultValue;

    public static short? To( this string? value, in short? defaultValue ) => short.TryParse(value, out short result)
                                                                                 ? result
                                                                                 : defaultValue;

    public static ushort To( this string? value, in ushort defaultValue ) => ushort.TryParse(value, out ushort result)
                                                                                 ? result
                                                                                 : defaultValue;

    public static ushort? To( this string? value, in ushort? defaultValue ) => ushort.TryParse(value, out ushort result)
                                                                                   ? result
                                                                                   : defaultValue;


    public static int To( this string? value, in int defaultValue ) => int.TryParse(value, out int result)
                                                                           ? result
                                                                           : defaultValue;

    public static int? To( this string? value, in int? defaultValue ) => int.TryParse(value, out int result)
                                                                             ? result
                                                                             : defaultValue;

    public static uint To( this string? value, in uint defaultValue ) => uint.TryParse(value, out uint result)
                                                                             ? result
                                                                             : defaultValue;

    public static uint? To( this string? value, in uint? defaultValue ) => uint.TryParse(value, out uint result)
                                                                               ? result
                                                                               : defaultValue;


    public static long? To( this string? value, in long? defaultValue ) => string.IsNullOrWhiteSpace(value)
                                                                               ? default
                                                                               : long.TryParse(value, out long result)
                                                                                   ? result
                                                                                   : defaultValue;

    public static long? To( this string? value, in long defaultValue ) => long.TryParse(value, out long result)
                                                                              ? result
                                                                              : defaultValue;

    public static ulong To( this string? value, in ulong defaultValue ) => ulong.TryParse(value, out ulong result)
                                                                               ? result
                                                                               : defaultValue;

    public static ulong? To( this string? value, in ulong? defaultValue ) => ulong.TryParse(value, out ulong result)
                                                                                 ? result
                                                                                 : defaultValue;


    public static float To( this string value, in float defaultValue ) => float.TryParse(value, out float d)
                                                                              ? d
                                                                              : defaultValue;

    public static float? To( this string value, in float? defaultValue ) => float.TryParse(value, out float d)
                                                                                ? d
                                                                                : defaultValue;


    public static double To( this string value, in double defaultValue ) => double.TryParse(value, out double d)
                                                                                ? d
                                                                                : defaultValue;

    public static double? To( this string value, in double? defaultValue ) => double.TryParse(value, out double d)
                                                                                  ? d
                                                                                  : defaultValue;


    public static decimal To( this string value, in decimal defaultValue ) => decimal.TryParse(value, out decimal d)
                                                                                  ? d
                                                                                  : defaultValue;

    public static decimal? To( this string value, in decimal? defaultValue ) => decimal.TryParse(value, out decimal d)
                                                                                    ? d
                                                                                    : defaultValue;


    public static decimal ToDecimal<TValue>( this TValue value ) where TValue : struct, Enum => value.ToLong();
    public static double  ToDouble<TValue>( this  TValue value ) where TValue : struct, Enum => value.ToLong();
    public static float   ToFloat<TValue>( this   TValue value ) where TValue : struct, Enum => value.ToLong();
    public static ulong   ToULong<TValue>( this   TValue value ) where TValue : struct, Enum => Convert.ToUInt64(value);
    public static long    ToLong<TValue>( this    TValue value ) where TValue : struct, Enum => Convert.ToInt64(value);
    public static int     ToInt<TValue>( this     TValue value ) where TValue : struct, Enum => Convert.ToInt32(value);
    public static uint    ToUInt<TValue>( this    TValue value ) where TValue : struct, Enum => Convert.ToUInt32(value);
    public static short   ToShort<TValue>( this   TValue value ) where TValue : struct, Enum => Convert.ToInt16(value);
    public static ushort  ToUShort<TValue>( this  TValue value ) where TValue : struct, Enum => Convert.ToUInt16(value);
    public static byte    ToByte<TValue>( this    TValue value ) where TValue : struct, Enum => Convert.ToByte(value);
    public static sbyte   ToSByte<TValue>( this   TValue value ) where TValue : struct, Enum => Convert.ToSByte(value);
}
