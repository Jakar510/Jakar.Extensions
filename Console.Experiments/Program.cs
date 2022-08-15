using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// using Jakar.Xml;
// using Jakar.Xml.Deserialization;


#nullable enable
namespace Console.Experiments;


public enum Page
{
    Home,
    Master,
    Detail
}



public static class Program
{
    public static async Task Main( string[] args )
    {
        try
        {
            "Hello World!".WriteToConsole();

            // BenchmarkRunner.Run<MapperBenchmarks>();
            // BenchmarkRunner.Run<JsonizerBenchmarks>();
            // BenchmarkRunner.Run<SpansBenchmarks>();

            // TestJson();

            // Test_Sql();

            await Test_HttpBuilder();


            // var id = Guid.NewGuid();
            // id.WriteToConsole();
            //
            // var b64 = id.ToBase64();
            // b64.WriteToConsole();
            //
            // var result = b64.AsGuid();
            // result?.WriteToConsole();
            // id.ToString().Length.WriteToConsole();
            // ( result == id ).WriteToConsole();

            "Bye".WriteToConsole();
        }
        catch ( Exception e )
        {
            e.WriteToConsole();
            e.WriteToDebug();
        }
    }


    public static async Task Test_HttpBuilder( CancellationToken token = default )
    {
        var          target  = new Uri("https://www.toptal.com/developers/postbin/");
        var          host    = new Uri("https://httpbin.org/");
        var          content = new AppVersion(1, 2, 3).ToString();
        WebRequester builder = WebRequestBuilder.Create(host).With_Host(host).With_Timeout(1).Build();


        ( await builder.Get("/bearer", token).AsJson() ).WriteToConsole();
        ( await builder.Put("/put", content, token).AsJson() ).WriteToConsole();
        ( await builder.Post("/post", content, token).AsJson() ).WriteToConsole();
        ( await builder.Get("/get", token).AsJson() ).WriteToConsole();
        ( await builder.Delete("/delete", token).AsJson() ).WriteToConsole();
        ( await builder.Patch("/patch", content, token).AsJson() ).WriteToConsole();
        ( await builder.Get("/headers",    token).AsJson() ).WriteToConsole();
        ( await builder.Get("/ip",         token).AsJson() ).WriteToConsole();
        ( await builder.Get("/user-agent", token).AsJson() ).WriteToConsole();
        ( await builder.Get("/cookies",    token).AsJson() ).WriteToConsole();
        ( await builder.Get("/image/png",  token).AsFile(MimeType.Png) ).WriteToConsole();
    }


    private static void Test_Sql()
    {
        // "SELECT * FROM Users"
        // var sql = new EasySqlBuilder().Select().All().From("Users").Result;
    }


    // try { First(); }
    // catch ( Exception e )
    // {
    //     e.ToString().WriteToConsole();
    //
    //     var details = new ExceptionDetails(e);
    //     details.ToPrettyJson().WriteToConsole();
    // }
    // public static void First() => Second();
    // private static void Second() => Third();
    // private static void Third() => Last();
    // private static void Last() => throw new NotImplementedException("", new NullReferenceException(nameof(Program)));

    private static void TestJson()
    {
        var first  = new Test("First",  new Test("1.1"), new Test("1.2"));
        var second = new Test("Second", new Test("2.1"));
        var test   = new Test("Root",   first, second);


        test.ToJson().WriteToConsole();
        string temp = test.ToJson();
        temp.WriteToConsole();

        "---TEST---".WriteToConsole();
        JToken json = temp.FromJson();
        json.ToJson(Formatting.Indented).WriteToConsole();
    }


//     public static void TestXml()
//     {
//         var document = new XDocument(@"
// <Group xmls=""System.Collections.Generic.List"">
// <Item>Test String</Item>
// </Group>");
//
//         foreach ( XNode node in document ) { }
//
//
//         // var d = new Dictionary<string, object>()
//         //         {
//         //             ["IDs"] = new List<double> { 1, 2, 3 },
//         //             [nameof(User)] = new User()
//         //                              {
//         //                                  Address = new List<Address>()
//         //                                            {
//         //                                                new()
//         //                                                {
//         //                                                    City  = "Plano",
//         //                                                    State = "Texas"
//         //                                                }
//         //                                            },
//         //                                  UserName   = "User",
//         //                                  IsActive   = true,
//         //                                  IsLoggedIn = true,
//         //                                  FirstName  = "First",
//         //                                  LastName   = "Last"
//         //                              },
//         //             ["Token"] = Guid.NewGuid(),
//         //             ["Data"] = new Dictionary<string, object>()
//         //                        {
//         //                            ["Test"] = "Success",
//         //                            ["Date"] = DateTime.Now
//         //                        },
//         //         };
//         //
//         //
//         // var l = new List<object>()
//         //         {
//         //             d,
//         //             1,
//         //             1d,
//         //             "hi",
//         //             new MultiDict(),
//         //             new[] { "1", "2", "3" },
//         //             TimeSpan.MinValue,
//         //             TimeSpan.MaxValue,
//         //             DateTime.MinValue,
//         //             DateTime.MaxValue,
//         //             (uint)0
//         //         };
//         //
//         // System.Console.WriteLine();
//         //
//         // string s = l.ToXml();
//         //
//         // s.WriteToConsole();
//         //
//         // var file = new LocalFile("Output.xml");
//         // await file.WriteToFileAsync(s);
//     }
}
