namespace Jakar.Extensions;
#nullable enable



public static partial class AsyncLinq
{
    public static IEnumerable<TElement> Random<TElement>( this IReadOnlyList<TElement> items, Random rand, CancellationToken token = default )
    {
        while ( token.ShouldContinue() ) { yield return items[rand.Next( items.Count )]; }
    }
    public static IEnumerable<TElement> RandomValues<TKey, TElement>( this IDictionary<TKey, TElement> dict, Random rand, CancellationToken token = default )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TKey key in dict.RandomKeys( rand, token ) ) { yield return dict[key]; }
    }


    public static IEnumerable<TKey> RandomKeys<TKey, TElement>( this IDictionary<TKey, TElement> dict, Random rand, CancellationToken token = default )
    {
        while ( token.ShouldContinue() )
        {
            yield return dict.ElementAt( rand.Next( dict.Count ) )
                             .Key;
        }
    }
}
