// Jakar.Extensions :: Jakar.Extensions
// 03/31/2023  11:56 AM

namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TElement>( IEnumerable<TElement> enumerable )
    {
        public TElement? First<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            foreach ( TElement element in enumerable )
            {
                if ( selector(element, value) ) { return element; }
            }

            return default;
        }
        public TElement? FirstOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            foreach ( TElement element in enumerable )
            {
                if ( selector(element, value) ) { return element; }
            }

            return default;
        }
        public TElement? Single<TValue>( Func<TElement, TValue, bool> selector, TValue value )
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
        public TElement? SingleOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value )
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
}
