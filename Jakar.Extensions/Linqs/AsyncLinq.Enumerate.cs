namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TElement>( IAsyncEnumerable<TElement> source )
    {
        public async IAsyncEnumerable<(int Index, TElement Value)> Enumerate( int start = 0 )
        {
            int index = start;

            await foreach ( TElement element in source )
            {
                checked { index++; }

                yield return ( index, element );
            }
        }
        public async IAsyncEnumerable<(long Index, TElement Value)> Enumerate( long start = 0 )
        {
            long index = start;

            await foreach ( TElement element in source )
            {
                checked { index++; }

                yield return ( index, element );
            }
        }
    }



    public static IEnumerable<(int index, KeyValuePair<TKey, TElement> pair)> EnumeratePairs<TKey, TElement>( this IDictionary<TKey, TElement> element, int start = 0 )
    {
        int index = start;

        foreach ( KeyValuePair<TKey, TElement> pair in element )
        {
            yield return ( index, pair );
            index++;
        }
    }


    public static IEnumerable<(int index, object key, object? value)> Enumerate( this IDictionary element, int start = 0 )
    {
        int index = start;

        foreach ( DictionaryEntry pair in element )
        {
            yield return ( index, pair.Key, pair.Value );
            index++;
        }
    }


    public static IEnumerable<(int index, object? item)> Enumerate( this IEnumerable element, int start )
    {
        int index = start;

        foreach ( object? item in element )
        {
            yield return ( index, item );
            index++;
        }
    }
    public static IEnumerable<(int index, TElement item)> Enumerate<TElement>( this IEnumerable<TElement> element, int start )
    {
        int index = start;

        foreach ( TElement item in element )
        {
            yield return ( index, item );
            index++;
        }
    }
    extension<TKey, TElement>( IDictionary<TKey, TElement> element )
    {
        public IEnumerable<(int index, TKey key, TElement value)> Enumerate( int start = 0 )
        {
            int index = start;

            foreach ( ( TKey key, TElement value ) in element )
            {
                yield return ( index, key, value );
                index++;
            }
        }
        public IEnumerable<(long index, KeyValuePair<TKey, TElement> pair)> EnumeratePairs( long start = 0 )
        {
            long index = start;

            foreach ( KeyValuePair<TKey, TElement> pair in element )
            {
                yield return ( index, pair );
                index++;
            }
        }
    }



    public static IEnumerable<(long index, object key, object? value)> Enumerate( this IDictionary element, long start = 0 )
    {
        long index = start;

        foreach ( DictionaryEntry pair in element )
        {
            yield return ( index, pair.Key, pair.Value );
            index++;
        }
    }


    public static IEnumerable<(long index, object? item)> Enumerate( this IEnumerable element, long start )
    {
        long index = start;

        foreach ( object? item in element )
        {
            yield return ( index, item );
            index++;
        }
    }
    public static IEnumerable<(long index, TElement item)> Enumerate<TElement>( this IEnumerable<TElement> element, long start )
    {
        long index = start;

        foreach ( TElement item in element )
        {
            yield return ( index, item );
            index++;
        }
    }
    public static IEnumerable<(long index, TKey key, TElement value)> Enumerate<TKey, TElement>( this IDictionary<TKey, TElement> element, long start = 0 )
    {
        long index = start;

        foreach ( ( TKey key, TElement value ) in element )
        {
            yield return ( index, key, value );
            index++;
        }
    }
}
