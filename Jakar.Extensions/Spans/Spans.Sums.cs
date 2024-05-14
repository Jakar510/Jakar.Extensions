// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
#if NET7_0_OR_GREATER
    [Pure]
    public static T Sum<T>( this ReadOnlySpan<T> value )
        where T : INumber<T>
    {
        T result = T.Zero;
        foreach ( T x in value ) { result += x; }

        return result;
    }


    [Pure]
    public static TNumber Sum<T, TNumber>( this ReadOnlySpan<T> value, Func<T, TNumber> selector )
        where TNumber : INumber<TNumber>
    {
        TNumber result = TNumber.Zero;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
#else
    [Pure]
    public static double Sum( this ReadOnlySpan<double> value )
    {
        double result = 0;
        foreach ( double x in value ) { result += x; }

        return result;
    }
    [Pure]
    public static float Sum( this ReadOnlySpan<float> value )
    {
        float result = 0;
        foreach ( float x in value ) { result += x; }

        return result;
    }
    [Pure]
    public static long Sum( this ReadOnlySpan<long> value )
    {
        long result = 0;
        foreach ( long x in value ) { result += x; }

        return result;
    }
    [Pure]
    public static ulong Sum( this ReadOnlySpan<ulong> value )
    {
        ulong result = 0;
        foreach ( ulong x in value ) { result += x; }

        return result;
    }
    [Pure]
    public static int Sum( this ReadOnlySpan<int> value )
    {
        int result = 0;
        foreach ( int x in value ) { result += x; }

        return result;
    }
    [Pure]
    public static uint Sum( this ReadOnlySpan<uint> value )
    {
        uint result = 0;
        foreach ( uint x in value ) { result += x; }

        return result;
    }
    [Pure]
    public static short Sum( this ReadOnlySpan<short> value )
    {
        short result = 0;
        foreach ( short x in value ) { result += x; }

        return result;
    }
    [Pure]
    public static ushort Sum( this ReadOnlySpan<ushort> value )
    {
        ushort result = 0;
        foreach ( ushort x in value ) { result += x; }

        return result;
    }


    [Pure]
    public static int Sum<T>( this ReadOnlySpan<T> value, Func<T, int> selector )
    {
        int result = 0;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
    [Pure]
    public static uint Sum<T>( this ReadOnlySpan<T> value, Func<T, uint> selector )
    {
        uint result = 0;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
    [Pure]
    public static long Sum<T>( this ReadOnlySpan<T> value, Func<T, long> selector )
    {
        long result = 0;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
    [Pure]
    public static ulong Sum<T>( this ReadOnlySpan<T> value, Func<T, ulong> selector )
    {
        ulong result = 0;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
    [Pure]
    public static float Sum<T>( this ReadOnlySpan<T> value, Func<T, float> selector )
    {
        float result = 0;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
    [Pure]
    public static double Sum<T>( this ReadOnlySpan<T> value, Func<T, double> selector )
    {
        double result = 0;
        foreach ( T x in value ) { result += selector( x ); }

        return result;
    }
#endif
}
