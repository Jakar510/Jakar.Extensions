using Bogus.Bson;
using Newtonsoft.Json;



namespace Jakar.Extensions.Experiments.Benchmarks;


/*

   BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
   AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
   .NET SDK 8.0.100
   [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
   DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


   | Method                        | Mean        | Error     | StdDev    | Rank | Gen0   | Gen1   | Allocated |
   |------------------------------ |------------:|----------:|----------:|-----:|-------:|-------:|----------:|
   | ToStringSerialize             |    620.8 ns |  11.89 ns |  16.67 ns |    1 | 0.1640 |      - |    1376 B |
   | FakerDeserialize              |    901.8 ns |   3.68 ns |   3.07 ns |    2 | 0.0820 |      - |     690 B |
   | SystemTextJsonSerialize       |  5,760.6 ns |  44.45 ns |  41.58 ns |    3 | 0.5417 |      - |    4536 B |
   | SystemTextJsonSerializePretty |  7,538.7 ns | 121.15 ns | 113.33 ns |    4 | 0.8545 | 0.0153 |    7190 B |
   | JsonNetSerialize              | 14,199.0 ns | 179.53 ns | 159.15 ns |    5 | 2.0447 | 0.0610 |   17144 B |
   | JsonNetSerializePretty        | 17,021.6 ns | 268.17 ns | 250.84 ns |    6 | 2.3499 | 0.0305 |   19760 B |
   | SystemTextJsonDeserialize     | 18,266.2 ns |  94.49 ns |  88.38 ns |    7 | 1.0376 |      - |    8696 B |
   | JsonNetDeserialize            | 24,603.9 ns |  83.56 ns |  74.07 ns |    8 | 1.1597 |      - |    9776 B |

 */



[Config(typeof(BenchmarkConfig)), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), SimpleJob(RuntimeMoniker.HostProcess), Orderer(SummaryOrderPolicy.FastestToSlowest), RankColumn, MemoryDiagnoser, SuppressMessage("ReSharper", "InconsistentNaming")]
public class JsonNet_SystemTextJson_Benchmarks
{
    private const string NODE_JSON = """
                                     {
                                       "AppName": "Gorgeous Plastic Shirt",
                                       "Description": "Fantastic",
                                       "Price": 23.474790677749812,
                                       "Date": "2023-12-03T18:16:23.6915673+00:00",
                                       "Children": [
                                         {
                                           "AppName": "Awesome Plastic Car",
                                           "Description": "Small",
                                           "Price": 30.657281164019174,
                                           "Date": "2023-12-03T18:16:23.6929482+00:00",
                                           "Children": [
                                             {
                                               "AppName": "Unbranded Concrete Pizza",
                                               "Description": "Fantastic",
                                               "Price": 94.07766085149403,
                                               "Date": "2023-12-04T18:16:23.6929638+00:00",
                                               "Children": [
                                                 {
                                                   "AppName": "Unbranded Granite Salad",
                                                   "Description": "Small",
                                                   "Price": 66.06374753622815,
                                                   "Date": "2023-12-05T18:16:23.6929664+00:00",
                                                   "Children": [
                                                     {
                                                       "AppName": "Refined Frozen Pants",
                                                       "Description": "Tasty",
                                                       "Price": 8.501224852951566,
                                                       "Date": "2023-11-24T18:16:23.6929686+00:00",
                                                       "Children": []
                                                     }
                                                   ]
                                                 },
                                                 {
                                                   "AppName": "Small Metal Sausages",
                                                   "Description": "Generic",
                                                   "Price": 40.94389211369755,
                                                   "Date": "2023-12-08T18:16:23.6930666+00:00",
                                                   "Children": []
                                                 }
                                               ]
                                             },
                                             {
                                               "AppName": "Fantastic Granite Bike",
                                               "Description": "Rustic",
                                               "Price": 22.186910049957202,
                                               "Date": "2023-12-03T18:16:23.6930708+00:00",
                                               "Children": []
                                             },
                                             {
                                               "AppName": "Fantastic Granite Bacon",
                                               "Description": "Handcrafted",
                                               "Price": 47.41072325681185,
                                               "Date": "2023-12-06T18:16:23.693073+00:00",
                                               "Children": []
                                             },
                                             {
                                               "AppName": "Awesome Granite Car",
                                               "Description": "Practical",
                                               "Price": 53.86904665950482,
                                               "Date": "2023-11-26T18:16:23.6930752+00:00",
                                               "Children": []
                                             }
                                           ]
                                         },
                                         {
                                           "AppName": "Generic Wooden Chips",
                                           "Description": "Rustic",
                                           "Price": 98.79916960229026,
                                           "Date": "2023-11-29T18:16:23.6930775+00:00",
                                           "Children": []
                                         },
                                         {
                                           "AppName": "Intelligent Plastic Shirt",
                                           "Description": "Unbranded",
                                           "Price": 44.68665022123856,
                                           "Date": "2023-11-23T18:16:23.6930796+00:00",
                                           "Children": []
                                         },
                                         {
                                           "AppName": "Gorgeous Fresh Pants",
                                           "Description": "Incredible",
                                           "Price": 30.797002792335935,
                                           "Date": "2023-11-28T18:16:23.6930815+00:00",
                                           "Children": []
                                         },
                                         {
                                           "AppName": "Unbranded Steel Fish",
                                           "Description": "Incredible",
                                           "Price": 93.00952775914556,
                                           "Date": "2023-12-06T18:16:23.6930836+00:00",
                                           "Children": []
                                         },
                                         {
                                           "AppName": "Gorgeous Concrete Pants",
                                           "Description": "Refined",
                                           "Price": 67.5470646020505,
                                           "Date": "2023-12-06T18:16:23.6930857+00:00",
                                           "Children": []
                                         },
                                         {
                                           "AppName": "Gorgeous Plastic Pants",
                                           "Description": "Generic",
                                           "Price": 84.19487140880095,
                                           "Date": "2023-11-30T18:16:23.6930876+00:00",
                                           "Children": []
                                         },
                                         {
                                           "AppName": "Generic Plastic Soap",
                                           "Description": "Handmade",
                                           "Price": 40.52820379402765,
                                           "Date": "2023-12-04T18:16:23.6930897+00:00",
                                           "Children": []
                                         }
                                       ]
                                     }
                                     """;

    private const string TestJson_JSON = """
                                         {
                                             "Errors": {
                                                 "Alert": null,
                                                 "Details": [
                                                     {
                                                         "Detail": null,
                                                         "Errors": [],
                                                         "Instance": null,
                                                         "StatusCode": 202,
                                                         "Title": "Accepted",
                                                         "Type": "Server.Accepted"
                                                     },
                                                     {
                                                         "Detail": null,
                                                         "Errors": [],
                                                         "Instance": null,
                                                         "StatusCode": 400,
                                                         "Title": "BadRequest",
                                                         "Type": "Client.BadRequest"
                                                     }
                                                 ]
                                             },
                                             "User": {
                                                 "Addresses": [],
                                                 "Company": "",
                                                 "CreatedBy": null,
                                                 "Department": "",
                                                 "Description": "",
                                                 "Email": "",
                                                 "EscalateTo": null,
                                                 "Ext": "",
                                                 "FirstName": "",
                                                 "FullName": "",
                                                 "Gender": "",
                                                 "Groups": [],
                                                 "ImageID": null,
                                                 "LastName": "",
                                                 "PhoneNumber": "",
                                                 "PreferredLanguage": 0,
                                                 "Rights": "",
                                                 "Roles": [],
                                                 "SubscriptionExpires": null,
                                                 "Title": "",
                                                 "UserID": "00000000-0000-0000-0000-000000000000",
                                                 "UserName": "",
                                                 "Website": "",
                                                 "ID": "00000000-0000-0000-0000-000000000000"
                                             },
                                             "Files": [
                                                 {
                                                     "FileSize": 0,
                                                     "Hash": "Hash",
                                                     "ID": "00000000-0000-0000-0000-000000000000",
                                                     "MetaData": {
                                                         "FileDescription": null,
                                                         "FileName": "file.dat",
                                                         "FileType": "application/octet-stream",
                                                         "MimeType": 16
                                                     },
                                                     "Payload": "payload"
                                                 }
                                             ],
                                             "CreateUser": {
                                                 "ConfirmPassword": "",
                                                 "Password": "",
                                                 "UserName": "",
                                                 "Addresses": [],
                                                 "Company": "",
                                                 "CreatedBy": null,
                                                 "Department": "",
                                                 "Description": "",
                                                 "Email": "",
                                                 "EscalateTo": null,
                                                 "Ext": "",
                                                 "FirstName": "",
                                                 "FullName": "",
                                                 "Gender": "",
                                                 "Groups": [],
                                                 "ImageID": null,
                                                 "LastName": "",
                                                 "PhoneNumber": "",
                                                 "PreferredLanguage": 0,
                                                 "Rights": "",
                                                 "Roles": [],
                                                 "SubscriptionExpires": null,
                                                 "Title": "",
                                                 "UserID": "00000000-0000-0000-0000-000000000000",
                                                 "Website": "",
                                                 "ID": "00000000-0000-0000-0000-000000000000"
                                             },
                                             "Location": {
                                                 "Accuracy": null,
                                                 "Altitude": null,
                                                 "AltitudeReferenceSystem": 0,
                                                 "Course": null,
                                                 "ID": "00000000-0000-0000-0000-000000000000",
                                                 "InstanceID": "00000000-0000-0000-0000-000000000000",
                                                 "IsFromMockProvider": false,
                                                 "Latitude": 0,
                                                 "Longitude": 0,
                                                 "Speed": null,
                                                 "Timestamp": "0001-01-01T00:00:00+00:00",
                                                 "VerticalAccuracy": null
                                             },
                                             "pair": {
                                                 "Key": "date",
                                                 "Value": "Friday, September 19, 2025"
                                             },
                                             "email": {
                                                 "Value": "bite@me.com"
                                             },
                                             "mutableError": {
                                                 "Detail": null,
                                                 "Errors": [],
                                                 "Instance": null,
                                                 "StatusCode": 500,
                                                 "Title": "InternalServerError",
                                                 "Type": "Server.InternalServerError"
                                             },
                                             "readOnlyError": {
                                                 "Detail": null,
                                                 "Errors": [],
                                                 "Instance": null,
                                                 "StatusCode": 500,
                                                 "Title": "InternalServerError",
                                                 "Type": "Server.InternalServerError"
                                             }
                                         }
                                         """;


    private static readonly Node     _node = NODE_JSON.FromJson<Node>(Node.JsonContext);
    private static readonly TestJson _test = TestJson_JSON.FromJson<TestJson>(TestJson.JsonContext);


    [Benchmark, Category("Serialize")]   public string? Node_ToString_Serialize()             => _node.ToString();
    [Benchmark, Category("Serialize")]   public string  Node_JsonNet_Serialize()              => JsonConvert.SerializeObject(_node, Formatting.None);
    [Benchmark, Category("Serialize")]   public string  Node_JsonNet_Serialize_Pretty()       => JsonConvert.SerializeObject(_node, Formatting.Indented);
    [Benchmark, Category("Serialize")]   public string  Node_SystemTextJson_Serialize()       => JsonSerializer.Serialize(_node, ExperimentContext.Default.Node);
    [Benchmark, Category("Serialize")]   public string  Node_SystemTextJson_SerializePretty() => JsonSerializer.Serialize(_node, ExperimentContext.Pretty);
    [Benchmark, Category("Deserialize")] public Node?   Node_Faker_Deserialize()              => Node.NodeFaker.Instance.Generate();
    [Benchmark, Category("Deserialize")] public Node?   Node_JsonNet_Deserialize()            => JsonConvert.DeserializeObject<Node>(NODE_JSON);
    [Benchmark, Category("Deserialize")] public Node?   Node_SystemTextJson_Deserialize()     => JsonSerializer.Deserialize(NODE_JSON, ExperimentContext.Default.Node);


    [Benchmark, Category("Serialize")]   public string?   TestJson_ToString_Serialize()             => _test.ToString();
    [Benchmark, Category("Serialize")]   public string    TestJson_JsonNet_Serialize()              => JsonConvert.SerializeObject(_test, Formatting.None);
    [Benchmark, Category("Serialize")]   public string    TestJson_JsonNet_Serialize_Pretty()       => JsonConvert.SerializeObject(_test, Formatting.Indented);
    [Benchmark, Category("Serialize")]   public string    TestJson_SystemTextJson_Serialize()       => JsonSerializer.Serialize(_test, ExperimentContext.Default.Node);
    [Benchmark, Category("Serialize")]   public string    TestJson_SystemTextJson_SerializePretty() => JsonSerializer.Serialize(_test, ExperimentContext.Pretty);
    [Benchmark, Category("Deserialize")] public TestJson? TestJson_JsonNet_Deserialize()            => JsonConvert.DeserializeObject<TestJson>(NODE_JSON);
    [Benchmark, Category("Deserialize")] public TestJson? TestJson_SystemTextJson_Deserialize()     => JsonSerializer.Deserialize(TestJson_JSON, ExperimentContext.Default.TestJson);


    public static void Test()
    {
        JsonNet_SystemTextJson_Benchmarks benchmarks = new();

        using ( StopWatch.Start(nameof(Node_ToString_Serialize)) ) { benchmarks.Node_ToString_Serialize(); }

        using ( StopWatch.Start(nameof(Node_JsonNet_Deserialize)) ) { benchmarks.Node_JsonNet_Deserialize(); }

        using ( StopWatch.Start(nameof(Node_JsonNet_Serialize_Pretty)) ) { benchmarks.Node_JsonNet_Serialize_Pretty(); }

        using ( StopWatch.Start(nameof(Node_SystemTextJson_Serialize)) ) { benchmarks.Node_SystemTextJson_Serialize(); }

        using ( StopWatch.Start(nameof(Node_SystemTextJson_SerializePretty)) ) { benchmarks.Node_SystemTextJson_SerializePretty(); }

        using ( StopWatch.Start(nameof(Node_Faker_Deserialize)) ) { benchmarks.Node_Faker_Deserialize(); }

        using ( StopWatch.Start(nameof(Node_JsonNet_Deserialize)) ) { benchmarks.Node_JsonNet_Deserialize(); }

        using ( StopWatch.Start(nameof(Node_SystemTextJson_Deserialize)) ) { benchmarks.Node_SystemTextJson_Deserialize(); }


        using ( StopWatch.Start(nameof(TestJson_ToString_Serialize)) ) { benchmarks.TestJson_ToString_Serialize(); }

        using ( StopWatch.Start(nameof(TestJson_JsonNet_Serialize)) ) { benchmarks.TestJson_JsonNet_Serialize(); }

        using ( StopWatch.Start(nameof(TestJson_JsonNet_Serialize_Pretty)) ) { benchmarks.TestJson_JsonNet_Serialize_Pretty(); }

        using ( StopWatch.Start(nameof(TestJson_SystemTextJson_Serialize)) ) { benchmarks.TestJson_SystemTextJson_Serialize(); }

        using ( StopWatch.Start(nameof(TestJson_SystemTextJson_SerializePretty)) ) { benchmarks.TestJson_SystemTextJson_SerializePretty(); }

        using ( StopWatch.Start(nameof(TestJson_JsonNet_Deserialize)) ) { benchmarks.TestJson_JsonNet_Deserialize(); }

        using ( StopWatch.Start(nameof(TestJson_SystemTextJson_Deserialize)) ) { benchmarks.TestJson_SystemTextJson_Deserialize(); }
    }
    public static async ValueTask SaveAsync()
    {
        using TelemetrySpan span = TelemetrySpan.Create();
        LocalFile           file = "test.json";
        await file.WriteAsync(NODE_JSON);
        Console.WriteLine(file.FullPath);
    }
}
