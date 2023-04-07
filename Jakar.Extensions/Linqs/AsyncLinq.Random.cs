namespace Jakar.Extensions;
#nullable enable



public static partial class AsyncLinq
{
    public static TElement[] ToArray<TElement>( this IReadOnlyList<TElement> source )
    {
    #if NET6_0_OR_GREATER
        TElement[] array = GC.AllocateUninitializedArray<TElement>( source.Count );
    #else
        var array = new TElement[source.Count];
    #endif

        for ( int i = 0; i < array.Length; i++ ) { array[i] = source[i]; }

        return array;
    }


    public static IEnumerable<TElement> Random<TElement>( this IReadOnlyList<TElement> items, Random random, CancellationToken token = default )
    {
        while ( token.ShouldContinue() ) { yield return items[random.Next( items.Count )]; }
    }
    public static IEnumerable<TElement> Random<TElement>( this IReadOnlyList<TElement> items, Random random, int count, CancellationToken token = default ) => items.Random( random, token )
                                                                                                                                                                    .Take( count );


    public static IEnumerable<KeyValuePair<TKey, TElement>> Random<TKey, TElement>( this IReadOnlyDictionary<TKey, TElement> dict, Random random, CancellationToken token = default )
    {
        while ( token.ShouldContinue() ) { yield return dict.ElementAt( random.Next( dict.Count ) ); }
    }
    public static IEnumerable<TElement> RandomValues<TKey, TElement>( this IReadOnlyDictionary<TKey, TElement> dict, Random random, CancellationToken token = default ) => dict.Random( random, token )
                                                                                                                                                                               .Select( pair => pair.Value );
    public static IEnumerable<TKey> RandomKeys<TKey, TElement>( this IReadOnlyDictionary<TKey, TElement> dict, Random random, CancellationToken token = default ) => dict.Random( random, token )
                                                                                                                                                                         .Select( pair => pair.Key );
}
