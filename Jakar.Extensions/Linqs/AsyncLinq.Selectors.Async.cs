// Jakar.Extensions :: Jakar.Extensions
// 09/11/2022  3:02 PM

namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TElement>( IAsyncEnumerable<TElement> source )
    {
        public async ValueTask<TElement> First<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( selector(element, value) ) { return element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> First<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( await selector(element, value)
                        .ConfigureAwait(false) ) { return element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }

        public async ValueTask<TElement?> FirstOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( selector(element, value) ) { return element; }
            }

            return default;
        }
        public async ValueTask<TElement?> FirstOrDefault<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( await selector(element, value)
                        .ConfigureAwait(false) ) { return element; }
            }

            return default;
        }

        public async ValueTask<TElement> Last<TValue>( Func<TElement, TValue, bool> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token)
                                              .ConfigureAwait(false);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( selector(list[i], value) ) { return list[i]; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Last<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token)
                                              .ConfigureAwait(false);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( await selector(list[i], value)
                        .ConfigureAwait(false) ) { return list[i]; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }

        public async ValueTask<TElement?> LastOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token)
                                              .ConfigureAwait(false);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( selector(list[i], value) ) { return list[i]; }
            }

            return default;
        }
        public async ValueTask<TElement?> LastOrDefault<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token)
                                              .ConfigureAwait(false);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( await selector(list[i], value)
                        .ConfigureAwait(false) ) { return list[i]; }
            }

            return default;
        }

        public async ValueTask<TElement> Single<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

                if ( selector(element, value) ) { result = element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Single<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

                if ( await selector(element, value)
                        .ConfigureAwait(false) ) { result = element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }

        public async ValueTask<TElement?> SingleOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( result is not null ) { return default; }

                if ( selector(element, value) ) { result = element; }
            }

            return default;
        }
        public async ValueTask<TElement?> SingleOrDefault<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( result is not null ) { return default; }

                if ( await selector(element, value)
                        .ConfigureAwait(false) ) { result = element; }
            }

            return result;
        }
    }
}
