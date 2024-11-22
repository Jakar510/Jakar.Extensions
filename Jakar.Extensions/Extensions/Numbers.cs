namespace Jakar.Extensions;


public static class Numbers
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TResult As<TResult>( this string? value, TResult defaultValue )
        where TResult : struct, INumber<TResult> => TResult.TryParse( value, CultureInfo.CurrentUICulture, out TResult result )
                                                        ? result
                                                        : defaultValue;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TResult? As<TResult>( this string? value, TResult? defaultValue )
        where TResult : struct, INumber<TResult> => TResult.TryParse( value, CultureInfo.CurrentUICulture, out TResult result )
                                                        ? result
                                                        : defaultValue;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this decimal value ) => byte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this double  value ) => byte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this float   value ) => byte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this long    value ) => byte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this int     value ) => byte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static byte AsByte( this short   value ) => byte.CreateTruncating( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static float AsFloat( this decimal value ) => float.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static float AsFloat( this double  value ) => float.CreateTruncating( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this decimal value ) => int.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this double  value ) => int.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this float   value ) => int.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this long    value ) => int.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int AsInt( this ulong   value ) => int.CreateTruncating( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this decimal value ) => sbyte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this double  value ) => sbyte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this float   value ) => sbyte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this long    value ) => sbyte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this int     value ) => sbyte.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static sbyte AsSByte( this short   value ) => sbyte.CreateTruncating( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this decimal value ) => short.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this double  value ) => short.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this float   value ) => short.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this long    value ) => short.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static short AsShort( this int     value ) => short.CreateTruncating( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this decimal value ) => uint.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this double  value ) => uint.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this float   value ) => uint.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this long    value ) => uint.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static uint AsUInt( this ulong   value ) => uint.CreateTruncating( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this decimal value ) => ushort.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this double  value ) => ushort.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this float   value ) => ushort.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this ulong   value ) => ushort.CreateTruncating( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ushort AsUShort( this long    value ) => ushort.CreateTruncating( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TNumber As<TEnum, TNumber>( this TEnum value )
        where TEnum : struct, Enum
        where TNumber : INumber<TNumber> => CastTo<TEnum, TNumber>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int AsInt<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, int>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long AsLong<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, long>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static sbyte AsSByte<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, sbyte>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ushort AsUShort<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, ushort>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static short AsShort<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, short>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static uint AsUInt<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, uint>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ulong AsULong<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, ulong>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static decimal AsDecimal<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, decimal>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static byte AsByte<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, byte>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static double AsDouble<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, double>.From( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static float AsFloat<TEnum>( this TEnum value )
        where TEnum : struct, Enum => CastTo<TEnum, float>.From( value );
}
