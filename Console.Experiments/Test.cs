// // Jakar.Extensions :: Console.Experiments
// // 05/02/2022  9:21 AM
//
// #nullable enable
// using System.IO;
// using Jakar.Json;
// using Jakar.Json.Serialization;
// using JObject = Jakar.Json.Serialization.JObject;
//
//
//
// namespace Console.Experiments;
//
//
// public class JsonTest : IJsonizer
// {
//     public double      Number   { get; init; }
//     public List<Test>? Children { get; init; }
//
//
//     public string Name { get; init; } = string.Empty;
//
//
//     public Test() { }
//     public Test( string name, params Test[] children ) : this( name, new List<Test>( children ) ) { }
//     public Test( string name, List<Test>    children ) : this( name, random.Next() * random.NextDouble(), children ) { }
//     public Test( string name, double        number, params Test[] children ) : this( name, number, new List<Test>( children ) ) { }
//     public Test( string name, double number, List<Test> children )
//     {
//         Name     = name;
//         Number   = number;
//         Children = children;
//     }
//     public static readonly Random random = new();
//
//
//     public string ToJson()
//     {
//         var sw = new StringWriter();
//
//         var writer = new JsonTextWriter( sw )
//                      {
//                          Formatting = Formatting.Indented,
//                      };
//
//         ToJson( writer );
//         return sw.ToString();
//     }
//     public void ToJson( in JsonTextWriter writer )
//     {
//         writer.WriteStartObject();
//
//         writer.WritePropertyName( nameof(Name) );
//         writer.WriteValue( Name );
//
//         writer.WritePropertyName( nameof(Number) );
//         writer.WriteValue( Number );
//
//         writer.WritePropertyName( nameof(Children) );
//         writer.WriteStartArray();
//
//         if ( Children is not null )
//         {
//             foreach ( Test child in Children ) { child.ToJson( writer ); }
//         }
//
//         writer.WriteEndArray();
//         writer.WriteEndObject();
//     }
//
//
//     string IJsonizer.ToJson( in JWriter writer )
//     {
//         JObject node = writer.AddObject();
//         Serialize( ref node );
//         return writer.ToString();
//     }
//     public void Serialize( ref JObject parent ) => parent.Begin()
//                                                          .Add( Name )
//                                                          .Add( Number )
//                                                          .AddArray( Children )
//                                                          .Complete();
// }

namespace Console.Experiments;


public sealed record Test
{
    public string?        Name      { get; init; }
    public double         Value     { get; init; }
    public DateTimeOffset TimeStamp { get; init; } 
}
