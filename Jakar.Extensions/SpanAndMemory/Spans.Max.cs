// Jakar.Extensions :: Jakar.Extensions
// double.MinValue3/29/2double.MinValue23  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
#if NET7_0_OR_GREATER
    [ Pure ]
    public static T Max<T>( this ReadOnlySpan<T> value, T start )
        where T : INumber<T>
    {
        T result = start;
        foreach ( T x in value ) { result = T.Max( result, x ); }

        return result;
    }


    [ Pure ]
    public static TNumber Max<T, TNumber>( this ReadOnlySpan<T> value, Func<T, TNumber> selector, TNumber start )
        where TNumber : INumber<TNumber>
    {
        TNumber result = start;
        foreach ( T x in value ) { result = TNumber.Max( result, selector( x ) ); }

        return result;
    }
#else
    [ Pure ]
    public static double Max( this ReadOnlySpan<double> value )
    {
        double result = double.MinValue;
        foreach ( double x in value ) { result = Math.Max( result, x ); }

        return result;
    }
    [ Pure ]
    public static float Max( this ReadOnlySpan<float> value )
    {
        float result = float.MinValue;
        foreach ( float x in value ) { result = Math.Max( result, x ); }

        return result;
    }
    [ Pure ]
    public static long Max( this ReadOnlySpan<long> value )
    {
        long result = long.MinValue;
        foreach ( long x in value ) { result = Math.Max( result, x ); }

        return result;
    }
    [ Pure ]
    public static ulong Max( this ReadOnlySpan<ulong> value )
    {
        ulong result = ulong.MinValue;
        foreach ( ulong x in value ) { result = Math.Max( result, x ); }

        return result;
    }
    [ Pure ]
    public static int Max( this ReadOnlySpan<int> value )
    {
        int result = int.MinValue;
        foreach ( int x in value ) { result = Math.Max( result, x ); }

        return result;
    }
    [ Pure ]
    public static uint Max( this ReadOnlySpan<uint> value )
    {
        uint result = uint.MinValue;
        foreach ( uint x in value ) { result = Math.Max( result, x ); }

        return result;
    }
    [ Pure ]
    public static short Max( this ReadOnlySpan<short> value )
    {
        short result = short.MinValue;
        foreach ( short x in value ) { result = Math.Max( result, x ); }

        return result;
    }
    [ Pure ]
    public static ushort Max( this ReadOnlySpan<ushort> value )
    {
        ushort result = ushort.MinValue;
        foreach ( ushort x in value ) { result = Math.Max( result, x ); }

        return result;
    }


    [ Pure ]
    public static int Max<T>( this ReadOnlySpan<T> value, Func<T, int> selector )
    {
        int result = int.MinValue;
        foreach ( T x in value ) { result = Math.Max( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static uint Max<T>( this ReadOnlySpan<T> value, Func<T, uint> selector )
    {
        uint result = uint.MinValue;
        foreach ( T x in value ) { result = Math.Max( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static long Max<T>( this ReadOnlySpan<T> value, Func<T, long> selector )
    {
        long result = long.MinValue;
        foreach ( T x in value ) { result = Math.Max( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static ulong Max<T>( this ReadOnlySpan<T> value, Func<T, ulong> selector )
    {
        ulong result = ulong.MinValue;
        foreach ( T x in value ) { result = Math.Max( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static float Max<T>( this ReadOnlySpan<T> value, Func<T, float> selector )
    {
        float result = float.MinValue;
        foreach ( T x in value ) { result = Math.Max( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static double Max<T>( this ReadOnlySpan<T> value, Func<T, double> selector )
    {
        double result = double.MinValue;
        foreach ( T x in value ) { result = Math.Max( result, selector( x ) ); }

        return result;
    }
#endif
}
