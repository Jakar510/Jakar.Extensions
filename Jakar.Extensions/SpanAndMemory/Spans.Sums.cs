// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
#if NET7_0_OR_GREATER
    [Pure]
    public static T Sum<T>( this Span<T> value ) where T : INumber<T>
    {
        T size = T.Zero;
        foreach ( T x in value ) { size += x; }

        return size;
    }
#else
    [ Pure ]
    public static double Sum( this Span<double> value )
    {
        double size = 0;
        foreach ( double x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static float Sum( this Span<float> value )
    {
        float size = 0;
        foreach ( float x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static long Sum( this Span<long> value )
    {
        long size = 0;
        foreach ( long x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static ulong Sum( this Span<ulong> value )
    {
        ulong size = 0;
        foreach ( ulong x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static int Sum( this Span<int> value )
    {
        int size = 0;
        foreach ( int x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static uint Sum( this Span<uint> value )
    {
        uint size = 0;
        foreach ( uint x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static short Sum( this Span<short> value )
    {
        short size = 0;
        foreach ( short x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static ushort Sum( this Span<ushort> value )
    {
        ushort size = 0;
        foreach ( ushort x in value ) { size += x; }

        return size;
    }
#endif

    [ Pure ]
    public static int Sum<T>( this Span<T> value, Func<T, int> selector )
    {
        int size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static uint Sum<T>( this Span<T> value, Func<T, uint> selector )
    {
        uint size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static long Sum<T>( this Span<T> value, Func<T, long> selector )
    {
        long size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static ulong Sum<T>( this Span<T> value, Func<T, ulong> selector )
    {
        ulong size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static float Sum<T>( this Span<T> value, Func<T, float> selector )
    {
        float size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static double Sum<T>( this Span<T> value, Func<T, double> selector )
    {
        double size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }


#if NET7_0_OR_GREATER
    [Pure]
    public static T Sum<T>( this ReadOnlySpan<T> value ) where T : INumber<T>
    {
        T size = T.Zero;
        foreach ( T x in value ) { size += x; }

        return size;
    }
#else
    [ Pure ]
    public static double Sum( this ReadOnlySpan<double> value )
    {
        double size = 0;
        foreach ( double x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static float Sum( this ReadOnlySpan<float> value )
    {
        float size = 0;
        foreach ( float x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static long Sum( this ReadOnlySpan<long> value )
    {
        long size = 0;
        foreach ( long x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static ulong Sum( this ReadOnlySpan<ulong> value )
    {
        ulong size = 0;
        foreach ( ulong x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static int Sum( this ReadOnlySpan<int> value )
    {
        int size = 0;
        foreach ( int x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static uint Sum( this ReadOnlySpan<uint> value )
    {
        uint size = 0;
        foreach ( uint x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static short Sum( this ReadOnlySpan<short> value )
    {
        short size = 0;
        foreach ( short x in value ) { size += x; }

        return size;
    }
    [ Pure ]
    public static ushort Sum( this ReadOnlySpan<ushort> value )
    {
        ushort size = 0;
        foreach ( ushort x in value ) { size += x; }

        return size;
    }
#endif

    [ Pure ]
    public static int Sum<T>( this ReadOnlySpan<T> value, Func<T, int> selector )
    {
        int size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static uint Sum<T>( this ReadOnlySpan<T> value, Func<T, uint> selector )
    {
        uint size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static long Sum<T>( this ReadOnlySpan<T> value, Func<T, long> selector )
    {
        long size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static ulong Sum<T>( this ReadOnlySpan<T> value, Func<T, ulong> selector )
    {
        ulong size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static float Sum<T>( this ReadOnlySpan<T> value, Func<T, float> selector )
    {
        float size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [ Pure ]
    public static double Sum<T>( this ReadOnlySpan<T> value, Func<T, double> selector )
    {
        double size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
}
