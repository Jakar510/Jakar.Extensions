// Jakar.Extensions :: Jakar.Extensions
// 09/11/2022  3:02 PM

namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TElement>( IAsyncEnumerable<TElement> source )
    {
        public async ValueTask<TElement> First()
        {
            await foreach ( TElement element in source ) { return element; }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> First( Func<TElement, bool> selector )
        {
            await foreach ( TElement element in source )
            {
                if ( selector(element) ) { return element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> First( Func<TElement, ValueTask<bool>> selector )
        {
            await foreach ( TElement element in source )
            {
                if ( await selector(element) ) { return element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> First<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            await foreach ( TElement element in source )
            {
                if ( selector(element, value) ) { return element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> First<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            await foreach ( TElement element in source )
            {
                if ( await selector(element, value) ) { return element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement?> FirstOrDefault()
        {
            await foreach ( TElement element in source ) { return element; }

            return default;
        }
        public async ValueTask<TElement?> FirstOrDefault( Func<TElement, bool> selector )
        {
            await foreach ( TElement element in source )
            {
                if ( selector(element) ) { return element; }
            }

            return default;
        }
        public async ValueTask<TElement?> FirstOrDefault( Func<TElement, ValueTask<bool>> selector )
        {
            await foreach ( TElement element in source )
            {
                if ( await selector(element) ) { return element; }
            }

            return default;
        }
        public async ValueTask<TElement?> FirstOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            await foreach ( TElement element in source )
            {
                if ( selector(element, value) ) { return element; }
            }

            return default;
        }
        public async ValueTask<TElement?> FirstOrDefault<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            await foreach ( TElement element in source )
            {
                if ( await selector(element, value) ) { return element; }
            }

            return default;
        }
        public async ValueTask<TElement> Last( CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);
            return list.Last();
        }
        public async ValueTask<TElement> Last( Func<TElement, bool> selector, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);
            return list.Last(selector);
        }
        public async ValueTask<TElement> Last( Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( await selector(list[i]) ) { return list[i]; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Last<TValue>( Func<TElement, TValue, bool> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( selector(list[i], value) ) { return list[i]; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Last<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( await selector(list[i], value) ) { return list[i]; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement?> LastOrDefault( CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);
            return list.LastOrDefault();
        }
        public async ValueTask<TElement?> LastOrDefault( Func<TElement, bool> selector, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);
            return list.LastOrDefault(selector);
        }
        public async ValueTask<TElement?> LastOrDefault( Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( await selector(list[i]) ) { return list[i]; }
            }

            return default;
        }
        public async ValueTask<TElement?> LastOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( selector(list[i], value) ) { return list[i]; }
            }

            return default;
        }
        public async ValueTask<TElement?> LastOrDefault<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value, CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token);

            for ( int i = list.Count; i < 0; i-- )
            {
                if ( await selector(list[i], value) ) { return list[i]; }
            }

            return default;
        }
        public async ValueTask<TElement> Single()
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

                result = element;
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Single( Func<TElement, bool> selector )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

                if ( selector(element) ) { result = element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Single( Func<TElement, ValueTask<bool>> selector )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

                if ( await selector(element) ) { result = element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Single<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

                if ( selector(element, value) ) { result = element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement> Single<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

                if ( await selector(element, value) ) { result = element; }
            }

            throw new InvalidOperationException($"No records in {nameof(source)}");
        }
        public async ValueTask<TElement?> SingleOrDefault()
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { return default; }

                result = element;
            }

            return default;
        }
        public async ValueTask<TElement?> SingleOrDefault( Func<TElement, bool> selector )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { return default; }

                if ( selector(element) ) { result = element; }
            }

            return default;
        }
        public async ValueTask<TElement?> SingleOrDefault( Func<TElement, ValueTask<bool>> selector )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { return default; }

                if ( await selector(element) ) { result = element; }
            }

            return default;
        }
        public async ValueTask<TElement?> SingleOrDefault<TValue>( Func<TElement, TValue, bool> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { return default; }

                if ( selector(element, value) ) { result = element; }
            }

            return default;
        }
        public async ValueTask<TElement?> SingleOrDefault<TValue>( Func<TElement, TValue, ValueTask<bool>> selector, TValue value )
        {
            TElement? result = default;

            await foreach ( TElement element in source )
            {
                if ( result is not null ) { return default; }

                if ( await selector(element, value) ) { result = element; }
            }

            return result;
        }
    }
}
