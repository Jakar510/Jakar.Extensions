// Jakar.Extensions :: Jakar.Extensions
// 09/11/2022  3:00 PM

#pragma warning disable CS8424
namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TElement>( IAsyncEnumerable<TElement> source )
    {
        public async IAsyncEnumerable<TElement> Append( TElement value )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return element; }

            yield return value;
        }
        public async IAsyncEnumerable<TElement> Append( IAsyncEnumerable<TElement> values )
        {
            await foreach ( TElement element in values.ConfigureAwait(false) ) { yield return element; }

            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return element; }
        }
        public async IAsyncEnumerable<TElement> Append( IEnumerable<TElement> values )
        {
            foreach ( TElement element in values ) { yield return element; }

            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return element; }
        }
        public async IAsyncEnumerable<TElement> Prepend( TElement value )
        {
            yield return value;
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return element; }
        }
        public async IAsyncEnumerable<TElement> Prepend( IEnumerable<TElement> values )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return element; }

            foreach ( TElement element in values ) { yield return element; }
        }
        public async IAsyncEnumerable<TElement> Prepend( IAsyncEnumerable<TElement> values )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return element; }

            await foreach ( TElement element in values.ConfigureAwait(false) ) { yield return element; }
        }
        public async IAsyncEnumerable<TElement1> Cast<TElement1>( Func<TElement, TElement1> func )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return func(element); }
        }
    }



    extension( IAsyncEnumerable<object> source )
    {
        public async IAsyncEnumerable<TElement> Cast<TElement>()
        {
            await foreach ( object element in source.ConfigureAwait(false) ) { yield return (TElement)element; }
        }
        public async IAsyncEnumerable<TElement> CastSafe<TElement>()
        {
            await foreach ( object element in source.ConfigureAwait(false) )
            {
                if ( element is TElement value ) { yield return value; }
            }
        }
    }



    extension<TElement>( IAsyncEnumerable<TElement> source )
    {
        public IAsyncEnumerable<TElement> Distinct() => source.Distinct(EqualityComparer<TElement>.Default);
        public async IAsyncEnumerable<TElement> Distinct( EqualityComparer<TElement> comparer )
        {
            HashSet<TElement> set = await source.ToHashSet(comparer).ConfigureAwait(false);
            foreach ( TElement element in set ) { yield return element; }
        }
        public IAsyncEnumerable<TElement> DistinctBy<TKey>( Func<TElement, TKey> keySelector ) =>
            source.DistinctBy(keySelector, EqualityComparer<TKey>.Default);
        public IAsyncEnumerable<TElement> DistinctBy<TKey>( Func<TElement, ValueTask<TKey>> keySelector ) =>
            source.DistinctBy(keySelector, EqualityComparer<TKey>.Default);
        public async IAsyncEnumerable<TElement> DistinctBy<TKey>( Func<TElement, TKey> keySelector, EqualityComparer<TKey> comparer )
        {
            HashSet<TKey> set = new(comparer);

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( set.Add(keySelector(element)) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TElement> DistinctBy<TKey>( Func<TElement, ValueTask<TKey>> keySelector, EqualityComparer<TKey> comparer )
        {
            HashSet<TKey> set = new(comparer);

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( set.Add(await keySelector(element)
                                .ConfigureAwait(false)) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TElement> Skip( int count, int start = 0 )
        {
            await foreach ( ( int index, TElement? value ) in source.Enumerate(start).ConfigureAwait(false) )
            {
                if ( index >= count ) { yield return value; }
            }
        }
        public async IAsyncEnumerable<TElement> SkipLast( int count, [EnumeratorCancellation] CancellationToken token = default )
        {
            List<TElement> list = await source.ToList(DEFAULT_CAPACITY, token).ConfigureAwait(false);

            for ( int index = 0; index < list.Count; index++ )
            {
                if ( token.IsCancellationRequested ) { yield break; }

                if ( index >= count ) { yield break; }

                yield return list[index];
            }
        }
        public async IAsyncEnumerable<TElement> SkipWhile( Func<TElement, bool> predicate )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( predicate(element) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TElement> SkipWhile( Func<TElement, ValueTask<bool>> predicate )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( await predicate(element).ConfigureAwait(false) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TElement> SkipWhile( Func<TElement, int, bool> predicate, int start = 0 )
        {
            await foreach ( ( int index, TElement? value ) in source.Enumerate(start).ConfigureAwait(false) )
            {
                if ( predicate(value, index) ) { yield return value; }
            }
        }
        public async IAsyncEnumerable<TElement> SkipWhile( Func<TElement, int, ValueTask<bool>> predicate, int start = 0 )
        {
            await foreach ( ( int index, TElement? value ) in source.Enumerate(start).ConfigureAwait(false) )
            {
                if ( await predicate(value, index).ConfigureAwait(false) ) { yield return value; }
            }
        }
        public async IAsyncEnumerable<TElement> Where( Func<TElement, bool> predicate )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( predicate(element) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TElement> Where( Func<TElement, ValueTask<bool>> predicate )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                if ( await predicate(element).ConfigureAwait(false) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TElement> Where( Func<TElement, int, bool> predicate )
        {
            int index = NOT_FOUND;

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                checked { index++; }

                if ( predicate(element, index) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TElement> Where( Func<TElement, int, ValueTask<bool>> predicate )
        {
            int index = NOT_FOUND;

            await foreach ( TElement element in source.ConfigureAwait(false) )
            {
                checked { index++; }

                if ( await predicate(element, index).ConfigureAwait(false) ) { yield return element; }
            }
        }
        public async IAsyncEnumerable<TResult> Select<TResult>( Func<TElement, TResult> selector )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return selector(element); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TResult>( Func<TElement, TArg1, TResult> selector, TArg1 arg1 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return selector(element, arg1); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TArg2, TResult>( Func<TElement, TArg1, TArg2, TResult> selector, TArg1 arg1, TArg2 arg2 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return selector(element, arg1, arg2); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TArg2, TArg3, TResult>( Func<TElement, TArg1, TArg2, TArg3, TResult> selector, TArg1 arg1, TArg2 arg2, TArg3 arg3 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return selector(element, arg1, arg2, arg3); }
        }
        public async IAsyncEnumerable<TResult> Select<TResult>( Func<TElement, ValueTask<TResult>> selector )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TResult>( Func<TElement, TArg1, ValueTask<TResult>> selector, TArg1 arg1 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element, arg1).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TArg2, TResult>( Func<TElement, TArg1, TArg2, ValueTask<TResult>> selector, TArg1 arg1, TArg2 arg2 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element, arg1, arg2).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TArg2, TArg3, TResult>( Func<TElement, TArg1, TArg2, TArg3, ValueTask<TResult>> selector, TArg1 arg1, TArg2 arg2, TArg3 arg3 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element, arg1, arg2, arg3).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TResult>( Func<TElement, Task<TResult>> selector )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TResult>( Func<TElement, TArg1, Task<TResult>> selector, TArg1 arg1 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element, arg1).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TArg2, TResult>( Func<TElement, TArg1, TArg2, Task<TResult>> selector, TArg1 arg1, TArg2 arg2 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element, arg1, arg2).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TArg1, TArg2, TArg3, TResult>( Func<TElement, TArg1, TArg2, TArg3, Task<TResult>> selector, TArg1 arg1, TArg2 arg2, TArg3 arg3 )
        {
            await foreach ( TElement element in source.ConfigureAwait(false) ) { yield return await selector(element, arg1, arg2, arg3).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TResult>( Func<TElement, int, TResult> selector, int start = 0 )
        {
            await foreach ( ( int index, TElement? value ) in source.Enumerate(start).ConfigureAwait(false) ) { yield return selector(value, index); }
        }
        public async IAsyncEnumerable<TResult> Select<TResult>( Func<TElement, int, ValueTask<TResult>> selector, int start = 0 )
        {
            await foreach ( ( int index, TElement? value ) in source.Enumerate(start).ConfigureAwait(false) ) { yield return await selector(value, index).ConfigureAwait(false); }
        }
        public async IAsyncEnumerable<TResult> Select<TResult>( Func<TElement, long, TResult> selector, long start = 0 )
        {
            await foreach ( ( long index, TElement? value ) in source.Enumerate(start).ConfigureAwait(false) ) { yield return selector(value, index); }
        }
        public async IAsyncEnumerable<TResult> Select<TResult>( Func<TElement, long, ValueTask<TResult>> selector, long start = 0 )
        {
            await foreach ( ( long index, TElement? value ) in source.Enumerate(start).ConfigureAwait(false) ) { yield return await selector(value, index).ConfigureAwait(false); }
        }
    }



    extension<TElement>( IEnumerable<IEnumerable<TElement>> values )
    {
        public IEnumerable<TElement> Consolidate()
        {
            List<TElement> results = new();
            foreach ( IEnumerable<TElement> element in values ) { results.AddRange(element); }

            return results;
        }
        public IEnumerable<TElement> ConsolidateUnique()
        {
            HashSet<TElement> results = new();

            foreach ( IEnumerable<TElement> element in values )
            {
                foreach ( TElement item in element ) { results.Add(item); }
            }

            return results;
        }
    }



    extension<TElement>( IAsyncEnumerable<IAsyncEnumerable<TElement>> source )
    {
        public async IAsyncEnumerable<TElement> Consolidate()
        {
            await foreach ( IAsyncEnumerable<TElement> element in source.ConfigureAwait(false) )
            {
                await foreach ( TElement item in element.ConfigureAwait(false) ) { yield return item; }
            }
        }
        public async IAsyncEnumerable<TElement> ConsolidateUnique( [EnumeratorCancellation] CancellationToken token = default )
        {
            HashSet<TElement> results = new();

            await foreach ( TElement element in source.Consolidate()
                                                      .WithCancellation(token).ConfigureAwait(false) ) { results.Add(element); }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach ( TElement element in results )
            {
                if ( token.IsCancellationRequested ) { yield break; }

                yield return element;
            }
        }
    }



    extension<TElement>( IAsyncEnumerable<IEnumerable<TElement>> source )
    {
        public async IAsyncEnumerable<TElement> Consolidate()
        {
            await foreach ( IEnumerable<TElement> element in source.ConfigureAwait(false) )
            {
                foreach ( TElement item in element ) { yield return item; }
            }
        }
        public async IAsyncEnumerable<TElement> ConsolidateUnique( [EnumeratorCancellation] CancellationToken token = default )
        {
            HashSet<TElement> results = new();

            await foreach ( TElement element in source.Consolidate()
                                                      .WithCancellation(token).ConfigureAwait(false) ) { results.Add(element); }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach ( TElement element in results )
            {
                if ( token.IsCancellationRequested ) { yield break; }

                yield return element;
            }
        }
    }



    public static IEnumerable<string> WhereNotNull( this IEnumerable<string?> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( string? element in values )
        {
            if ( !string.IsNullOrEmpty(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<string> WhereNotNull( this IAsyncEnumerable<string?> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        await foreach ( string? element in values.ConfigureAwait(false) )
        {
            if ( !string.IsNullOrEmpty(element) ) { yield return element; }
        }
    }


    public static IEnumerable<TElement> WhereNotNull<TElement>( this IEnumerable<TElement?> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in values )
        {
            if ( element is not null ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> WhereNotNull<TElement>( this IAsyncEnumerable<TElement?> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        await foreach ( TElement? element in values.ConfigureAwait(false) )
        {
            if ( element is not null ) { yield return element; }
        }
    }


    public static async Task<IEnumerable<TElement>> WhereNotNull<TElement>( this Task<IEnumerable<TElement?>> values )
    {
        IEnumerable<TElement?> results = await values.ConfigureAwait(false);
        return results.WhereNotNull();
    }
    public static async ValueTask<IEnumerable<TElement>> WhereNotNull<TElement>( this ValueTask<IEnumerable<TElement?>> values )
    {
        IEnumerable<TElement?> results = await values.ConfigureAwait(false);
        return results.WhereNotNull();
    }


    public static async Task<IEnumerable<TElement>> WhereNotNull<TCollection, TElement>( this Task<TCollection> values )
        where TCollection : IEnumerable<TElement?>
    {
        IEnumerable<TElement?> results = await values.ConfigureAwait(false);
        return results.WhereNotNull();
    }
    public static async ValueTask<IEnumerable<TElement>> WhereNotNull<TCollection, TElement>( this ValueTask<TCollection> values )
        where TCollection : IEnumerable<TElement?>
    {
        IEnumerable<TElement?> results = await values.ConfigureAwait(false);
        return results.WhereNotNull();
    }


    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TElement>( this Task<IEnumerable<TElement?>> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values.ConfigureAwait(false) )
        {
            if ( element is not null ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TElement>( this ValueTask<IEnumerable<TElement?>> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values.ConfigureAwait(false) )
        {
            if ( element is not null ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TCollection, TElement>( this Task<TCollection> values )
        where TCollection : IEnumerable<TElement?>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values.ConfigureAwait(false) )
        {
            if ( element is not null ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TCollection, TElement>( this ValueTask<TCollection> values )
        where TCollection : IEnumerable<TElement?>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values.ConfigureAwait(false) )
        {
            if ( element is not null ) { yield return element; }
        }
    }



    extension<TElement>( IAsyncEnumerable<TElement> source )
    {
        public async ValueTask<bool> All( Func<TElement, bool> selector, CancellationToken token = default )
        {
            await foreach ( TElement element in source.WithCancellation(token).ConfigureAwait(false) )
            {
                if ( !selector(element) ) { return false; }
            }

            return true;
        }
        public async ValueTask<bool> All( Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
        {
            await foreach ( TElement element in source.WithCancellation(token).ConfigureAwait(false) )
            {
                if ( !await selector(element).ConfigureAwait(false) ) { return false; }
            }

            return true;
        }
        public async ValueTask<bool> All( Func<TElement, Task<bool>> selector, CancellationToken token = default )
        {
            await foreach ( TElement element in source.WithCancellation(token).ConfigureAwait(false) )
            {
                if ( !await selector(element).ConfigureAwait(false) ) { return false; }
            }

            return true;
        }
        public async ValueTask<bool> Any( CancellationToken token = default )
        {
            await foreach ( TElement _ in source.WithCancellation(token).ConfigureAwait(false) ) { return true; }

            return false;
        }
        public async ValueTask<bool> Any( Func<TElement, bool> selector, CancellationToken token = default )
        {
            await foreach ( TElement element in source.WithCancellation(token).ConfigureAwait(false) )
            {
                if ( selector(element) ) { return true; }
            }

            return false;
        }
        public async ValueTask<bool> Any( Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
        {
            await foreach ( TElement element in source.WithCancellation(token).ConfigureAwait(false) )
            {
                if ( await selector(element).ConfigureAwait(false) ) { return true; }
            }

            return false;
        }
        public async ValueTask<bool> Any( Func<TElement, Task<bool>> selector, CancellationToken token = default )
        {
            await foreach ( TElement element in source.WithCancellation(token).ConfigureAwait(false) )
            {
                if ( await selector(element).ConfigureAwait(false) ) { return true; }
            }

            return false;
        }
    }
}
