// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
#if NET7_0_OR_GREATER
    [ Pure ]
    public static T Min<T>( this ReadOnlySpan<T> value, T start )
        where T : INumber<T>
    {
        T result = start;
        foreach ( T x in value ) { result = T.Min( result, x ); }

        return result;
    }


    [ Pure ]
    public static TNumber Min<T, TNumber>( this ReadOnlySpan<T> value, Func<T, TNumber> selector, TNumber start )
        where TNumber : INumber<TNumber>
    {
        TNumber result = start;
        foreach ( T x in value ) { result = TNumber.Min( result, selector( x ) ); }

        return result;
    }
#else
    [ Pure ]
    public static double Min( this ReadOnlySpan<double> value )
    {
        double result = 0;
        foreach ( double x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static float Min( this ReadOnlySpan<float> value )
    {
        float result = 0;
        foreach ( float x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static long Min( this ReadOnlySpan<long> value )
    {
        long result = 0;
        foreach ( long x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static ulong Min( this ReadOnlySpan<ulong> value )
    {
        ulong result = 0;
        foreach ( ulong x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static int Min( this ReadOnlySpan<int> value )
    {
        int result = 0;
        foreach ( int x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static uint Min( this ReadOnlySpan<uint> value )
    {
        uint result = 0;
        foreach ( uint x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static short Min( this ReadOnlySpan<short> value )
    {
        short result = 0;
        foreach ( short x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static ushort Min( this ReadOnlySpan<ushort> value )
    {
        ushort result = 0;
        foreach ( ushort x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    
    
    [ Pure ]
    public static int Min<T>( this ReadOnlySpan<T> value, Func<T, int> selector )
    {
        int result = 0;
        foreach ( T x in value ) { result = Math.Min( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static uint Min<T>( this ReadOnlySpan<T> value, Func<T, uint> selector )
    {
        uint result = 0;
        foreach ( T x in value ) { result = Math.Min( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static long Min<T>( this ReadOnlySpan<T> value, Func<T, long> selector )
    {
        long result = 0;
        foreach ( T x in value ) { result = Math.Min( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static ulong Min<T>( this ReadOnlySpan<T> value, Func<T, ulong> selector )
    {
        ulong result = 0;
        foreach ( T x in value ) { result = Math.Min( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static float Min<T>( this ReadOnlySpan<T> value, Func<T, float> selector )
    {
        float result = 0;
        foreach ( T x in value ) { result = Math.Min( result, selector( x ) ); }

        return result;
    }
    [ Pure ]
    public static double Min<T>( this ReadOnlySpan<T> value, Func<T, double> selector )
    {
        double result = 0;
        foreach ( T x in value ) { result = Math.Min( result, selector( x ) ); }

        return result;
    }
#endif
}
