// Jakar.Extensions :: Jakar.Extensions
// 03/31/2023  11:56 AM

namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static TElement? First<TElement, TValue>( this IEnumerable<TElement> enumerable, Func<TElement, TValue, bool> selector, TValue value )
    {
        foreach ( TElement element in enumerable )
        {
            if ( selector(element, value) ) { return element; }
        }

        return default;
    }
    public static TElement? FirstOrDefault<TElement, TValue>( this IEnumerable<TElement> enumerable, Func<TElement, TValue, bool> selector, TValue value )
    {
        foreach ( TElement element in enumerable )
        {
            if ( selector(element, value) ) { return element; }
        }

        return default;
    }


    public static TElement? Single<TElement, TValue>( this IEnumerable<TElement> enumerable, Func<TElement, TValue, bool> selector, TValue value )
    {
        Enumerable.Range(0, 10)
                  .AsAsyncEnumerable();

        TElement? result = default;

        foreach ( TElement element in enumerable )
        {
            if ( !selector(element, value) ) { continue; }

            if ( result is not null ) { throw new InvalidOperationException($"{nameof(enumerable)} has multiple results"); }

            result = element;
            break;
        }

        return result;
    }
    public static TElement? SingleOrDefault<TElement, TValue>( this IEnumerable<TElement> enumerable, Func<TElement, TValue, bool> selector, TValue value )
    {
        TElement? result = default;

        foreach ( TElement element in enumerable )
        {
            if ( !selector(element, value) ) { continue; }

            if ( result is not null ) { return default; }

            result = element;
            break;
        }

        return result;
    }
}
