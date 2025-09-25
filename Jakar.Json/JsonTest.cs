// Jakar.Extensions :: Console.Experiments
// 05/02/2022  9:21 AM

namespace Jakar.Json;


#if DEBUG
public sealed record JsonTest( string Name, double Number, params JsonTest[] Children ) : IJsonizer
{
    public JsonTest( string name, params JsonTest[] children ) : this( name, RandomNumber(), children ) { }


    public static void Run()
    {
        JsonTest json = Demo();
        Console.WriteLine();
        Console.WriteLine();
        string result;

        using ( StopWatch.Start() ) { result = json.ToJsonNet(); }

        Console.WriteLine( result );
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        using ( StopWatch.Start() ) { result = json.ToJson(); }

        Console.WriteLine( result );
        Console.WriteLine();
        Console.WriteLine();
    }


    public static JsonTest Demo()
    {
        var t1 = new JsonTest( "T1", new JsonTest( "T1.A1" ), new JsonTest( "T1.A2" ) );
        var t2 = new JsonTest( "T2", new JsonTest( "T2.B1" ), new JsonTest( "T2.B2" ) );
        return new JsonTest( nameof(Demo), t1, t2 );
    }


    public string ToJsonNet()
    {
        var sw = new StringWriter();

        var writer = new JsonTextWriter( sw )
                     {
                         Formatting = Formatting.Indented
                     };

        ToJsonNet( ref writer );
        return sw.ToString();
    }
    public void ToJsonNet( ref JsonTextWriter writer )
    {
        writer.WriteStartObject();

        writer.WritePropertyName( nameof(Name) );
        writer.WriteValue( Name );

        writer.WritePropertyName( nameof(Number) );
        writer.WriteValue( Number );

        writer.WritePropertyName( nameof(Children) );
        writer.WriteStartArray();

        foreach ( JsonTest child in Children ) { child.ToJsonNet( ref writer ); }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }


    public int JsonSize() => Name.Length + 100 + Children.Sum( x => x.JsonSize() );
    void IJsonizer.Serialize( ref JWriter writer )
    {
        JsonObject node = writer.AddObject();
        Serialize( ref node );
    }
    public void Serialize( ref JsonObject parent ) => parent.Add( Name ).AddNumber( Number ).AddArray( Children ).Complete();
    public void Serialize( ref JArray parent )
    {
        JsonObject node = parent.AddObject();
        Serialize( ref node );
    }
    public void Deserialize( ref JReader writer ) { }
    public void Deserialize( ref JNode   parent ) { }
    

    public static double RandomNumber() => Random.Shared.Next() * Random.Shared.NextDouble();
}



// public sealed record JsonTest
// {
//     public string?        AppName      { get; init; }
//     public DateTimeOffset TimeStamp { get; init; }
//     public double         Value     { get; init; }
// }
#endif
