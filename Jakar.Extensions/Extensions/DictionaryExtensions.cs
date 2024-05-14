// Jakar.Extensions :: Jakar.Extensions
// 04/11/2024  11:04

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


public static class DictionaryExtensions
{
    public static void Add<T>( this T dictionary, string key, in StringValues value )
        where T : IDictionary<string, StringValues>
    {
        if ( dictionary.TryGetValue( key, out StringValues values ) )
        {
            dictionary[key] = StringValues.Concat( values, value );
            return;
        }

        dictionary.Add( key, value );
    }
}
