namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask Add<TElement>( this ConcurrentBag<TElement> collection, IAsyncEnumerable<TElement> items )
    {
        await foreach ( TElement value in items ) { collection.Add( value ); }
    }
    public static async ValueTask Add<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> items )
    {
        await foreach ( TElement value in items ) { collection.Add( value ); }
    }
    public static async ValueTask AddOrUpdate<TElement>( this IList<TElement> collection, IAsyncEnumerable<TElement> value )
    {
        await foreach ( TElement element in value ) { collection.AddOrUpdate( element ); }
    }
    public static async ValueTask Remove<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> items )
    {
        await foreach ( TElement value in items ) { collection.Remove( value ); }
    }
    public static async ValueTask TryAdd<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> value )
    {
        await foreach ( TElement element in value ) { collection.TryAdd( element ); }
    }
#if NETSTANDARD2_1
    public static void Add<TElement>( this ConcurrentBag<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.Add( value ); }
    }
#else
    public static void Add<TElement>( this ConcurrentBag<TElement> collection, in ReadOnlySpan<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Add( value ); }
    }
#endif
    public static void Add<TElement>( this ConcurrentBag<TElement> collection, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Add( value ); }
    }


    public static void Add<TElement>( this ICollection<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.Add( value ); }
    }
    public static void Add<TElement>( this ICollection<TElement> collection, in ReadOnlySpan<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Add( value ); }
    }
    public static void Add<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Add( value ); }
    }


    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, IEnumerable<TKey> keys )
    {
        foreach ( TKey value in keys ) { dict.AddDefault( value ); }
    }
    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, TKey key ) => dict.Add( key, default );


#if NETSTANDARD2_1
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.TryAdd( value ); }
    }
#else
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, in ReadOnlySpan<TElement> items )
    {
        foreach ( TElement value in items ) { collection.AddOrUpdate( value ); }
    }
#endif
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { collection.AddOrUpdate( element ); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, TElement value )
    {
        int index = collection.IndexOf( value );

        if ( index >= 0 ) { collection[index] = value; }
        else { collection.Add( value ); }
    }


#if NETSTANDARD2_1
    public static void Remove<TElement>( this ICollection<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.Remove( value ); }
    }
#else
    public static void Remove<TElement>( this ICollection<TElement> collection, in ReadOnlySpan<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Remove( value ); }
    }
#endif
    public static void Remove<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> items )
    {
        foreach ( TElement value in items ) { collection.Remove( value ); }
    }


#if NETSTANDARD2_1
    public static void TryAdd<TElement>( this ICollection<TElement> collection, params TElement[] items )
    {
        foreach ( TElement value in items ) { collection.TryAdd( value ); }
    }
#else
    public static void TryAdd<TElement>( this ICollection<TElement> collection, in ReadOnlySpan<TElement> items )
    {
        foreach ( TElement value in items ) { collection.TryAdd( value ); }
    }
#endif
    public static void TryAdd<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> value )
    {
        foreach ( TElement element in value ) { collection.TryAdd( element ); }
    }
    public static void TryAdd<TElement>( this ICollection<TElement> collection, TElement value )
    {
        if ( collection.Contains( value ) ) { return; }

        collection.Add( value );
    }
}
