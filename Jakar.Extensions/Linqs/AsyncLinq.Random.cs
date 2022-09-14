namespace Jakar.Extensions;
#nullable enable



public static partial class AsyncLinq
{
    public static IEnumerator<TElement> Random<TElement>( this IReadOnlyIndexable<TElement> items, Random rand, CancellationToken token = default )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        while ( token.ShouldContinue() ) { yield return items[rand.Next(items.Count)]; }
    }
    public static IEnumerator<TElement> Random<TElement>( this IIndexable<TElement> items, Random rand, CancellationToken token = default )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        while ( token.ShouldContinue() ) { yield return items[rand.Next(items.Count)]; }
    }
    public static IEnumerator<TElement> Random<TElement>( this IReadOnlyList<TElement> items, Random rand, CancellationToken token = default )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        while ( token.ShouldContinue() ) { yield return items[rand.Next(items.Count)]; }
    }


    public static IEnumerator<TKey> RandomKeys<TKey, TElement>( this IDictionary<TKey, TElement> dict, Random rand, CancellationToken token = default )
    {
        if ( dict is null ) { throw new ArgumentNullException(nameof(dict)); }

        List<TKey> items = dict.Keys.ToList();

        while ( token.ShouldContinue() )
        {
            items.TryAdd(dict.Keys);
            yield return items[rand.Next(items.Count)];
        }
    }
    public static IEnumerator<TElement> RandomValues<TKey, TElement>( this IDictionary<TKey, TElement> dict, Random rand, CancellationToken token = default )
    {
        if ( dict is null ) { throw new ArgumentNullException(nameof(dict)); }

        List<TElement> items = dict.Values.ToList();

        while ( token.ShouldContinue() )
        {
            items.TryAdd(dict.Values);
            yield return items[rand.Next(items.Count)];
        }
    }
}
