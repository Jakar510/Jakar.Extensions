namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask Add<TElement>( this ConcurrentBag<TElement> collection, IAsyncEnumerable<TElement> values, CancellationToken token = default )
    {
        await foreach ( TElement value in values.WithCancellation( token ) ) { collection.Add( value ); }
    }
    public static async ValueTask Add<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> values, CancellationToken token = default )
        where TElement : IEquatable<TElement>
    {
        if ( collection is ConcurrentObservableCollection<TElement> list )
        {
            await list.AddAsync( values, token );
            return;
        }

        await foreach ( TElement value in values.WithCancellation( token ) ) { collection.Add( value ); }
    }
    public static async ValueTask AddOrUpdate<TElement>( this IList<TElement> collection, IAsyncEnumerable<TElement> values, CancellationToken token = default )
        where TElement : IEquatable<TElement>
    {
        if ( collection is ConcurrentObservableCollection<TElement> list )
        {
            await list.AddOrUpdate( values, token );
            return;
        }

        await foreach ( TElement value in values.WithCancellation( token ) ) { collection.AddOrUpdate( value ); }
    }
    public static async ValueTask Remove<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> values, CancellationToken token = default )
        where TElement : IEquatable<TElement>
    {
        if ( collection is ConcurrentObservableCollection<TElement> list )
        {
            await list.RemoveAsync( values, token );
            return;
        }

        await foreach ( TElement value in values.WithCancellation( token ) ) { collection.Remove( value ); }
    }
    public static async ValueTask TryAdd<TElement>( this ICollection<TElement> collection, IAsyncEnumerable<TElement> values, CancellationToken token = default )
        where TElement : IEquatable<TElement>
    {
        if ( collection is ConcurrentObservableCollection<TElement> list )
        {
            await list.TryAddAsync( values, token );
            return;
        }

        await foreach ( TElement value in values.WithCancellation( token ) ) { collection.TryAdd( value ); }
    }
    public static void Add<TElement>( this ConcurrentBag<TElement> collection, params ReadOnlySpan<TElement> values )
    {
        foreach ( TElement value in values ) { collection.Add( value ); }
    }
    public static void Add<TElement>( this ConcurrentBag<TElement> collection, IEnumerable<TElement> values )
    {
        foreach ( TElement value in values ) { collection.Add( value ); }
    }


    public static void Add<TElement>( this ICollection<TElement> collection, params ReadOnlySpan<TElement> values )
    {
        foreach ( TElement value in values ) { collection.Add( value ); }
    }
    public static void Add<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> values )
    {
        foreach ( TElement value in values ) { collection.Add( value ); }
    }


    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, IEnumerable<TKey> keys )
    {
        foreach ( TKey value in keys ) { dict.AddDefault( value ); }
    }
    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, params ReadOnlySpan<TKey> keys )
    {
        foreach ( TKey value in keys ) { dict.AddDefault( value ); }
    }
    public static void AddDefault<TKey, TElement>( this IDictionary<TKey, TElement?> dict, TKey key ) => dict.Add( key, default );


    public static void AddOrUpdate<TElement>( this IList<TElement> collection, params ReadOnlySpan<TElement> values )
    {
        foreach ( TElement value in values ) { collection.AddOrUpdate( value ); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, IEnumerable<TElement> values )
    {
        foreach ( TElement value in values ) { collection.AddOrUpdate( value ); }
    }
    public static void AddOrUpdate<TElement>( this IList<TElement> collection, TElement value )
    {
        int index = collection.IndexOf( value );

        if ( index >= 0 ) { collection[index] = value; }
        else { collection.Add( value ); }
    }


    public static void Remove<TElement>( this ICollection<TElement> collection, params ReadOnlySpan<TElement> values )
    {
        foreach ( TElement value in values ) { collection.Remove( value ); }
    }
    public static void Remove<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> values )
    {
        foreach ( TElement value in values ) { collection.Remove( value ); }
    }


    public static void TryAdd<TElement>( this ICollection<TElement> collection, params ReadOnlySpan<TElement> values )
    {
        foreach ( TElement value in values ) { collection.TryAdd( value ); }
    }
    public static void TryAdd<TElement>( this ICollection<TElement> collection, IEnumerable<TElement> values )
    {
        foreach ( TElement value in values ) { collection.TryAdd( value ); }
    }
    public static void TryAdd<TElement>( this ICollection<TElement> collection, TElement value )
    {
        if ( collection.Contains( value ) ) { return; }

        collection.Add( value );
    }
}
