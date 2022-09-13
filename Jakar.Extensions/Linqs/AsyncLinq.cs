#pragma warning disable CS8424



namespace Jakar.Extensions;


/// <summary>
/// <para><see cref="Enumerable"/></para>
/// <para>AsyncEnumerable</para>
/// </summary>
public static partial class AsyncLinq
{
    public static async IAsyncEnumerable<TElement> WhereNotNull<TElement>( this IAsyncEnumerable<TElement?> source, [EnumeratorCancellation] CancellationToken token = default )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        await foreach ( TElement? element in source.WithCancellation(token) )
        {
            if ( element is not null ) { yield return element; }
        }
    }


    public static async Task ForEachAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, Task> action, bool continueOnCapturedContext = true, [EnumeratorCancellation] CancellationToken token = default )
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

        foreach ( TElement element in results )
        {
            token.ThrowIfCancellationRequested();
            yield return element;
        }
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
    public static async IAsyncEnumerable<(long Index, TElement Value)> EnumerateLong<TElement>( this IAsyncEnumerable<TElement> source, [EnumeratorCancellation] CancellationToken token = default )
    {
        long index = 0;

        await foreach ( TElement element in source.WithCancellation(token) )
        {
            checked { index++; }

            yield return ( index, element );
        }
    }
}
