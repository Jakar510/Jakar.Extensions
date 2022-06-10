using System.Buffers;
using Jakar.Extensions.Collections;
using Microsoft.Toolkit.HighPerformance.Enumerables;


#if NET6_0


// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  2:53 PM

#nullable enable
namespace Jakar.Mapper;


// A Pair holds a key and a value from a dictionary. It is used by the IEnumerable<T> implementation for both IDictionary<TKey, TValue> and IReadOnlyDictionary<TKey, TValue>.



public ref struct MContext
{
    private readonly object   _instance;
    private readonly PairDict _pairs;


    public MContext( object instance, in PairDict pairs )
    {
        _instance = instance;
        _pairs    = pairs;
    }


    public static MContext FromObject<T>( T context ) where T : notnull
    {
        PropertyInfo[] properties = context.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        Pair[]         mapping    = ArrayPool<Pair>.Shared.Rent(properties.Length);

        foreach ( SpanEnumerable<PropertyInfo>.Item pair in properties.Enumerate() )
        {
            string  key   = pair.Value.Name;
            object? value = pair.Value.GetMethod?.Invoke(context, default);
            mapping[pair.Index] = new Pair(key, value);
        }


        try { return new MContext(context, new PairDict(mapping)); }
        finally { ArrayPool<Pair>.Shared.Return(mapping); }
    }

    public long AsLong( in ReadOnlySpan<char> key ) { throw new NotImplementedException(); }
}



#endif
