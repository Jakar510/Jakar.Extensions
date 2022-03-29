using System.Threading.Tasks.Dataflow;


namespace Jakar.Extensions.Collections;


public static class EnumerableExtensions
{
    public static bool IsEmpty( this ICollection collection ) => collection.Count == 0;


    public static IEnumerable<TItem> ConsolidateUnique<TItem>( this IEnumerable<IEnumerable<TItem>> items )
    {
        var results = new List<TItem>();

        foreach ( IEnumerable<TItem> enumerable in items )
        {
            foreach ( TItem item in enumerable )
            {
                if ( results.Contains(item) ) { continue; }

                results.Add(item);
            }
        }


        return results;
    }

    public static IEnumerable<TItem> Consolidate<TItem>( this IEnumerable<IEnumerable<TItem>> items )
    {
        var results = new List<TItem>();

        foreach ( IEnumerable<TItem> enumerable in items ) { results.AddRange(enumerable); }

        return results;
    }


    public static void Add<TItem>( this ICollection<TItem> list, IEnumerable<TItem> items ) => items.ForEach(list.Add);
    public static void Add<TItem>( this ICollection<TItem> list, params TItem[]     items ) => items.ForEach(list.Add);


    public static void Remove<TItem>( this ICollection<TItem> list, IEnumerable<TItem> items ) => items.ForEach(item => list.Remove(item));
    public static void Remove<TItem>( this ICollection<TItem> list, params TItem[]     items ) => items.ForEach(item => list.Remove(item));


    public static void ForEach<TItem>( this IEnumerable<TItem> list, Action<TItem> action )
    {
        foreach ( TItem item in list ) { action(item); }
    }

    public static void ForEachParallel<TItem>( this IEnumerable<TItem> list, Action<TItem> action ) => list.AsParallel().ForAll(action);

    public static async Task ForEachAsync<TItem>( this IEnumerable<TItem> list, Func<TItem, Task> action, bool continueOnCapturedContext = true )
    {
        foreach ( TItem item in list ) { await action(item).ConfigureAwait(continueOnCapturedContext); }
    }


#region Python style Enumerate for collection

    public static IEnumerable<(int index, object? item)> Enumerate( this IEnumerable enumerable, int start = 0 )
    {
        int index = start;

        foreach ( object? item in enumerable )
        {
            yield return ( index, item );
            index++;
        }
    }

    public static IEnumerable<(int index, TItem item)> Enumerate<TItem>( this IEnumerable<TItem> enumerable, int start = 0 )
    {
        int index = start;

        foreach ( TItem item in enumerable )
        {
            yield return ( index, item );
            index++;
        }
    }

    public static IEnumerable<(int index, object key, object value)> Enumerate( this IDictionary enumerable, int start = 0 )
    {
        int index = start;

        foreach ( DictionaryEntry pair in enumerable )
        {
            yield return ( index, pair.Key, pair.Value );
            index++;
        }
    }

    public static IEnumerable<(int index, KeyValuePair<TKey, TValue> pair)> EnumeratePairs<TKey, TValue>( this IDictionary<TKey, TValue> enumerable, int start = 0 )
    {
        int index = start;

        foreach ( KeyValuePair<TKey, TValue> pair in enumerable )
        {
            yield return ( index, pair );
            index++;
        }
    }

    public static IEnumerable<(int index, TKey key, TValue value)> Enumerate<TKey, TValue>( this IDictionary<TKey, TValue> enumerable, int start = 0 )
    {
        int index = start;

        foreach ( ( TKey key, TValue value ) in enumerable )
        {
            yield return ( index, key, value );
            index++;
        }
    }

#endregion


#region Random Items in collection

    public static IEnumerator<TValue> Random<TValue>( this IReadOnlyIndexable<TValue> items, Random? rand = default )
    {
        if ( items is null ) throw new ArgumentNullException(nameof(items));

        rand ??= new Random();

        while ( true ) { yield return items[rand.Next(items.Count)]; }
    }

    public static IEnumerator<TValue> Random<TValue>( this IIndexable<TValue> items, Random? rand = default )
    {
        if ( items is null ) throw new ArgumentNullException(nameof(items));

        rand ??= new Random();

        while ( true ) { yield return items[rand.Next(items.Count)]; }
    }

    public static IEnumerator<TValue> Random<TValue>( this IList<TValue> items, Random? rand = default )
    {
        if ( items is null ) throw new ArgumentNullException(nameof(items));

        rand ??= new Random();

        while ( true ) { yield return items[rand.Next(items.Count)]; }
    }

    public static IEnumerator<TValue> Random<TValue>( this IReadOnlyList<TValue> items, Random? rand = default )
    {
        if ( items is null ) throw new ArgumentNullException(nameof(items));

        rand ??= new Random();

        while ( true ) { yield return items[rand.Next(items.Count)]; }
    }


    public static IEnumerator<TKey> RandomKeys<TKey, TValue>( this IDictionary<TKey, TValue> dict, Random? rand = default )
    {
        if ( dict is null ) throw new ArgumentNullException(nameof(dict));

        rand ??= new Random();

        List<TKey> items = dict.Keys.ToList();

        while ( true ) { yield return items[rand.Next(items.Count)]; }
    }

    public static IEnumerator<TValue> RandomValues<TKey, TValue>( this IDictionary<TKey, TValue> dict, Random? rand = default )
    {
        if ( dict is null ) throw new ArgumentNullException(nameof(dict));

        rand ??= new Random();

        List<TValue> items = dict.Values.ToList();

        while ( true ) { yield return items[rand.Next(items.Count)]; }
    }

#endregion
}
