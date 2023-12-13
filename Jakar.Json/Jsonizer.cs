// Jakar.Extensions :: Jakar.Json
// 04/26/2022  10:37 AM

namespace Jakar.Json;


public sealed class JsonizerAttribute : Attribute { }



/// <summary> A PREDICTABLE (de)serializer for any given object.
///     <para> Primary goals are to be PERFORMANT, easily human readable, predictable and consistent. </para>
///     <see cref="Jakar.Extensions.MsJsonModels.IJsonModel"/> </summary>
public static class Jsonizer
{
    // public static T FromJson<T>( this string json ) where T : IJsonizer, new()
    // {
    //     var       value    = new T();
    //     using var document = new JReader(json);
    //     value.Deserialize(document);
    //     return value;
    // }


    public static string ToJson( this IJsonizer value, Formatting formatting = Formatting.Indented )
    {
        var writer = new JWriter( value.JsonSize(), formatting );

        try
        {
            value.Serialize( ref writer );
            string result = writer.ToString();
            return result;
        }
        finally { writer.Dispose(); }
    }
}
