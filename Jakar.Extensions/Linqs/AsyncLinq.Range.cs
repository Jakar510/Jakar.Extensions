namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static IEnumerable<T> Range<T>( T start, T count )
        where T : INumber<T> => Range( start, count, T.One );
    public static IEnumerable<T> Range<T>( T start, T count, T offset )
        where T : INumber<T>
    {
        for ( T i = start; i < start + count; i += offset ) { yield return i; }
    }
}
