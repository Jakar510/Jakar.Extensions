namespace Jakar.Extensions;
#nullable enable



public static partial class AsyncLinq
{
    public static void Add<TElement>( this ConcurrentBag<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.Add(value); }
    }
    public static async ValueTask Add<TElement>( this ConcurrentBag<TElement> collection, IAsyncEnumerable<TElement> items, CancellationToken token = default )
    {
        await foreach ( TElement value in items.WithCancellation(token) ) { collection.Add(value); }
    }
    public static void Add<TElement>( this ConcurrentBag<TElement> collection, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Add(value); }
    }


    public static void Add<TElement>( this ICollection<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.Add(value); }
    }
    public static async ValueTask Add<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> items, CancellationToken token = default )
    {
        await foreach ( TElement value in items.WithCancellation(token) ) { collection.Add(value); }
    }
    public static void Add<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Add(value); }
    }


    public static void Remove<TElement>( this ICollection<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.Remove(value); }
    }
    public static async ValueTask Remove<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> items, CancellationToken token = default )
    {
        await foreach ( TElement value in items.WithCancellation(token) ) { collection.Remove(value); }
    }
    public static void Remove<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Remove(value); }
    }


    public static void AddOrUpdate<TElement>( this IList<TElement> collection, params TElement[] value )
    {
        foreach ( TElement element in value ) { collection.AddOrUpdate(element); }
    }
    public static async ValueTask AddOrUpdate<TElement>( this IList<TElement> collection, IAsyncEnumerable<TElement> value, CancellationToken token = default )
    {
        await foreach ( TElement element in value.WithCancellation(token) ) { collection.AddOrUpdate(element); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { collection.AddOrUpdate(element); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, TElement value )
    {
        int index = collection.IndexOf(value);

        if ( index >= 0 ) { collection[index] = value; }
        else { collection.Add(value); }
    }


    public static void TryAdd<TElement>( this ICollection<TElement> collection, params TElement[] value )
    {
        foreach ( TElement element in value ) { collection.TryAdd(element); }
    }
    public static async ValueTask TryAdd<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> value, CancellationToken token = default )
    {
        await foreach ( TElement element in value.WithCancellation(token) ) { collection.TryAdd(element); }
    }
    public static void TryAdd<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { collection.TryAdd(element); }
    }
    public static void TryAdd<TElement>( this ICollection<TElement> collection, TElement value )
    {
        if ( collection.Contains(value) ) { return; }

        collection.Add(value);
    }


    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, IEnumerable<TKey> keys )
    {
        foreach ( TKey value in keys ) { dict.AddDefault(value); }
    }
    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, TKey key ) => dict.Add(key, default);
}
