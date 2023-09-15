// Jakar.Extensions :: Jakar.Extensions
// 09/11/2022  3:00 PM

#pragma warning disable CS8424
namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async IAsyncEnumerable<TElement> Append<TElement>( this IAsyncEnumerable<TElement> source, TElement value )
    {
        await foreach ( TElement element in source ) { yield return element; }

        yield return value;
    }
    public static async IAsyncEnumerable<TElement> Append<TElement>( this IAsyncEnumerable<TElement> source, IAsyncEnumerable<TElement> values )
    {
        await foreach ( TElement element in values ) { yield return element; }

        await foreach ( TElement element in source ) { yield return element; }
    }
    public static async IAsyncEnumerable<TElement> Append<TElement>( this IAsyncEnumerable<TElement> source, IEnumerable<TElement> values )
    {
        foreach ( TElement element in values ) { yield return element; }

        await foreach ( TElement element in source ) { yield return element; }
    }


    public static async IAsyncEnumerable<TElement> Prepend<TElement>( this IAsyncEnumerable<TElement> source, TElement value )
    {
        yield return value;
        await foreach ( TElement element in source ) { yield return element; }
    }
    public static async IAsyncEnumerable<TElement> Prepend<TElement>( this IAsyncEnumerable<TElement> source, IEnumerable<TElement> values )
    {
        await foreach ( TElement element in source ) { yield return element; }

        foreach ( TElement element in values ) { yield return element; }
    }
    public static async IAsyncEnumerable<TElement> Prepend<TElement>( this IAsyncEnumerable<TElement> source, IAsyncEnumerable<TElement> values )
    {
        await foreach ( TElement element in source ) { yield return element; }

        await foreach ( TElement element in values ) { yield return element; }
    }


    public static async IAsyncEnumerable<TElement> Cast<TSource, TElement>( this IAsyncEnumerable<TSource> source, Func<TSource, TElement> func )
    {
        await foreach ( TSource element in source ) { yield return func( element ); }
    }
    public static async IAsyncEnumerable<TElement> Cast<TElement>( this IAsyncEnumerable<object> source )
    {
        await foreach ( object element in source ) { yield return (TElement)element; }
    }
    public static async IAsyncEnumerable<TElement> CastSafe<TElement>( this IAsyncEnumerable<object> source )
    {
        await foreach ( object element in source )
        {
            if ( element is TElement value ) { yield return value; }
        }
    }


    public static IAsyncEnumerable<TElement> Distinct<TElement>( this IAsyncEnumerable<TElement> source ) => source.Distinct( EqualityComparer<TElement>.Default );
    public static async IAsyncEnumerable<TElement> Distinct<TElement>( this IAsyncEnumerable<TElement> source, IEqualityComparer<TElement> comparer )
    {
        HashSet<TElement> set = await source.ToHashSet( comparer );
        foreach ( TElement element in set ) { yield return element; }
    }
    public static IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector ) =>
        source.DistinctBy( keySelector, EqualityComparer<TKey>.Default );
    public static IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TKey>> keySelector ) =>
        source.DistinctBy( keySelector, EqualityComparer<TKey>.Default );
    public static async IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector, IEqualityComparer<TKey> comparer )
    {
        var set = new HashSet<TKey>( comparer );

        await foreach ( TElement element in source )
        {
            if ( set.Add( keySelector( element ) ) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TKey>> keySelector, IEqualityComparer<TKey> comparer )
    {
        var set = new HashSet<TKey>( comparer );

        await foreach ( TElement element in source )
        {
            if ( set.Add( await keySelector( element )
                             .ConfigureAwait( false ) ) ) { yield return element; }
        }
    }


    public static async IAsyncEnumerable<TElement> Skip<TElement>( this IAsyncEnumerable<TElement> source, int count, int start = 0 )
    {
        await foreach ( (int index, TElement? value) in source.Enumerate( start ) )
        {
            if ( index >= count ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipLast<TElement>( this IAsyncEnumerable<TElement> source, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        List<TElement> list = await source.ToList( token );

        for ( int index = 0; index < list.Count; index++ )
        {
            if ( token.IsCancellationRequested ) { yield break; }

            if ( index >= count ) { yield break; }

            yield return list[index];
        }
    }


    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( predicate( element ) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( await predicate( element ) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, bool> predicate, int start = 0 )
    {
        await foreach ( (int index, TElement? value) in source.Enumerate( start ) )
        {
            if ( predicate( value, index ) ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<bool>> predicate, int start = 0 )
    {
        await foreach ( (int index, TElement? value) in source.Enumerate( start ) )
        {
            if ( await predicate( value, index ) ) { yield return value; }
        }
    }


    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( predicate( element ) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( await predicate( element ) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, bool> predicate )
    {
        int index = -1;

        await foreach ( TElement element in source )
        {
            checked { index++; }

            if ( predicate( element, index ) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<bool>> predicate )
    {
        int index = -1;

        await foreach ( TElement element in source )
        {
            checked { index++; }

            if ( await predicate( element, index ) ) { yield return element; }
        }
    }


    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, TResult> selector )
    {
        await foreach ( TElement element in source ) { yield return selector( element ); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TResult>> selector )
    {
        await foreach ( TElement element in source ) { yield return await selector( element ); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, int, TResult> selector, int start = 0 )
    {
        await foreach ( (int index, TElement? value) in source.Enumerate( start ) ) { yield return selector( value, index ); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<TResult>> selector, int start = 0 )
    {
        await foreach ( (int index, TElement? value) in source.Enumerate( start ) ) { yield return await selector( value, index ); }
    }


    public static IEnumerable<TElement> Consolidate<TElement>( this IEnumerable<IEnumerable<TElement>> values )
    {
        var results = new List<TElement>();
        foreach ( IEnumerable<TElement> element in values ) { results.AddRange( element ); }

        return results;
    }
    public static IEnumerable<TElement> ConsolidateUnique<TElement>( this IEnumerable<IEnumerable<TElement>> values )
    {
        var results = new HashSet<TElement>();

        foreach ( IEnumerable<TElement> element in values )
        {
            foreach ( TElement item in element ) { results.Add( item ); }
        }

        return results;
    }

    public static async IAsyncEnumerable<TElement> Consolidate<TElement>( this IAsyncEnumerable<IAsyncEnumerable<TElement>> source )
    {
        await foreach ( IAsyncEnumerable<TElement> element in source )
        {
            await foreach ( TElement item in element ) { yield return item; }
        }
    }
    public static async IAsyncEnumerable<TElement> ConsolidateUnique<TElement>( this IAsyncEnumerable<IAsyncEnumerable<TElement>> values, [EnumeratorCancellation] CancellationToken token = default )
    {
        var results = new HashSet<TElement>();

        await foreach ( TElement element in values.Consolidate()
                                                  .WithCancellation( token ) ) { results.Add( element ); }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( TElement element in results )
        {
            if ( token.IsCancellationRequested ) { yield break; }

            yield return element;
        }
    }

    public static async IAsyncEnumerable<TElement> Consolidate<TElement>( this IAsyncEnumerable<IEnumerable<TElement>> source )
    {
        await foreach ( IEnumerable<TElement> element in source )
        {
            foreach ( TElement item in element ) { yield return item; }
        }
    }
    public static async IAsyncEnumerable<TElement> ConsolidateUnique<TElement>( this IAsyncEnumerable<IEnumerable<TElement>> values, [EnumeratorCancellation] CancellationToken token = default )
    {
        var results = new HashSet<TElement>();

        await foreach ( TElement element in values.Consolidate()
                                                  .WithCancellation( token ) ) { results.Add( element ); }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( TElement element in results )
        {
            if ( token.IsCancellationRequested ) { yield break; }

            yield return element;
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
        await foreach ( string? element in values )
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
        await foreach ( TElement? element in values )
        {
            if ( element is not null ) { yield return element; }
        }
    }


    public static async Task<IEnumerable<TElement>> WhereNotNull<TElement>( this Task<IEnumerable<TElement?>> values )
    {
        IEnumerable<TElement?> results = await values;
        return results.WhereNotNull();
    }
    public static async ValueTask<IEnumerable<TElement>> WhereNotNull<TElement>( this ValueTask<IEnumerable<TElement?>> values )
    {
        IEnumerable<TElement?> results = await values;
        return results.WhereNotNull();
    }


    public static async Task<IEnumerable<TElement>> WhereNotNull<TCollection, TElement>( this Task<TCollection> values ) where TCollection : IEnumerable<TElement?>
    {
        IEnumerable<TElement?> results = await values;
        return results.WhereNotNull();
    }
    public static async ValueTask<IEnumerable<TElement>> WhereNotNull<TCollection, TElement>( this ValueTask<TCollection> values ) where TCollection : IEnumerable<TElement?>
    {
        IEnumerable<TElement?> results = await values;
        return results.WhereNotNull();
    }


    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TElement>( this Task<IEnumerable<TElement?>> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values )
        {
            if ( element is not null ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TElement>( this ValueTask<IEnumerable<TElement?>> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values )
        {
            if ( element is not null ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TCollection, TElement>( this Task<TCollection> values ) where TCollection : IEnumerable<TElement?>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values )
        {
            if ( element is not null ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> WhereNotNullAsync<TCollection, TElement>( this ValueTask<TCollection> values ) where TCollection : IEnumerable<TElement?>
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TElement? element in await values )
        {
            if ( element is not null ) { yield return element; }
        }
    }


    public static async ValueTask<bool> All<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( !selector( element ) ) { return false; }
        }

        return true;
    }
    public static async ValueTask<bool> All<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( !await selector( element ) ) { return false; }
        }

        return true;
    }
    public static async ValueTask<bool> All<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, Task<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( !await selector( element ) ) { return false; }
        }

        return true;
    }


    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        await foreach ( TElement _ in source.WithCancellation( token ) ) { return true; }

        return false;
    }
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( selector( element ) ) { return true; }
        }

        return false;
    }
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( await selector( element ) ) { return true; }
        }

        return false;
    }
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, Task<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( await selector( element ) ) { return true; }
        }

        return false;
    }
}
