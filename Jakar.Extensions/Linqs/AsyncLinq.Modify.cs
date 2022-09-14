namespace Jakar.Extensions;
#nullable enable



public static partial class AsyncLinq
{
    public static void Add<TElement>( this ICollection<TElement> list, params TElement[] items )
    {
        foreach ( TElement value in items ) { list.Add(value); }
    }
    public static async ValueTask Add<TElement>( this ICollection<TElement> list, IAsyncEnumerable<TElement> items, CancellationToken token = default )
    {
        await foreach ( TElement value in items.WithCancellation(token) ) { list.Add(value); }
    }
    public static void Add<TElement>( this ICollection<TElement> list, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { list.Add(value); }
    }


    public static void Remove<TElement>( this ICollection<TElement> list, params TElement[] items )
    {
        foreach ( TElement value in items ) { list.Remove(value); }
    }
    public static async ValueTask Remove<TElement>( this ICollection<TElement> list, IAsyncEnumerable<TElement> items, CancellationToken token = default )
    {
        await foreach ( TElement value in items.WithCancellation(token) ) { list.Remove(value); }
    }
    public static void Remove<TElement>( this ICollection<TElement> list, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { list.Remove(value); }
    }




    public static void AddOrUpdate<TElement>( this IList<TElement> list, params TElement[] value )
    {
        foreach ( TElement element in value ) { list.AddOrUpdate(element); }
    }
    public static async ValueTask AddOrUpdate<TElement>( this IList<TElement> list, IAsyncEnumerable<TElement> value, CancellationToken token = default )
    {
        await foreach ( TElement element in value.WithCancellation(token) ) { list.AddOrUpdate(element); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> list, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { list.AddOrUpdate(element); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> list, TElement value )
    {
        int index = list.IndexOf(value);

        if ( index >= 0 ) { list[index] = value; }
        else { list.Add(value); }
    }


    public static void TryAdd<TElement>( this ICollection<TElement> list, params TElement[] value )
    {
        foreach ( TElement element in value ) { list.TryAdd(element); }
    }
    public static async ValueTask TryAdd<TElement>( this ICollection<TElement> list, IAsyncEnumerable<TElement> value, CancellationToken token = default )
    {
        await foreach ( TElement element in value.WithCancellation(token) ) { list.TryAdd(element); }
    }
    public static void TryAdd<TElement>( this ICollection<TElement> list, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { list.TryAdd(element); }
    }
    public static void TryAdd<TElement>( this ICollection<TElement> list, TElement value )
    {
        if ( list.Contains(value) ) { return; }

        list.Add(value);
    }


    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, IEnumerable<TKey> keys )
    {
        foreach ( TKey value in keys ) { dict.AddDefault(value); }
    }
    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, TKey key ) => dict.Add(key, default);
}
