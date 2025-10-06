// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure] public static double Average( this         ReadOnlySpan<double> value )                                => value.Sum() / value.Length;
    [Pure] public static float  Average( this         ReadOnlySpan<float>  value )                                => value.Sum() / value.Length;
    [Pure] public static long   Average( this         ReadOnlySpan<long>   value )                                => value.Sum() / value.Length;
    [Pure] public static ulong  Average( this         ReadOnlySpan<ulong>  value )                                => value.Sum() / (ulong)value.Length;
    [Pure] public static int    Average( this         ReadOnlySpan<int>    value )                                => value.Sum() / value.Length;
    [Pure] public static uint   Average( this         ReadOnlySpan<uint>   value )                                => (uint)( value.Sum()   / value.Length );
    [Pure] public static short  Average( this         ReadOnlySpan<short>  value )                                => (short)( value.Sum()  / value.Length );
    [Pure] public static ushort Average( this         ReadOnlySpan<ushort> value )                                => (ushort)( value.Sum() / value.Length );
    [Pure] public static int    Average<TValue>( this ReadOnlySpan<TValue> value, Func<TValue, int>    selector ) => value.Sum(selector) / value.Length;
    [Pure] public static uint   Average<TValue>( this ReadOnlySpan<TValue> value, Func<TValue, uint>   selector ) => (uint)( value.Sum(selector) / value.Length );
    [Pure] public static long   Average<TValue>( this ReadOnlySpan<TValue> value, Func<TValue, long>   selector ) => value.Sum(selector) / value.Length;
    [Pure] public static ulong  Average<TValue>( this ReadOnlySpan<TValue> value, Func<TValue, ulong>  selector ) => value.Sum(selector) / (ulong)value.Length;
    [Pure] public static float  Average<TValue>( this ReadOnlySpan<TValue> value, Func<TValue, float>  selector ) => value.Sum(selector) / value.Length;
    [Pure] public static double Average<TValue>( this ReadOnlySpan<TValue> value, Func<TValue, double> selector ) => value.Sum(selector) / value.Length;


    [Pure] public static TNumber Average<TNumber>( this ReadOnlySpan<TNumber> value )
        where TNumber : INumber<TNumber> => value.Sum() / TNumber.CreateTruncating(value.Length);


    [Pure] public static TNumber Average<TValue, TNumber>( this ReadOnlySpan<TValue> value, Func<TValue, TNumber> selector )
        where TNumber : INumber<TNumber> => value.Sum(selector) / TNumber.CreateTruncating(value.Length);
}
