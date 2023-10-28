// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [ Pure ] public static double Average( this Span<double> value ) => value.Sum() / value.Length;
    [ Pure ] public static float  Average( this Span<float>  value ) => value.Sum() / value.Length;
    [ Pure ] public static long   Average( this Span<long>   value ) => value.Sum() / value.Length;
    [ Pure ] public static ulong  Average( this Span<ulong>  value ) => value.Sum() / (ulong)value.Length;
    [ Pure ] public static int    Average( this Span<int>    value ) => value.Sum() / value.Length;
    [ Pure ] public static uint   Average( this Span<uint>   value ) => (uint)(value.Sum()   / value.Length);
    [ Pure ] public static short  Average( this Span<short>  value ) => (short)(value.Sum()  / value.Length);
    [ Pure ] public static ushort Average( this Span<ushort> value ) => (ushort)(value.Sum() / value.Length);


    [ Pure ] public static int    Average<T>( this Span<T> value, Func<T, int>    selector ) => value.Sum( selector ) / value.Length;
    [ Pure ] public static uint   Average<T>( this Span<T> value, Func<T, uint>   selector ) => (uint)(value.Sum( selector ) / value.Length);
    [ Pure ] public static long   Average<T>( this Span<T> value, Func<T, long>   selector ) => value.Sum( selector ) / value.Length;
    [ Pure ] public static ulong  Average<T>( this Span<T> value, Func<T, ulong>  selector ) => value.Sum( selector ) / (ulong)value.Length;
    [ Pure ] public static float  Average<T>( this Span<T> value, Func<T, float>  selector ) => value.Sum( selector ) / value.Length;
    [ Pure ] public static double Average<T>( this Span<T> value, Func<T, double> selector ) => value.Sum( selector ) / value.Length;


    [ Pure ] public static double Average( this ReadOnlySpan<double> value ) => value.Sum() / value.Length;
    [ Pure ] public static float  Average( this ReadOnlySpan<float>  value ) => value.Sum() / value.Length;
    [ Pure ] public static long   Average( this ReadOnlySpan<long>   value ) => value.Sum() / value.Length;
    [ Pure ] public static ulong  Average( this ReadOnlySpan<ulong>  value ) => value.Sum() / (ulong)value.Length;
    [ Pure ] public static int    Average( this ReadOnlySpan<int>    value ) => value.Sum() / value.Length;
    [ Pure ] public static uint   Average( this ReadOnlySpan<uint>   value ) => (uint)(value.Sum()   / value.Length);
    [ Pure ] public static short  Average( this ReadOnlySpan<short>  value ) => (short)(value.Sum()  / value.Length);
    [ Pure ] public static ushort Average( this ReadOnlySpan<ushort> value ) => (ushort)(value.Sum() / value.Length);


    [ Pure ] public static int    Average<T>( this ReadOnlySpan<T> value, Func<T, int>    selector ) => value.Sum( selector ) / value.Length;
    [ Pure ] public static uint   Average<T>( this ReadOnlySpan<T> value, Func<T, uint>   selector ) => (uint)(value.Sum( selector ) / value.Length);
    [ Pure ] public static long   Average<T>( this ReadOnlySpan<T> value, Func<T, long>   selector ) => value.Sum( selector ) / value.Length;
    [ Pure ] public static ulong  Average<T>( this ReadOnlySpan<T> value, Func<T, ulong>  selector ) => value.Sum( selector ) / (ulong)value.Length;
    [ Pure ] public static float  Average<T>( this ReadOnlySpan<T> value, Func<T, float>  selector ) => value.Sum( selector ) / value.Length;
    [ Pure ] public static double Average<T>( this ReadOnlySpan<T> value, Func<T, double> selector ) => value.Sum( selector ) / value.Length;
}
