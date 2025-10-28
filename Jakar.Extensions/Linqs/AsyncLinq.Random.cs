namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static IEnumerable<TElement> Random<TElement>( this IReadOnlyList<TElement> items, Random random, CancellationToken token = default )
    {
        while ( token.ShouldContinue() ) { yield return items[random.Next(items.Count)]; }
    }
    public static IEnumerable<TElement> Random<TElement>( this IReadOnlyList<TElement> items, Random random, int count, CancellationToken token = default ) => items.Random(random, token)
                                                                                                                                                                    .Take(count);


    public static IEnumerable<KeyValuePair<TKey, TElement>> Random<TKey, TElement>( this IReadOnlyDictionary<TKey, TElement> dict, Random random, CancellationToken token = default )
    {
        while ( token.ShouldContinue() ) { yield return dict.ElementAt(random.Next(dict.Count)); }
    }
    public static IEnumerable<TElement> RandomValues<TKey, TElement>( this IReadOnlyDictionary<TKey, TElement> dict, Random random, CancellationToken token = default ) => dict.Random(random, token)
                                                                                                                                                                               .Select(pair => pair.Value);
    public static IEnumerable<TKey> RandomKeys<TKey, TElement>( this IReadOnlyDictionary<TKey, TElement> dict, Random random, CancellationToken token = default ) => dict.Random(random, token)
                                                                                                                                                                         .Select(pair => pair.Key);
}
