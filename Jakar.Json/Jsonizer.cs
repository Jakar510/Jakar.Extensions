// Jakar.Extensions :: Jakar.Json
// 04/26/2022  10:37 AM

using System.Reflection;
using Jakar.Extensions;



#nullable enable
namespace Jakar.Json;


public sealed class JsonizerAttribute : Attribute { }



/// <summary>
/// A PREDICTABLE (de)serializer for any given object.
/// <para>
///	Primary goals are to be PERFORMANT, easily human readable, predictable and consistent.
/// </para>
/// <see cref="JsonModels.IJsonModel"/>
/// </summary>
public static class Jsonizer
{
    // public static T FromJson<T>( this string json ) where T : IJsonizer, new()
    // {
    //     var       value    = new T();
    //     using var document = new JReader(json);
    //     value.Deserialize(document);
    //     return value;
    // }


    public static string ToJson( this IJsonizer value, Formatting formatting = Formatting.Indented ) => ToJson(value, new JWriter(formatting));
    public static string ToJson( this IJsonizer value, in JWriter writer )
    {
        using ( writer )
        {
            value.ToJson(writer);
            return writer.ToString();
        }
    }
}
