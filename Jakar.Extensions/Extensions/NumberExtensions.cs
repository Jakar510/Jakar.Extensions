#nullable enable
namespace Jakar.Extensions;


public static class Numbers
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static byte As( this string? value, byte defaultValue ) => byte.TryParse( value, out byte result )
                                                                          ? result
                                                                          : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static byte? As( this string? value, byte? defaultValue ) => byte.TryParse( value, out byte result )
                                                                            ? result
                                                                            : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static decimal As( this string? value, decimal defaultValue ) => decimal.TryParse( value, out decimal d )
                                                                                ? d
                                                                                : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static decimal? As( this string? value, decimal? defaultValue ) => decimal.TryParse( value, out decimal d )
                                                                                  ? d
                                                                                  : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static double As( this string? value, double defaultValue ) => double.TryParse( value, out double d )
                                                                              ? d
                                                                              : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static double? As( this string? value, double? defaultValue ) => double.TryParse( value, out double d )
                                                                                ? d
                                                                                : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static float As( this string? value, float defaultValue ) => float.TryParse( value, out float d )
                                                                            ? d
                                                                            : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static float? As( this string? value, float? defaultValue ) => float.TryParse( value, out float d )
                                                                              ? d
                                                                              : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int As( this string? value, int defaultValue ) => int.TryParse( value, out int result )
                                                                        ? result
                                                                        : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int? As( this string? value, int? defaultValue ) => int.TryParse( value, out int result )
                                                                          ? result
                                                                          : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long? As( this string? value, long defaultValue ) => long.TryParse( value, out long result )
                                                                           ? result
                                                                           : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long? As( this string? value, long? defaultValue ) => long.TryParse( value, out long result )
                                                                            ? result
                                                                            : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static sbyte As( this string? value, sbyte defaultValue ) => sbyte.TryParse( value, out sbyte result )
                                                                            ? result
                                                                            : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static sbyte? As( this string? value, sbyte? defaultValue ) => sbyte.TryParse( value, out sbyte result )
                                                                              ? result
                                                                              : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static short As( this string? value, short defaultValue ) => short.TryParse( value, out short result )
                                                                            ? result
                                                                            : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static short? As( this string? value, short? defaultValue ) => short.TryParse( value, out short result )
                                                                              ? result
                                                                              : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint As( this string? value, uint defaultValue ) => uint.TryParse( value, out uint result )
                                                                          ? result
                                                                          : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint? As( this string? value, uint? defaultValue ) => uint.TryParse( value, out uint result )
                                                                            ? result
                                                                            : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong As( this string? value, ulong defaultValue ) => ulong.TryParse( value, out ulong result )
                                                                            ? result
                                                                            : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong? As( this string? value, ulong? defaultValue ) => ulong.TryParse( value, out ulong result )
                                                                              ? result
                                                                              : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ushort As( this string? value, ushort defaultValue ) => ushort.TryParse( value, out ushort result )
                                                                              ? result
                                                                              : defaultValue;
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ushort? As( this string? value, ushort? defaultValue ) => ushort.TryParse( value, out ushort result )
                                                                                ? result
                                                                                : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this decimal value ) => (byte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this double  value ) => (byte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this float   value ) => (byte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this long    value ) => (byte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this int     value ) => (byte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this short   value ) => (byte)value;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static float AsFloat( this decimal value ) => (float)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static float AsFloat( this double  value ) => (float)value;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this decimal value ) => (int)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this double  value ) => (int)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this float   value ) => (int)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this long    value ) => (int)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this ulong   value ) => (int)value;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this decimal value ) => (sbyte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this double  value ) => (sbyte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this float   value ) => (sbyte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this long    value ) => (sbyte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this int     value ) => (sbyte)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this short   value ) => (sbyte)value;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this decimal value ) => (short)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this double  value ) => (short)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this float   value ) => (short)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this long    value ) => (short)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this int     value ) => (short)value;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this decimal value ) => (uint)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this double  value ) => (uint)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this float   value ) => (uint)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this long    value ) => (uint)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this ulong   value ) => (uint)value;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this decimal value ) => (ushort)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this double  value ) => (ushort)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this float   value ) => (ushort)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this ulong   value ) => (ushort)value;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this long    value ) => (ushort)value;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt<TEnum>( this         TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, int>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static long AsLong<TEnum>( this       TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, long>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte<TEnum>( this     TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, sbyte>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort<TEnum>( this   TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, ushort>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort<TEnum>( this     TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, short>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt<TEnum>( this       TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, uint>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ulong AsULong<TEnum>( this     TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, ulong>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static decimal AsDecimal<TEnum>( this TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, decimal>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte<TEnum>( this       TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, byte>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static double AsDouble<TEnum>( this   TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, double>.From( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static float AsFloat<TEnum>( this     TEnum value ) where TEnum : struct, Enum => CastTo<TEnum, float>.From( value );
}
