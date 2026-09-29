namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static IEnumerable<TValue> Range<TValue>( TValue start, TValue count )
        where TValue : INumber<TValue> => Range(start, count, TValue.One);
    public static IEnumerable<TValue> Range<TValue>( TValue start, TValue count, TValue offset )
        where TValue : INumber<TValue>
    {
        for ( TValue i = start; i < start + count; i += offset ) { yield return i; }
    }
}
