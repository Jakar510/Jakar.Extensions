// Jakar.Extensions :: Jakar.Extensions
// 09/11/2022  3:00 PM

#pragma warning disable CS8424
namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        await foreach ( TElement _ in source.WithCancellation(token) ) { return true; }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( selector(element) ) { return true; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }
    public static async ValueTask<bool> Any<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( await selector(element) ) { return true; }
        }

        throw new InvalidOperationException($"No records in {nameof(source)}");
    }


    public static async ValueTask<bool> All<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( !selector(element) ) { return false; }
        }

        return true;
    }
    public static async ValueTask<bool> All<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation(token) )
        {
            if ( !await selector(element) ) { return false; }
        }

        return true;
    }


    public static async IAsyncEnumerable<TElement> Append<TElement>( this IAsyncEnumerable<TElement> source, TElement value )
    {
        await foreach ( TElement element in source ) { yield return element; }

        yield return value;
    }
    public static async IAsyncEnumerable<TElement> Prepend<TElement>( this IAsyncEnumerable<TElement> source, TElement value )
    {
        yield return value;
        await foreach ( TElement element in source ) { yield return element; }
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


    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( await predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, bool> predicate )
    {
        int index = -1;

        await foreach ( TElement element in source )
        {
            checked { index++; }

            if ( predicate(element, index) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> Where<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<bool>> predicate )
    {
        int index = -1;

        await foreach ( TElement element in source )
        {
            checked { index++; }

            if ( await predicate(element, index) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, TResult> selector )
    {
        await foreach ( TElement element in source ) { yield return selector(element); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TResult>> selector )
    {
        await foreach ( TElement element in source ) { yield return await selector(element); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, int, TResult> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) ) { yield return selector(value, index); }
    }
    public static async IAsyncEnumerable<TResult> Select<TElement, TResult>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<TResult>> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) ) { yield return await selector(value, index); }
    }
    public static async IAsyncEnumerable<TElement> Skip<TElement>( this IAsyncEnumerable<TElement> source, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) )
        {
            if ( index >= count ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> predicate )
    {
        await foreach ( TElement element in source )
        {
            if ( await predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) )
        {
            if ( predicate(value, index) ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipWhile<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, int, ValueTask<bool>> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TElement? value ) in source.Enumerate(token) )
        {
            if ( await predicate(value, index) ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TElement> SkipLast<TElement>( this IAsyncEnumerable<TElement> source, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        List<TElement> list = await source.ToList(token);

        for ( var index = 0; index < list.Count; index++ )
        {
            if ( index >= count ) { yield break; }

            yield return list[index];
        }
    }
    public static IAsyncEnumerable<TElement> Distinct<TElement>( this IAsyncEnumerable<TElement> source, [EnumeratorCancellation] CancellationToken token = default ) => source.Distinct(EqualityComparer<TElement>.Default, token: token);
    public static async IAsyncEnumerable<TElement> Distinct<TElement>( this IAsyncEnumerable<TElement> source, IEqualityComparer<TElement> comparer, [EnumeratorCancellation] CancellationToken token = default )
    {
        HashSet<TElement> set = await source.ToHashSet(comparer, token);

        foreach ( TElement element in set )
        {
            token.ThrowIfCancellationRequested();
            yield return element;
        }
    }
    public static IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector ) =>
        source.DistinctBy(keySelector, EqualityComparer<TKey>.Default);
    public static IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TKey>> keySelector ) =>
        source.DistinctBy(keySelector, EqualityComparer<TKey>.Default);
    public static async IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector, IEqualityComparer<TKey> comparer )
    {
        var set = new HashSet<TKey>(comparer);

        await foreach ( TElement element in source )
        {
            if ( set.Add(keySelector(element)) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TElement> DistinctBy<TElement, TKey>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<TKey>> keySelector, IEqualityComparer<TKey> comparer )
    {
        var set = new HashSet<TKey>(comparer);

        await foreach ( TElement element in source )
        {
            if ( set.Add(await keySelector(element)
                            .ConfigureAwait(false)) ) { yield return element; }
        }
    }
}
