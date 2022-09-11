#pragma warning disable CS8424



namespace Jakar.Extensions;


/// <summary>
/// <para><see cref="Enumerable"/></para>
/// <para><see cref="AsyncEnumerable"/></para>
/// </summary>
public static class AsyncLinq
{
    public static async IAsyncEnumerable<TElement> WhereNotNull<TElement>( this IAsyncEnumerable<TElement?> source, [EnumeratorCancellation] CancellationToken token = default )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        await foreach ( TElement? element in source.WithCancellation(token) )
        {
            if ( element is not null ) { yield return element; }
        }
    }


    public static async ValueTask ForEachAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, Task> action, bool continueOnCapturedContext = true, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement item in source.WithCancellation(token) )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }
    public static async ValueTask ForEachAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask> action, bool continueOnCapturedContext = true, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement item in source.WithCancellation(token) )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }


    public static async ValueTask<List<TElement>> ToList<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        var list = new List<TElement>();
        await foreach ( TElement element in source.WithCancellation(token) ) { list.Add(element); }

        return list;
    }
    public static ValueTask<HashSet<TElement>> ToHashSet<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default ) => source.ToHashSet(EqualityComparer<TElement>.Default, token);
    public static async ValueTask<HashSet<TElement>> ToHashSet<TElement>( this IAsyncEnumerable<TElement> source, IEqualityComparer<TElement> comparer, CancellationToken token = default )
    {
        var list = new HashSet<TElement>(comparer);
        await foreach ( TElement element in source.WithCancellation(token) ) { list.Add(element); }

        return list;
    }
    public static async ValueTask<ObservableCollection<TElement>> ToObservableCollection<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        var list = new ObservableCollection<TElement>();
        await foreach ( TElement element in source.WithCancellation(token) ) { list.Add(element); }

        return list;
    }


    public static async IAsyncEnumerable<TElement> ConsolidateUnique<TElement>( this IAsyncEnumerable<IAsyncEnumerable<TElement>> items, [EnumeratorCancellation] CancellationToken token = default )
    {
        var results = new HashSet<TElement>();
        await foreach ( TElement element in items.Consolidate(token) ) { results.Add(element); }

        foreach ( TElement element in results ) { yield return element; }
    }
    public static async IAsyncEnumerable<TElement> Consolidate<TElement>( this IAsyncEnumerable<IAsyncEnumerable<TElement>> source, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( IAsyncEnumerable<TElement> element in source.WithCancellation(token) )
        {
            await foreach ( TElement item in element.WithCancellation(token) ) { yield return item; }
        }
    }


    public static async IAsyncEnumerable<(int Index, TElement Value)> Enumerate<TElement>( this IAsyncEnumerable<TElement> source, [EnumeratorCancellation] CancellationToken token = default )
    {
        var index = 0;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            checked { index++; }

            yield return ( index, element );
        }
    }


    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        await foreach ( TElement _ in source.WithCancellation(token) ) { return true; }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( selector(element) ) { return true; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( await selector(element) ) { return true; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }


    public static async ValueTask<bool> All<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( !selector(element) ) { return false; }
        }

        return true;
    }
    public static async ValueTask<bool> All<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( !await selector(element) ) { return false; }
        }

        return true;
    }


    public static async ValueTask<TElement> First<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) ) { return element; }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<TElement> First<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( selector(element) ) { return element; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<TElement> First<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( await selector(element) ) { return element; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }


    public static async ValueTask<TElement?> FirstOrDefault<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) ) { return element; }

        return default;
    }
    public static async ValueTask<TElement?> FirstOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( selector(element) ) { return element; }
        }

        return default;
    }
    public static async ValueTask<TElement?> FirstOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( await selector(element) ) { return element; }
        }

        return default;
    }


    public static async ValueTask<TElement> Single<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

            result = element;
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<TElement> Single<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

            if ( selector(element) ) { result = element; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<TElement> Single<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( result is not null ) { throw new InvalidOperationException($"Multiple records in {nameof(source)}"); }

            if ( await selector(element) ) { result = element; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }


    public static async ValueTask<TElement?> SingleOrDefault<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( result is not null ) { return default; }

            result = element;
        }

        return default;
    }
    public static async ValueTask<TElement?> SingleOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( result is not null ) { return default; }

            if ( selector(element) ) { result = element; }
        }

        return default;
    }
    public static async ValueTask<TElement?> SingleOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( result is not null ) { return default; }

            if ( await selector(element) ) { result = element; }
        }

        return default;
    }


    public static async ValueTask<TElement> Last<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);
        return list.Last();
    }
    public static async ValueTask<TElement> Last<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);
        return list.Last(selector);
    }
    public static async ValueTask<TElement> Last<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);

        for ( int i = list.Count; i < 0; i-- )
        {
            if ( await selector(list[i]) ) { return list[i]; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }


    public static async ValueTask<TElement?> LastOrDefault<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);
        return list.LastOrDefault();
    }
    public static async ValueTask<TElement?> LastOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);
        return list.LastOrDefault(selector);
    }
    public static async ValueTask<TElement?> LastOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);

        for ( int i = list.Count; i < 0; i-- )
        {
            if ( await selector(list[i]) ) { return list[i]; }
        }

        return default;
    }


    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( await predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        int index = -1;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            checked { index++; }

            if ( predicate(element, index) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<bool>> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        int index = -1;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            checked { index++; }

            if ( await predicate(element, index) ) { yield return element; }
        }
    }


    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, TResult> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) ) { yield return selector(element); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TResult>> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) ) { yield return await selector(element); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, int, TResult> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) ) { yield return selector(value, index); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<TResult>> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) ) { yield return await selector(value, index); }
    }


    public static async IAsyncEnumerable<TElement> Skip<TElement>( this IAsyncEnumerable<TElement> source, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) )
        {
            if ( index >= count ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( await predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) )
        {
            if ( predicate(value, index) ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<bool>> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) )
        {
            if ( await predicate(value, index) ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipLast<TElement>( this IAsyncEnumerable<TElement> source, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);

        for ( var index = 0; index < list.Count; index++ )
        {
            if ( index >= count ) { yield break; }

            yield return list[index];
        }
    }


    public static IAsyncEnumerable<TElement> Distinct<TElement>( this IAsyncEnumerable<TElement> source, [EnumeratorCancellation] CancellationToken token = default ) => source.Distinct(EqualityComparer<TElement>.Default, token);
    public static async IAsyncEnumerable<TElement> Distinct<TElement>( this IAsyncEnumerable<TElement> source, IEqualityComparer<TElement> comparer, [EnumeratorCancellation] CancellationToken token = default )
    {
        HashSet<TElement> set = await source.ToHashSet(comparer, token);

        foreach ( TElement element in set )
        {
            token.ThrowIfCancellationRequested();
            yield return element;
        }
    }
    public static IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector, [EnumeratorCancellation] CancellationToken token = default ) =>
        source.DistinctBy(keySelector, EqualityComparer<TKey>.Default, token);
    public static IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TKey>> keySelector, [EnumeratorCancellation] CancellationToken token = default ) =>
        source.DistinctBy(keySelector, EqualityComparer<TKey>.Default, token);
    public static async IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken token = default )
    {
        var set = new HashSet<TKey>(comparer);

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( set.Add(keySelector(element)) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement>            source,
                                                                               Func<TElement, ValueTask<TKey>>            keySelector,
                                                                               IEqualityComparer<TKey>                    comparer,
                                                                               [EnumeratorCancellation] CancellationToken token = default
    )
    {
        var set = new HashSet<TKey>(comparer);

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( set.Add(await keySelector(element)
                            .ConfigureAwait(false)) ) { yield return element; }
        }
    }
}
