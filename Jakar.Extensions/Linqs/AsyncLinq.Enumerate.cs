using System.Xml.Linq;



namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TElement>( IAsyncEnumerable<TElement> self )
    {
        public IAsyncEnumerable<(TNumber Index, TElement Value)> Enumerate<TNumber>()
            where TNumber : INumber<TNumber> => self.Enumerate(TNumber.Zero);

        public async IAsyncEnumerable<(TNumber Index, TElement Value)> Enumerate<TNumber>( TNumber start )
            where TNumber : INumber<TNumber>
        {
            TNumber Index = start;

            await foreach ( TElement x in self.ConfigureAwait(false) )
            {
                checked { Index++; }

                yield return ( Index, x );
            }
        }
    }



    extension( IEnumerable self )
    {
        public IEnumerable<(TNumber Index, object? Value)> Enumerate<TNumber>()
            where TNumber : INumber<TNumber> => self.Enumerate(TNumber.Zero);

        public IEnumerable<(TNumber Index, object? Value)> Enumerate<TNumber>( TNumber start )
            where TNumber : INumber<TNumber>
        {
            TNumber Index = start;

            foreach ( object? item in self )
            {
                yield return ( Index, item );
                Index++;
            }
        }
    }



    extension<TElement>( IEnumerable<TElement> self )
    {
        public IEnumerable<(TNumber Index, TElement Value)> Enumerate<TNumber>()
            where TNumber : INumber<TNumber> => self.Enumerate(TNumber.Zero);

        public IEnumerable<(TNumber Index, TElement Value)> Enumerate<TNumber>( TNumber start )
            where TNumber : INumber<TNumber>
        {
            TNumber Index = start;

            foreach ( TElement item in self )
            {
                yield return ( Index, item );
                Index++;
            }
        }
    }



    extension<TKey, TElement>( IDictionary<TKey, TElement> self )
        where TKey : notnull
    {
        public IEnumerable<(TNumber Index, TKey Key, TElement Value)> Enumerate<TNumber>( bool sorted = true )
            where TNumber : INumber<TNumber> => self.Enumerate(TNumber.Zero, sorted);

        public IEnumerable<(TNumber Index, TKey Key, TElement Value)> Enumerate<TNumber>( TNumber start, bool sorted = true )
            where TNumber : INumber<TNumber>
        {
            TNumber           Index = start;
            ICollection<TKey> keys  = self.Keys;

            if ( sorted )
            {
                List<TKey> list = new(self.Keys);
                list.Sort();
                keys = list;
            }

            foreach ( var Key in keys )
            {
                yield return ( Index, Key, self[Key] );
                Index++;
            }
        }


        public IEnumerable<(TNumber Index, KeyValuePair<TKey, TElement> Pair)> EnumeratePairs<TNumber>( bool sorted = true )
            where TNumber : INumber<TNumber> => self.EnumeratePairs(TNumber.Zero, sorted);

        public IEnumerable<(TNumber Index, KeyValuePair<TKey, TElement> Pair)> EnumeratePairs<TNumber>( TNumber start, bool sorted = true )
            where TNumber : INumber<TNumber>
        {
            TNumber           Index = start;
            ICollection<TKey> keys  = self.Keys;

            if ( sorted )
            {
                List<TKey> list = new(self.Keys);
                list.Sort();
                keys = list;
            }

            foreach ( var Key in keys )
            {
                yield return ( Index, new KeyValuePair<TKey, TElement>(Key, self[Key]) );
                Index++;
            }
        }
    }



    extension( IDictionary self )
    {
        public IEnumerable<(TNumber Index, object Key, object? Value)> Enumerate<TNumber>()
            where TNumber : INumber<TNumber> => self.Enumerate(TNumber.Zero);

        public IEnumerable<(TNumber Index, object Key, object? Value)> Enumerate<TNumber>( TNumber start )
            where TNumber : INumber<TNumber>
        {
            TNumber Index = start;

            foreach ( DictionaryEntry Pair in self )
            {
                yield return ( Index, Pair.Key, Pair.Value );
                Index++;
            }
        }
    }
}
