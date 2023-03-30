// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:59 PM

namespace Jakar.Extensions;


public static partial class Spans
{
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int Sum<T>( this Span<T> value, Func<T, int> selector )
    {
        int size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static long Sum<T>( this Span<T> value, Func<T, long> selector )
    {
        long size = 0;
        foreach ( T x in value ) { size += selector( x ); }

        return size;
    }
}
