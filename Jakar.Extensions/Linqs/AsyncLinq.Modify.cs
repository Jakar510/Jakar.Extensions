namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask AddOrUpdate<TElement>( this IList<TElement> collection, IAsyncEnumerable<TElement> values, CancellationToken token = default )
        where TElement : IEquatable<TElement>
    {
        if ( collection is ConcurrentObservableCollection<TElement> list )
        {
            await list.AddOrUpdate(values, token).ConfigureAwait(false);
            return;
        }

        await foreach ( TElement value in values.WithCancellation(token).ConfigureAwait(false) ) { collection.AddOrUpdate(value); }
    }



    extension<TElement>( ICollection<TElement> collection )
        where TElement : IEquatable<TElement>
    {
        public async ValueTask Remove( IAsyncEnumerable<TElement> values, CancellationToken token = default )
        {
            if ( collection is ConcurrentObservableCollection<TElement> list )
            {
                await list.RemoveAsync(values, token).ConfigureAwait(false);
                return;
            }

            await foreach ( TElement value in values.WithCancellation(token).ConfigureAwait(false) ) { collection.Remove(value); }
        }
        public async ValueTask TryAdd( IAsyncEnumerable<TElement> values, CancellationToken token = default )
        {
            if ( collection is ConcurrentObservableCollection<TElement> list )
            {
                await list.TryAddAsync(values, token).ConfigureAwait(false);
                return;
            }

            await foreach ( TElement value in values.WithCancellation(token).ConfigureAwait(false) ) { collection.TryAdd(value); }
        }
        public async ValueTask Add( IAsyncEnumerable<TElement> values, CancellationToken token = default )
        {
            switch ( collection )
            {
                case ConcurrentObservableCollection<TElement> list:
                    await list.AddAsync(values, token).ConfigureAwait(false);
                    return;

                case ObservableCollection<TElement> list:
                    await list.AddAsync(values, token).ConfigureAwait(false);
                    return;
            }

            await foreach ( TElement value in values.WithCancellation(token).ConfigureAwait(false) ) { collection.Add(value); }
        }
    }



    extension<TElement>( ConcurrentBag<TElement> collection )
    {
        public void Add( params ReadOnlySpan<TElement> values )
        {
            foreach ( TElement value in values ) { collection.Add(value); }
        }
        public void Add( IEnumerable<TElement> values )
        {
            foreach ( TElement value in values ) { collection.Add(value); }
        }
        public async ValueTask Add( IAsyncEnumerable<TElement> values, CancellationToken token = default )
        {
            await foreach ( TElement value in values.WithCancellation(token).ConfigureAwait(false) ) { collection.Add(value); }
        }
    }



    extension<TElement>( ICollection<TElement> collection )
    {
        public void Add( params ReadOnlySpan<TElement> values )
        {
            foreach ( TElement value in values ) { collection.Add(value); }
        }
        public void Add( IEnumerable<TElement> values )
        {
            foreach ( TElement value in values ) { collection.Add(value); }
        }
    }



    extension<TKey, TElement>( IDictionary<TKey, TElement?> dict )
    {
        public void AddDefault( IEnumerable<TKey> keys )
        {
            foreach ( TKey value in keys ) { dict.AddDefault(value); }
        }
        public void AddDefault( params ReadOnlySpan<TKey> keys )
        {
            foreach ( TKey value in keys ) { dict.AddDefault(value); }
        }
        public void AddDefault( TKey key ) => dict.Add(key, default);
    }



    extension<TElement>( IList<TElement> collection )
    {
        public void AddOrUpdate( params ReadOnlySpan<TElement> values )
        {
            foreach ( TElement value in values ) { collection.AddOrUpdate(value); }
        }
        public void AddOrUpdate( IEnumerable<TElement> values )
        {
            foreach ( TElement value in values ) { collection.AddOrUpdate(value); }
        }
        public void AddOrUpdate( TElement value )
        {
            int index = collection.IndexOf(value);

            if ( index >= 0 ) { collection[index] = value; }
            else { collection.Add(value); }
        }
    }



    extension<TElement>( ICollection<TElement> collection )
    {
        public void Remove( params ReadOnlySpan<TElement> values )
        {
            foreach ( TElement value in values ) { collection.Remove(value); }
        }
        public void Remove( IEnumerable<TElement> values )
        {
            foreach ( TElement value in values ) { collection.Remove(value); }
        }
        public void TryAdd( params ReadOnlySpan<TElement> values )
        {
            foreach ( TElement value in values ) { collection.TryAdd(value); }
        }
        public void TryAdd( IEnumerable<TElement> values )
        {
            foreach ( TElement value in values ) { collection.TryAdd(value); }
        }
        public void TryAdd( TElement value )
        {
            if ( collection.Contains(value) ) { return; }

            collection.Add(value);
        }
    }
}
