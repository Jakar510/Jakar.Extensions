using OneOf.Types;


#pragma warning disable CS8424



namespace Jakar.Extensions;


/// <summary>
/// <para><see cref="System.Linq.Enumerable"/></para>
/// </summary>
public static class AsyncLinq
{
    public static async Task<List<TSource>> ToList<TSource>( this IAsyncEnumerable<TSource> enumerable, CancellationToken token = default )
    {
        var list = new List<TSource>();
        await foreach ( TSource element in enumerable.WithCancellation(token) ) { list.Add(element); }

        return list;
    }
    public static async Task<HashSet<TSource>> ToHashSet<TSource>( this IAsyncEnumerable<TSource> enumerable, CancellationToken token = default )
    {
        var list = new HashSet<TSource>();
        await foreach ( TSource element in enumerable.WithCancellation(token) ) { list.Add(element); }

        return list;
    }
    public static async Task<ObservableCollection<TSource>> ToObservableCollection<TSource>( this IAsyncEnumerable<TSource> enumerable, CancellationToken token = default )
    {
        var list = new ObservableCollection<TSource>();
        await foreach ( TSource element in enumerable.WithCancellation(token) ) { list.Add(element); }

        return list;
    }
    public static async Task<IEnumerable<TSource>> ToEnumerable<TSource>( this IAsyncEnumerable<TSource> enumerable, CancellationToken token = default )
    {
        var list = new List<TSource>();
        await foreach ( TSource element in enumerable.WithCancellation(token) ) { list.Add(element); }

        return list;
    }


    public static async IAsyncEnumerable<(int Index, TSource Value)> Enumerate<TSource>( this IAsyncEnumerable<TSource> source, [EnumeratorCancellation] CancellationToken token = default )
    {
        var index = 0;

        await foreach ( TSource element in source.WithCancellation(token) )
        {
            checked { index++; }

            yield return ( index, element );
        }
    }


    public static async IAsyncEnumerable<TSource> Where<TSource>( this IAsyncEnumerable<TSource> enumerable, Func<TSource, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSource element in enumerable.WithCancellation(token) )
        {
            if ( predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TSource> Where<TSource>( this IAsyncEnumerable<TSource> enumerable, Func<TSource, int, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        int index = -1;

        await foreach ( TSource element in enumerable.WithCancellation(token) )
        {
            checked { index++; }

            if ( predicate(element, index) ) { yield return element; }
        }
    }


    public static async IAsyncEnumerable<TResult> Select<TSource, TResult>( this IAsyncEnumerable<TSource> enumerable, Func<TSource, TResult> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSource element in enumerable.WithCancellation(token) ) { yield return selector(element); }
    }
    public static async IAsyncEnumerable<TResult> Select<TSource, TResult>( this IAsyncEnumerable<TSource> enumerable, Func<TSource, int, TResult> selector, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TSource? value ) in enumerable.Enumerate(token) ) { yield return selector(value, index); }
    }


    public static async IAsyncEnumerable<TSource> Skip<TSource>( this IAsyncEnumerable<TSource> source, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TSource? value ) in source.Enumerate(token) )
        {
            if ( index >= count ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TSource> SkipWhile<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSource element in source.WithCancellation(token) )
        {
            if ( !predicate(element) ) { yield return element; }
        }
    }
    public static async IAsyncEnumerable<TSource> SkipWhile<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( ( int index, TSource? value ) in source.Enumerate(token) )
        {
            if ( predicate(value, index) ) { yield return value; }
        }
    }
    public static async IAsyncEnumerable<TSource> SkipLast<TSource>( this IAsyncEnumerable<TSource> source, int count, [EnumeratorCancellation] CancellationToken token = default )
    {
        List<TSource> list = await source.ToList(token);

        foreach ( ( int index, TSource? element ) in list.Enumerate() )
        {
            if ( index < count ) { yield return element; }
        }
    }


    public static IAsyncEnumerable<TSource> Distinct<TSource>( this IAsyncEnumerable<TSource> source, [EnumeratorCancellation] CancellationToken token = default ) => source.Distinct(EqualityComparer<TSource>.Default, token);
    public static async IAsyncEnumerable<TSource> Distinct<TSource>( this IAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, [EnumeratorCancellation] CancellationToken token = default )
    {
        HashSet<TSource> set = await source.ToHashSet(token);

        foreach ( TSource element in set )
        {
            token.ThrowIfCancellationRequested();
            yield return element;
        }
    }
    public static IAsyncEnumerable<TSource> DistinctBy<TSource, TKey>( this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, [EnumeratorCancellation] CancellationToken token = default ) =>
        source.DistinctBy(keySelector, EqualityComparer<TKey>.Default, token);
    public static async IAsyncEnumerable<TSource> DistinctBy<TSource, TKey>( this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken token = default )
    {
        var set = new HashSet<TKey>(comparer);

        await foreach ( TSource element in source.WithCancellation(token) )
        {
            if ( set.Add(keySelector(element)) ) { yield return element; }
        }
    }
}
