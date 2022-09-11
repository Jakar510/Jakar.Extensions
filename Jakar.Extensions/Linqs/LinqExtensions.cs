#nullable enable
namespace Jakar.Extensions;


public static class LinqExtensions
{
    public static bool IsEmpty( this ICollection collection ) => collection.Count == 0;


    public static IEnumerable<TElement> WhereNotNull<TElement>( this IEnumerable<TElement?> items )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in items )
        {
            if ( element is not null ) { yield return element; }
        }
    }


    public static IEnumerable<TElement> ConsolidateUnique<TElement>( this IEnumerable<IEnumerable<TElement>> items )
    {
        var results = new HashSet<TElement>();

        foreach ( IEnumerable<TElement> element in items )
        {
            foreach ( TElement item in element ) { results.Add(item); }
        }


        return results;
    }
    public static IEnumerable<TElement> Consolidate<TElement>( this IEnumerable<IEnumerable<TElement>> items )
    {
        var results = new List<TElement>();
        foreach ( IEnumerable<TElement> element in items ) { results.AddRange(element); }

        return results;
    }


    public static void Add<TElement>( this ICollection<TElement> list, IEnumerable<TElement> items ) => items.ForEach(list.Add);
    public static void Add<TElement>( this ICollection<TElement> list, params TElement[]     items ) => items.ForEach(list.Add);


    public static void Remove<TElement>( this ICollection<TElement> list, IEnumerable<TElement> items ) => items.ForEach(item => list.Remove(item));
    public static void Remove<TElement>( this ICollection<TElement> list, params TElement[]     items ) => items.ForEach(item => list.Remove(item));


    public static void ForEach<TElement>( this IEnumerable<TElement> list, Action<TElement> action )
    {
        foreach ( TElement item in list ) { action(item); }
    }
    public static void ForEachParallel<TElement>( this IEnumerable<TElement> list, Action<TElement> action ) => list.AsParallel()
                                                                                                                    .ForAll(action);


    public static async Task ForEachAsync<TElement>( this IEnumerable<TElement> list, Func<TElement, Task> action, bool continueOnCapturedContext = true )
    {
        foreach ( TElement item in list )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }
    public static async ValueTask ForEachAsync<TElement>( this IEnumerable<TElement> list, Func<TElement, ValueTask> action, bool continueOnCapturedContext = true )
    {
        foreach ( TElement item in list )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }


    public static void AddOrUpdate<TElement>( this IList<TElement> list, TElement value )
    {
        int index = list.IndexOf(value);

        if ( index >= 0 ) { list[index] = value; }
        else { list.Add(value); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> list, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { list.AddOrUpdate(element); }
    }
    public static async ValueTask AddOrUpdate<TElement>( this IList<TElement> list, IAsyncEnumerable<TElement> value, CancellationToken token = default )
    {
        await foreach ( TElement element in value.WithCancellation(token) ) { list.AddOrUpdate(element); }
    }


    public static void TryAdd<TElement>( this ICollection<TElement> list, TElement value )
    {
        if ( list.Contains(value) ) { return; }

        list.Add(value);
    }
    public static void TryAdd<TElement>( this ICollection<TElement> list, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { list.TryAdd(element); }
    }
    public static async ValueTask TryAdd<TElement>( this ICollection<TElement> list, IAsyncEnumerable<TElement> value, CancellationToken token = default )
    {
        await foreach ( TElement element in value.WithCancellation(token) ) { list.TryAdd(element); }
    }


    public static IEnumerable<(int index, object? item)> Enumerate( this IEnumerable element, int start = 0 )
    {
        int index = start;

        foreach ( object? item in element )
        {
            yield return ( index, item );
            index++;
        }
    }
    public static IEnumerable<(int index, TElement item)> Enumerate<TElement>( this IEnumerable<TElement> element, int start = 0 )
    {
        int index = start;

        foreach ( TElement item in element )
        {
            yield return ( index, item );
            index++;
        }
    }

    public static IEnumerable<(int index, object key, object? value)> Enumerate( this IDictionary element, int start = 0 )
    {
        int index = start;

        foreach ( DictionaryEntry pair in element )
        {
            yield return ( index, pair.Key, pair.Value );
            index++;
        }
    }
    public static IEnumerable<(int index, KeyValuePair<TKey, TValue> pair)> EnumeratePairs<TKey, TValue>( this IDictionary<TKey, TValue> element, int start = 0 )
    {
        int index = start;

        foreach ( KeyValuePair<TKey, TValue> pair in element )
        {
            yield return ( index, pair );
            index++;
        }
    }

    public static IEnumerable<(int index, TKey key, TValue value)> Enumerate<TKey, TValue>( this IDictionary<TKey, TValue> element, int start = 0 )
    {
        int index = start;

        foreach ( ( TKey key, TValue value ) in element )
        {
            yield return ( index, key, value );
            index++;
        }
    }


    public static IEnumerator<TValue> Random<TValue>( this IReadOnlyIndexable<TValue> items, Random rand, CancellationToken token = default )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        while ( !token.IsCancellationRequested ) { yield return items[rand.Next(items.Count)]; }
    }
    public static IEnumerator<TValue> Random<TValue>( this IIndexable<TValue> items, Random rand, CancellationToken token = default )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        while ( !token.IsCancellationRequested ) { yield return items[rand.Next(items.Count)]; }
    }
    public static IEnumerator<TValue> Random<TValue>( this IReadOnlyList<TValue> items, Random rand, CancellationToken token = default )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        while ( !token.IsCancellationRequested ) { yield return items[rand.Next(items.Count)]; }
    }


    public static IEnumerator<TKey> RandomKeys<TKey, TValue>( this IDictionary<TKey, TValue> dict, Random rand, CancellationToken token = default )
    {
        if ( dict is null ) { throw new ArgumentNullException(nameof(dict)); }

        List<TKey> items = dict.Keys.ToList();

        while ( !token.IsCancellationRequested )
        {
            items.TryAdd(dict.Keys);
            yield return items[rand.Next(items.Count)];
        }
    }
    public static IEnumerator<TValue> RandomValues<TKey, TValue>( this IDictionary<TKey, TValue> dict, Random rand, CancellationToken token = default )
    {
        if ( dict is null ) { throw new ArgumentNullException(nameof(dict)); }

        List<TValue> items = dict.Values.ToList();

        while ( !token.IsCancellationRequested )
        {
            items.TryAdd(dict.Values);
            yield return items[rand.Next(items.Count)];
        }
    }
}
