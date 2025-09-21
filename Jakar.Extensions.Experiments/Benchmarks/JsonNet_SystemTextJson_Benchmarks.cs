using Bogus.Bson;
using Newtonsoft.Json;



namespace Jakar.Extensions.Experiments.Benchmarks;


/*

BenchmarkDotNet v0.15.3, Windows 11 (10.0.26100.6584/24H2/2024Update/HudsonValley)
AMD Ryzen 9 3900X 3.80GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 9.0.305
 [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3 [AttachedDebugger]
 DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3


| Method                                  | Mean          | Error       | StdDev        | Rank | Gen0   | Gen1   | Allocated |
|---------------------------------------- |--------------:|------------:|--------------:|-----:|-------:|-------:|----------:|
| TestJson_ToString_Serialize             |      7.772 ns |   0.1547 ns |     0.2117 ns |    1 |      - |      - |         - |
| Node_ToString_Serialize                 |    660.156 ns |   9.1724 ns |     8.1311 ns |    2 | 0.1631 |      - |    1368 B |
| Node_Faker                  |  1,064.754 ns |  21.0140 ns |    22.4847 ns |    3 | 0.0820 |      - |     690 B |
| Node_JsonNet_Serialize                  |  1,201.324 ns |  18.9912 ns |    17.7643 ns |    4 | 0.2766 | 0.0010 |    2320 B |
| Node_JsonNet_Serialize_Pretty           |  1,314.537 ns |  26.2381 ns |    64.8540 ns |    5 | 0.3014 |      - |    2528 B |
| TestJson_SystemTextJson_Serialize       |  5,141.210 ns |  66.6114 ns |    62.3083 ns |    6 | 0.5035 |      - |    4216 B |
| Node_SystemTextJson_Serialize           |  6,284.724 ns |  90.7470 ns |    84.8848 ns |    7 | 0.4654 |      - |    3912 B |
| TestJson_SystemTextJson_SerializePretty |  8,319.549 ns | 163.8312 ns |   240.1416 ns |    8 | 0.7324 |      - |    6301 B |
| Node_SystemTextJson_SerializePretty     |  8,843.487 ns | 100.1265 ns |    88.7596 ns |    9 | 0.7935 | 0.0153 |    6658 B |
| TestJson_JsonNet_Serialize              | 17,611.836 ns | 350.1496 ns |   327.5301 ns |   10 | 2.0142 | 0.0610 |   16848 B |
| Node_SystemTextJson         | 19,370.384 ns | 385.1058 ns |   472.9445 ns |   11 | 0.9155 |      - |    7704 B |
| TestJson_JsonNet_Serialize_Pretty       | 20,704.243 ns | 408.3273 ns |   401.0320 ns |   12 | 2.2583 | 0.0610 |   19112 B |
| TestJson_JsonNet            | 21,344.637 ns | 393.7377 ns |   368.3025 ns |   12 | 1.8921 | 0.0610 |   15872 B |
| TestJson_SystemTextJson     | 27,037.069 ns | 513.6680 ns |   649.6256 ns |   13 | 2.1057 | 0.0610 |   17688 B |
| Node_JsonNet                | 27,937.809 ns | 553.1903 ns | 1,325.4088 ns |   13 | 1.2207 |      - |   10416 B |

 */



[JsonExporterAttribute.Full]
[MarkdownExporterAttribute.GitHub]
[Config(typeof(BenchmarkConfig))]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[SimpleJob(RuntimeMoniker.HostProcess)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[MemoryDiagnoser]
[SuppressMessage("ReSharper", "InconsistentNaming")]
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
                                             "Errors": {
                                                 "Alert": null,
                                                 "Details": [
                                                     {
                                                         "Detail": null,
                                                         "Errors": null,
                                                         "Instance": null,
                                                         "StatusCode": 202,
                                                         "Title": "Accepted",
                                                         "Type": "Server.Accepted"
                                                     },
                                                     {
                                                         "Detail": null,
                                                         "Errors": null,
                                                         "Instance": null,
                                                         "StatusCode": 400,
                                                         "Title": "BadRequest",
                                                         "Type": "Client.BadRequest"
                                                     }
                                                 ]
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
                                             "email": {
                                                 "Value": "bite@me.com"
                                             },
                                             "mutableError": {
                                                 "Detail": null,
                                                 "Errors": null,
                                                 "Instance": null,
                                                 "StatusCode": 500,
                                                 "Title": "InternalServerError",
                                                 "Type": "Server.InternalServerError"
                                             },
                                             "readOnlyError": {
                                                 "Detail": null,
                                                 "Errors": null,
                                                 "Instance": null,
                                                 "StatusCode": 500,
                                                 "Title": "InternalServerError",
                                                 "Type": "Server.InternalServerError"
                                             },
                                             "pair": {
                                                 "Key": "date",
                                                 "Value": "Saturday, September 20, 2025"
                                             }
                                         }
                                         """;


    private static readonly Node                   _node           = NODE_JSON.FromJson<Node>(Node.JsonContext);
    private static readonly TestJson               _test           = TestJson_JSON.FromJson<TestJson>(TestJson.JsonContext);
    private static readonly JsonSerializerSettings jsonNetSettings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


    // [Benchmark, Category("Serialize")]   public string? Node_ToString_Serialize()             => _node.ToString();
    // [Benchmark, Category("Serialize")]   public string  Node_JsonNet_Serialize()              => JsonConvert.SerializeObject(_node, Formatting.None,     jsonNetSettings);
    // [Benchmark, Category("Serialize")]   public string  Node_JsonNet_Serialize_Pretty()       => JsonConvert.SerializeObject(_node, Formatting.Indented, jsonNetSettings);
    [Benchmark, Category("Serialize")]   public string Serialize_Node_SystemTextJson()        => JsonSerializer.Serialize(_node, ExperimentContext.Default.Node);
    [Benchmark, Category("Serialize")]   public string Serialize_Pretty_Node_SystemTextJson() => JsonSerializer.Serialize(_node, ExperimentContext.Pretty);
    [Benchmark, Category("Deserialize")] public Node?  Deserialize_Node_Faker()               => Node.NodeFaker.Instance.Generate();
    [Benchmark, Category("Deserialize")] public Node?  Deserialize_Node_JsonNet()             => JsonConvert.DeserializeObject<Node>(NODE_JSON);
    [Benchmark, Category("Deserialize")] public Node?  Deserialize_PNode_SystemTextJson()     => JsonSerializer.Deserialize(NODE_JSON, ExperimentContext.Default.Node);


    // [Benchmark] [Category("Serialize")]   public string?   TestJson_ToString_Serialize()             => _test.ToString();
    // [Benchmark] [Category("Serialize")]   public string    TestJson_JsonNet_Serialize()              => JsonConvert.SerializeObject(_test, Formatting.None,     jsonNetSettings);
    // [Benchmark] [Category("Serialize")]   public string    TestJson_JsonNet_Serialize_Pretty()       => JsonConvert.SerializeObject(_test, Formatting.Indented, jsonNetSettings);
    [Benchmark] [Category("Serialize")]   public string    Serialize_TestJson_SystemTextJson()        => JsonSerializer.Serialize(_test, ExperimentContext.Default.TestJson);
    [Benchmark] [Category("Serialize")]   public string    Serialize_Pretty_TestJson_SystemTextJson() => JsonSerializer.Serialize(_test, ExperimentContext.Pretty);
    [Benchmark] [Category("Deserialize")] public TestJson? Deserialize_TestJson_JsonNet()             => JsonConvert.DeserializeObject<TestJson>(NODE_JSON);
    [Benchmark] [Category("Deserialize")] public TestJson Deserialize_TestJson_SystemTextJson()      => TestJson_JSON.FromJson<TestJson>( ExperimentContext.Default);


    public static void Test()
    {
        JsonNet_SystemTextJson_Benchmarks benchmarks = new();

        // using ( StopWatch.Start(nameof(Node_ToString_Serialize)) ) { benchmarks.Node_ToString_Serialize(); }
        //
        // using ( StopWatch.Start(nameof(Node_JsonNet)) ) { benchmarks.Node_JsonNet(); }
        //
        // using ( StopWatch.Start(nameof(Node_JsonNet_Serialize_Pretty)) ) { benchmarks.Node_JsonNet_Serialize_Pretty(); }
        //
        // using ( StopWatch.Start(nameof(Node_SystemTextJson_Serialize)) ) { benchmarks.Node_SystemTextJson_Serialize(); }
        //
        // using ( StopWatch.Start(nameof(Node_SystemTextJson_SerializePretty)) ) { benchmarks.Node_SystemTextJson_SerializePretty(); }
        //
        // using ( StopWatch.Start(nameof(Node_Faker)) ) { benchmarks.Node_Faker(); }
        //
        // using ( StopWatch.Start(nameof(Node_JsonNet)) ) { benchmarks.Node_JsonNet(); }
        //
        // using ( StopWatch.Start(nameof(Node_SystemTextJson)) ) { benchmarks.Node_SystemTextJson(); }
        //
        //
        // using ( StopWatch.Start(nameof(TestJson_ToString_Serialize)) ) { benchmarks.TestJson_ToString_Serialize(); }
        //
        // using ( StopWatch.Start(nameof(TestJson_JsonNet_Serialize)) ) { benchmarks.TestJson_JsonNet_Serialize(); }
        //
        // using ( StopWatch.Start(nameof(TestJson_JsonNet_Serialize_Pretty)) ) { benchmarks.TestJson_JsonNet_Serialize_Pretty(); }
        //
        // using ( StopWatch.Start(nameof(TestJson_SystemTextJson_Serialize)) ) { benchmarks.TestJson_SystemTextJson_Serialize(); }
        //
        // using ( StopWatch.Start(nameof(TestJson_SystemTextJson_SerializePretty)) ) { benchmarks.TestJson_SystemTextJson_SerializePretty(); }
        //
        // using ( StopWatch.Start(nameof(TestJson_JsonNet)) ) { benchmarks.TestJson_JsonNet(); }
        //
        // using ( StopWatch.Start(nameof(TestJson_SystemTextJson)) ) { benchmarks.TestJson_SystemTextJson(); }
    }
}
