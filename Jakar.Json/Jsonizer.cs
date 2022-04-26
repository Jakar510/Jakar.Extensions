// Jakar.Extensions :: Jakar.Json
// 04/26/2022  10:37 AM

using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Xml;
using Jakar.Extensions;
using Jakar.Extensions.Exceptions.General;
using Jakar.Extensions.Types;
using Jakar.Json.Deserialization;
using Jakar.Json.Serialization;



namespace Jakar.Json;


/// <summary>
/// A PREDICTABLE (de)serializer for any given object.
/// <para>
///	Primary goals are to be PERFORMANT, easily human readable, predictable and consistent.
/// </para>
/// <see cref="JsonModels.IJsonModel"/>
/// </summary>
public static class Jsonizer
{
    /// <summary>
    /// Maps <see cref="Type.FullName"/> to <see cref="Type"/>.
    /// <para>
    ///	Uses <see cref="NodeNames.GetTypeName"/> to get the <see cref="Type.FullName"/>
    /// </para>
    /// </summary>
    internal static readonly ConcurrentDictionary<Type, Func<string, object>> parsers = new();


    public static T FromJson<T>( this string json ) where T : IJsonizer, new()
    {
        void Properties( ref object      obj, in JNode parent ) { }
        void Fields( ref     object      obj, in JNode parent ) { }
        void Array( ref      IEnumerable obj, in JNode parent ) { }
        void Dictionary( ref IDictionary obj, in JNode parent ) { }


        var value    = new T();
        var document = new JReader(json);


        return value;
    }
    public static string ToJson<T>( this T value ) where T : IJsonizer, new()
    {
        var writer = new JWriter();
        value.ToJson(writer);
        return writer.ToString();
    }
}
