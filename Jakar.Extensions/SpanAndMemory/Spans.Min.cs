// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
#if NET7_0_OR_GREATER
    [ Pure ]
    public static T Min<T>( this Span<T> value )
        where T : INumber<T>
    {
        T result = T.Zero;
        foreach ( T x in value ) { result = Min( result, x ); }

        return result;
    }

    [ Pure, MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static T Min<T>( T x, T y )
        where T : INumber<T> => x <= y
                                    ? x
                                    : y;

#else
    [ Pure ]
    public static double Min( this Span<double> value )
    {
        double result = 0;
        foreach ( double x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static float Min( this Span<float> value )
    {
        float result = 0;
        foreach ( float x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static long Min( this Span<long> value )
    {
        long result = 0;
        foreach ( long x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static ulong Min( this Span<ulong> value )
    {
        ulong result = 0;
        foreach ( ulong x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static int Min( this Span<int> value )
    {
        int result = 0;
        foreach ( int x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static uint Min( this Span<uint> value )
    {
        uint result = 0;
        foreach ( uint x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static short Min( this Span<short> value )
    {
        short result = 0;
        foreach ( short x in value ) { result = Math.Min( result, x ); }

        return result;
    }
    [ Pure ]
    public static ushort Min( this Span<ushort> value )
    {
        ushort result = 0;
        foreach ( ushort x in value ) { result = Math.Min( result, x ); }

        return result;
    }
#endif

    [ Pure ]
    public static int Min<T>( this Span<T> value, Func<T, int> selector )
    {
        int result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static uint Min<T>( this Span<T> value, Func<T, uint> selector )
    {
        uint result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static long Min<T>( this Span<T> value, Func<T, long> selector )
    {
        long result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static ulong Min<T>( this Span<T> value, Func<T, ulong> selector )
    {
        ulong result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static float Min<T>( this Span<T> value, Func<T, float> selector )
    {
        float result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static double Min<T>( this Span<T> value, Func<T, double> selector )
    {
        double result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }


#if NET7_0_OR_GREATER
    [ Pure ]
    public static T Min<T>( this ReadOnlySpan<T> value )
        where T : INumber<T>
    {
        T result = T.Zero;
        foreach ( T x in value ) { result = Min( result, x ); }

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
#endif

    [ Pure ]
    public static int Min<T>( this ReadOnlySpan<T> value, Func<T, int> selector )
    {
        int result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static uint Min<T>( this ReadOnlySpan<T> value, Func<T, uint> selector )
    {
        uint result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static long Min<T>( this ReadOnlySpan<T> value, Func<T, long> selector )
    {
        long result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static ulong Min<T>( this ReadOnlySpan<T> value, Func<T, ulong> selector )
    {
        ulong result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static float Min<T>( this ReadOnlySpan<T> value, Func<T, float> selector )
    {
        float result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
    [ Pure ]
    public static double Min<T>( this ReadOnlySpan<T> value, Func<T, double> selector )
    {
        double result = 0;
        foreach ( T x in value ) { result = Math.Min(result, selector( x )); }

        return result;
    }
}
