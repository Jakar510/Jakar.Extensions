// Jakar.Extensions :: Jakar.Extensions
// 04/11/2024  11:04

namespace Jakar.Extensions;


public static class DictionaryExtensions
{
    extension<TKey, TValue>( Dictionary<TKey, TValue> dictionary )
        where TKey : notnull
    {
        public TValue GetOrAdd( TKey key, TValue value )
        {
            ref TValue? entry = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);

            if ( exists )
            {
                Debug.Assert(entry is not null);
                return entry;
            }

            entry = value;
            return entry;
        }
        public TValue GetOrAdd( TKey key, Func<TValue> value )
        {
            ref TValue? entry = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);

            if ( exists )
            {
                Debug.Assert(entry is not null);
                return entry;
            }

            entry = value();
            return entry;
        }
        public TValue GetOrAdd( TKey key, Func<TKey, TValue> value )
        {
            ref TValue? entry = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);

            if ( exists )
            {
                Debug.Assert(entry is not null);
                return entry;
            }

            entry = value(key);
            return entry;
        }
        public TValue GetOrAdd<TAlternateKey>( TAlternateKey key, TValue value )
            where TAlternateKey : notnull, allows ref struct
        {
            Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
            ref TValue?                                             entry  = ref CollectionsMarshal.GetValueRefOrAddDefault(lookup, key, out bool exists);
            if ( exists ) { return entry!; }

            entry = value;
            return entry;
        }
        public TValue GetOrAdd<TAlternateKey>( TAlternateKey key, Func<TValue> value )
            where TAlternateKey : notnull, allows ref struct
        {
            Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
            ref TValue?                                             entry  = ref CollectionsMarshal.GetValueRefOrAddDefault(lookup, key, out bool exists);
            if ( exists ) { return entry!; }

            entry = value();
            return entry;
        }
        public TValue GetOrAdd<TAlternateKey>( TAlternateKey key, Func<TAlternateKey, TValue> value )
            where TAlternateKey : notnull, allows ref struct
        {
            Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
            ref TValue?                                             entry  = ref CollectionsMarshal.GetValueRefOrAddDefault(lookup, key, out bool exists);
            if ( exists ) { return entry!; }

            entry = value(key);
            return entry;
        }
        public bool TryUpdate( TKey key, TValue value )
        {
            ref TValue entry = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);

            entry = value;
            return true;
        }
        public bool TryUpdate<TAlternateKey>( TAlternateKey key, TValue value )
            where TAlternateKey : notnull, allows ref struct
        {
            Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
            ref TValue                                              entry  = ref CollectionsMarshal.GetValueRefOrNullRef(lookup, key);
            entry = value;
            return true;
        }
    }
}
