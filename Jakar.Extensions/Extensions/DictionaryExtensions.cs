// Jakar.Extensions :: Jakar.Extensions
// 04/11/2024  11:04

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


public static class DictionaryExtensions
{
    public static void Add<TValue>( this TValue dictionary, string key, in StringValues value )
        where TValue : IDictionary<string, StringValues>
    {
        if ( dictionary.TryGetValue( key, out StringValues values ) )
        {
            dictionary[key] = StringValues.Concat( values, value );
            return;
        }

        dictionary.Add( key, value );
    }
    public static TValue GetOrAdd<TKey, TValue>( this Dictionary<TKey, TValue> dictionary, TKey key, TValue value )
        where TKey : notnull
    {
        ref TValue? entry = ref CollectionsMarshal.GetValueRefOrAddDefault( dictionary, key, out bool exists );

        if ( exists )
        {
            Debug.Assert( entry is not null );
            return entry;
        }

        entry = value;
        return entry;
    }
#if NET9_0_OR_GREATER
    public static TValue GetOrAdd<TKey, TValue, TAlternateKey>( this Dictionary<TKey, TValue> dictionary, TAlternateKey key, TValue value )
        where TKey : notnull
        where TAlternateKey : notnull, allows ref struct
    {
        Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
        ref TValue?                                             entry = ref CollectionsMarshal.GetValueRefOrAddDefault( lookup, key, out bool exists );

        if ( exists )
        {
            Debug.Assert( entry is not null );
            return entry;
        }

        entry = value;
        return entry;
    }
#endif
    public static bool TryUpdate<TKey, TValue>( this Dictionary<TKey, TValue> dictionary, TKey key, TValue value )
        where TKey : notnull
    {
        ref TValue entry = ref CollectionsMarshal.GetValueRefOrNullRef( dictionary, key );
        if ( Unsafe.IsNullRef( ref entry ) ) { return false; }

        entry = value;
        return true;
    }
#if NET9_0_OR_GREATER
    public static bool TryUpdate<TKey, TValue, TAlternateKey>( this Dictionary<TKey, TValue> dictionary, TAlternateKey key, TValue value )
        where TKey : notnull
        where TAlternateKey : notnull, allows ref struct
    {
        Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup = dictionary.GetAlternateLookup<TAlternateKey>();
        ref TValue                                              entry = ref CollectionsMarshal.GetValueRefOrNullRef( lookup, key );
        if ( Unsafe.IsNullRef( ref entry ) ) { return false; }

        entry = value;
        return true;
    }
#endif
}
