// Jakar.Extensions :: Jakar.Json
// 04/26/2022  10:37 AM

using System.Xml;



namespace Jakar.Json;


public sealed class JsonizerAttribute : Attribute { }



/// <summary> A PREDICTABLE (de)serializer for any given object.
///     <para> Primary goals are to be PERFORMANT, easily human readable, predictable and consistent. </para>
///     <see cref="IJsonModel"/> </summary>
public static class Jsonizer
{
    // public static TValue FromJson<TValue>( this string json ) where TValue : IJsonizer, new()
    // {
    //     var       value    = new TValue();
    //     using var document = new JReader(json);
    //     value.Deserialize(document);
    //     return value;
    // }


    public static string ToJson( this IJsonizer value, Formatting formatting = Formatting.Indented )
    {
        JWriter writer = new( value.JsonSize(), formatting );

        try
        {
            value.Serialize( ref writer );
            string result = writer.ToString();
            return result;
        }
        finally { writer.Dispose(); }
    }
}
