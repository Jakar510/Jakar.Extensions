// Jakar.Extensions :: Jakar.Extensions
// 09/11/2022  3:00 PM

#pragma warning disable CS8424
namespace Jakar.Extensions;


public static partial class AsyncLinq
{
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
                                                      .WithCancellation(token)
                                                      .ConfigureAwait(false) ) { results.Add(element); }

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
                                                      .WithCancellation(token)
                                                      .ConfigureAwait(false) ) { results.Add(element); }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach ( TElement element in results )
            {
                if ( token.IsCancellationRequested ) { yield break; }

                yield return element;
            }
        }
    }



    public static async IAsyncEnumerable<TElement> CastSafe<TElement>( this IAsyncEnumerable<object> source )
    {
        await foreach ( object element in source.ConfigureAwait(false) )
        {
            if ( element is TElement value ) { yield return value; }
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
}
