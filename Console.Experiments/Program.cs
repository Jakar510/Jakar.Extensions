using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Http;
using Jakar.Extensions.Models;
using Jakar.Extensions.Strings;
using Jakar.SqlBuilder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// using Jakar.Xml;
// using Jakar.Xml.Deserialization;



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


            // BenchmarkRunner.Run<JsonizerBenchmarks>();
            // BenchmarkRunner.Run<SpansBenchmarks>();
            // TestJson();
            Test_Sql();

            await Test_HttpBuilder();

            "Bye".WriteToConsole();
        }
        catch ( Exception e )
        {
            e.WriteToConsole();
            throw;
        }
    }

    public static async Task Test_HttpBuilder( CancellationToken token = default )
    {
        var builder = HttpRequestBuilder.Create(new Uri("http://google.com"), token);

        var result = await builder.Get().AsString();
        result.WriteToConsole();
        string.Empty.WriteToConsole();
        string.Empty.WriteToConsole();

        result = await builder.Get().AsString();
        result.WriteToConsole();
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
