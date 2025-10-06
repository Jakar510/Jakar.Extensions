namespace Jakar.Extensions;


public static class Numbers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static double  Round( this double  value ) => Math.Round(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static float   Round( this float   value ) => (float)Math.Round(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static decimal Round( this decimal value ) => Math.Round(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static double  Floor( this double  value ) => Math.Floor(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static float   Floor( this float   value ) => (float)Math.Floor(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static decimal Floor( this decimal value ) => Math.Floor(value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static short   Abs( this short   value ) => Math.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static int     Abs( this int     value ) => Math.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static long    Abs( this long    value ) => Math.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static float   Abs( this float   value ) => Math.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static double  Abs( this double  value ) => Math.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static decimal Abs( this decimal value ) => Math.Abs(value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static short   Clamp( this short   value, short   min, short   max ) => Math.Clamp(value, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static int     Clamp( this int     value, int     min, int     max ) => Math.Clamp(value, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static uint    Clamp( this uint    value, uint    min, uint    max ) => Math.Clamp(value, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static long    Clamp( this long    value, long    min, long    max ) => Math.Clamp(value, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static ulong   Clamp( this ulong   value, ulong   min, ulong   max ) => Math.Clamp(value, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static float   Clamp( this float   value, float   min, float   max ) => Math.Clamp(value, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static double  Clamp( this double  value, double  min, double  max ) => Math.Clamp(value, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public static decimal Clamp( this decimal value, decimal min, decimal max ) => Math.Clamp(value, min, max);


    public static T Clamp<T>( this T value, T min, T max )
        where T : IComparisonOperators<T, T, bool>
    {
        if ( min > max ) { throw new ArgumentException($"{nameof(min)}: {min} > {nameof(max)}: {max}"); }

        if ( value < min ) { return min; }

        if ( value > max ) { return max; }

        return value;
    }
    public static T Min<T>( this T value, T other )
        where T : IComparisonOperators<T, T, bool> => value < other
                                                          ? other
                                                          : value;
    public static T Max<T>( this T value, T other )
        where T : IComparisonOperators<T, T, bool> => value > other
                                                          ? other
                                                          : value;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TResult As<TResult>( this string? value, TResult defaultValue )
        where TResult : struct, INumber<TResult> => TResult.TryParse(value, CultureInfo.CurrentUICulture, out TResult result)
                                                        ? result
                                                        : defaultValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TResult? As<TResult>( this string? value, TResult? defaultValue )
        where TResult : struct, INumber<TResult> => TResult.TryParse(value, CultureInfo.CurrentUICulture, out TResult result)
                                                        ? result
                                                        : defaultValue;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static byte  AsByte( this  decimal value ) => (byte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static byte  AsByte( this  double  value ) => (byte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static byte  AsByte( this  float   value ) => (byte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static byte  AsByte( this  long    value ) => (byte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static byte  AsByte( this  int     value ) => (byte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static byte  AsByte( this  short   value ) => (byte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static sbyte AsSByte( this decimal value ) => (sbyte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static sbyte AsSByte( this double  value ) => (sbyte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static sbyte AsSByte( this float   value ) => (sbyte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static sbyte AsSByte( this long    value ) => (sbyte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static sbyte AsSByte( this int     value ) => (sbyte)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static sbyte AsSByte( this short   value ) => (sbyte)value;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float AsFloat( this decimal value ) => (float)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float AsFloat( this double  value ) => (float)value;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int  AsInt( this  decimal value ) => (int)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int  AsInt( this  double  value ) => (int)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int  AsInt( this  float   value ) => (int)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int  AsInt( this  long    value ) => (int)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int  AsInt( this  ulong   value ) => (int)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint AsUInt( this decimal value ) => (uint)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint AsUInt( this double  value ) => (uint)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint AsUInt( this float   value ) => (uint)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint AsUInt( this long    value ) => (uint)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint AsUInt( this ulong   value ) => (uint)value;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static short  AsShort( this  decimal value ) => (short)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static short  AsShort( this  double  value ) => (short)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static short  AsShort( this  float   value ) => (short)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static short  AsShort( this  long    value ) => (short)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static short  AsShort( this  int     value ) => (short)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ushort AsUShort( this decimal value ) => (ushort)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ushort AsUShort( this double  value ) => (ushort)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ushort AsUShort( this float   value ) => (ushort)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ushort AsUShort( this ulong   value ) => (ushort)value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ushort AsUShort( this long    value ) => (ushort)value;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TNumber As<TEnum, TNumber>( this TEnum value )
        where TEnum : struct, Enum
        where TNumber : INumber<TNumber> => CastTo<TEnum, TNumber>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int AsInt<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, int>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static long AsLong<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, long>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static sbyte AsSByte<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, sbyte>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ushort AsUShort<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, ushort>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static short AsShort<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, short>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint AsUInt<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, uint>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong AsULong<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, ulong>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static decimal AsDecimal<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, decimal>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static byte AsByte<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, byte>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double AsDouble<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, double>.From(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float AsFloat<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, float>.From(value);
}
