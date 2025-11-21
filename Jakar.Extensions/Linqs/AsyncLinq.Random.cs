namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TElement>( IReadOnlyList<TElement> items )
    {
        public IEnumerable<TElement> Random( Random random, CancellationToken token = default )
        {
            while ( token.ShouldContinue() ) { yield return items[random.Next(items.Count)]; }
        }
        public IEnumerable<TElement> Random( Random random, int count, CancellationToken token = default ) => items.Random(random, token)
                                                                                                                   .Take(count);
    }



    extension<TKey, TElement>( IReadOnlyDictionary<TKey, TElement> dict )
    {
        public IEnumerable<KeyValuePair<TKey, TElement>> Random( Random random, CancellationToken token = default )
        {
            while ( token.ShouldContinue() ) { yield return dict.ElementAt(random.Next(dict.Count)); }
        }
        public IEnumerable<TElement> RandomValues( Random random, CancellationToken token = default ) => dict.Random(random, token)
                                                                                                             .Select(pair => pair.Value);
        public IEnumerable<TKey> RandomKeys( Random random, CancellationToken token = default ) => dict.Random(random, token)
                                                                                                       .Select(pair => pair.Key);
    }
}
