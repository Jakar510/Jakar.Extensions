using System.ComponentModel;
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
        var            mapping    = new PairDict(properties.Length);

        foreach ( PropertyInfo property in properties )
        {
            object? value = property.GetMethod?.Invoke(context, default);
            mapping[property.Name] = value;
        }


        return new MContext(context, mapping);
    }

    public long AsLong( in ReadOnlySpan<char> key ) { throw new NotImplementedException(); }
}



#endif
