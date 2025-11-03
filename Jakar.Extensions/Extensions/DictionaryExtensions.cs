// Jakar.Extensions :: Jakar.Extensions
// 04/11/2024  11:04

namespace Jakar.Extensions;


public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>( this Dictionary<TKey, TValue> dictionary, TKey key, TValue value )
        where TKey : notnull
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
    public static TValue GetOrAdd<TKey, TValue>( this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> value )
        where TKey : notnull
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
    public static TValue GetOrAdd<TKey, TValue>( this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> value )
        where TKey : notnull
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


    public static TValue GetOrAdd<TKey, TValue, TAlternateKey>( this Dictionary<TKey, TValue> dictionary, TAlternateKey key, TValue value )
        where TKey : notnull
        where TAlternateKey : notnull, allows ref struct
    {
        Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
        ref TValue?                                             entry  = ref CollectionsMarshal.GetValueRefOrAddDefault(lookup, key, out bool exists);
        if ( exists ) { return entry!; }

        entry = value;
        return entry;
    }
    public static TValue GetOrAdd<TKey, TValue, TAlternateKey>( this Dictionary<TKey, TValue> dictionary, TAlternateKey key, Func<TValue> value )
        where TKey : notnull
        where TAlternateKey : notnull, allows ref struct
    {
        Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
        ref TValue?                                             entry  = ref CollectionsMarshal.GetValueRefOrAddDefault(lookup, key, out bool exists);
        if ( exists ) { return entry!; }

        entry = value();
        return entry;
    }
    public static TValue GetOrAdd<TKey, TValue, TAlternateKey>( this Dictionary<TKey, TValue> dictionary, TAlternateKey key, Func<TAlternateKey, TValue> value )
        where TKey : notnull
        where TAlternateKey : notnull, allows ref struct
    {
        Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
        ref TValue?                                             entry  = ref CollectionsMarshal.GetValueRefOrAddDefault(lookup, key, out bool exists);
        if ( exists ) { return entry!; }

        entry = value(key);
        return entry;
    }


    public static bool TryUpdate<TKey, TValue>( this Dictionary<TKey, TValue> dictionary, TKey key, TValue value )
        where TKey : notnull
    {
        ref TValue entry = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);

        entry = value;
        return true;
    }
    public static bool TryUpdate<TKey, TValue, TAlternateKey>( this Dictionary<TKey, TValue> dictionary, TAlternateKey key, TValue value )
        where TKey : notnull
        where TAlternateKey : notnull, allows ref struct
    {
        Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
        ref TValue                                              entry  = ref CollectionsMarshal.GetValueRefOrNullRef(lookup, key);
        entry = value;
        return true;
    }
}
