using System.Diagnostics;
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
| Node_Faker                              |  1,064.754 ns |  21.0140 ns |    22.4847 ns |    3 | 0.0820 |      - |     690 B |
| Node_JsonNet_Serialize                  |  1,201.324 ns |  18.9912 ns |    17.7643 ns |    4 | 0.2766 | 0.0010 |    2320 B |
| Node_JsonNet_Serialize_Pretty           |  1,314.537 ns |  26.2381 ns |    64.8540 ns |    5 | 0.3014 |      - |    2528 B |
| TestJson_SystemTextJson_Serialize       |  5,141.210 ns |  66.6114 ns |    62.3083 ns |    6 | 0.5035 |      - |    4216 B |
| Node_SystemTextJson_Serialize           |  6,284.724 ns |  90.7470 ns |    84.8848 ns |    7 | 0.4654 |      - |    3912 B |
| TestJson_SystemTextJson_SerializePretty |  8,319.549 ns | 163.8312 ns |   240.1416 ns |    8 | 0.7324 |      - |    6301 B |
| Node_SystemTextJson_SerializePretty     |  8,843.487 ns | 100.1265 ns |    88.7596 ns |    9 | 0.7935 | 0.0153 |    6658 B |
| TestJson_JsonNet_Serialize              | 17,611.836 ns | 350.1496 ns |   327.5301 ns |   10 | 2.0142 | 0.0610 |   16848 B |
| Node_SystemTextJson                     | 19,370.384 ns | 385.1058 ns |   472.9445 ns |   11 | 0.9155 |      - |    7704 B |
| TestJson_JsonNet_Serialize_Pretty       | 20,704.243 ns | 408.3273 ns |   401.0320 ns |   12 | 2.2583 | 0.0610 |   19112 B |
| TestJson_JsonNet                        | 21,344.637 ns | 393.7377 ns |   368.3025 ns |   12 | 1.8921 | 0.0610 |   15872 B |
| TestJson_SystemTextJson                 | 27,037.069 ns | 513.6680 ns |   649.6256 ns |   13 | 2.1057 | 0.0610 |   17688 B |
| Node_JsonNet                            | 27,937.809 ns | 553.1903 ns | 1,325.4088 ns |   13 | 1.2207 |      - |   10416 B |

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
    private const string JSON = """
                                {
                                    "Nodes": [
                                        {
                                            "Children": [
                                                {
                                                    "Children": [
                                                        {
                                                            "Children": [
                                                                {
                                                                    "Children": [],
                                                                    "Date": "2025-09-28T20:23:32.973228+00:00",
                                                                    "Description": "Small",
                                                                    "Name": "Generic Granite Salad",
                                                                    "Price": 73.59723355227325
                                                                }
                                                            ],
                                                            "Date": "2025-09-28T20:23:32.9732247+00:00",
                                                            "Description": "Tasty",
                                                            "Name": "Awesome Rubber Shoes",
                                                            "Price": 59.29224821488598
                                                        },
                                                        {
                                                            "Children": [],
                                                            "Date": "2025-09-19T20:23:32.973293+00:00",
                                                            "Description": "Generic",
                                                            "Name": "Incredible Soft Pizza",
                                                            "Price": 15.612019823465726
                                                        }
                                                    ],
                                                    "Date": "2025-09-22T20:23:32.9732206+00:00",
                                                    "Description": "Unbranded",
                                                    "Name": "Practical Cotton Bike",
                                                    "Price": 43.92076358125415
                                                },
                                                {
                                                    "Children": [],
                                                    "Date": "2025-09-29T20:23:32.9732957+00:00",
                                                    "Description": "Sleek",
                                                    "Name": "Generic Plastic Car",
                                                    "Price": 66.7009729458432
                                                },
                                                {
                                                    "Children": [],
                                                    "Date": "2025-09-13T20:23:32.9732977+00:00",
                                                    "Description": "Refined",
                                                    "Name": "Awesome Wooden Towels",
                                                    "Price": 4.697046870428632
                                                },
                                                {
                                                    "Children": [],
                                                    "Date": "2025-09-24T20:23:32.9732997+00:00",
                                                    "Description": "Generic",
                                                    "Name": "Awesome Frozen Ball",
                                                    "Price": 64.75540434166949
                                                },
                                                {
                                                    "Children": [],
                                                    "Date": "2025-09-20T20:23:32.9733017+00:00",
                                                    "Description": "Unbranded",
                                                    "Name": "Refined Frozen Fish",
                                                    "Price": 74.66624645431557
                                                }
                                            ],
                                            "Date": "2025-09-11T20:23:32.9728251+00:00",
                                            "Description": "Ergonomic",
                                            "Name": "Gorgeous Wooden Towels",
                                            "Price": 79.76934597419638
                                        },
                                        {
                                            "Children": [],
                                            "Date": "2025-09-12T20:23:32.973304+00:00",
                                            "Description": "Fantastic",
                                            "Name": "Awesome Plastic Fish",
                                            "Price": 23.585592902168827
                                        },
                                        {
                                            "Children": [],
                                            "Date": "2025-09-19T20:23:32.9733068+00:00",
                                            "Description": "Refined",
                                            "Name": "Tasty Steel Cheese",
                                            "Price": 17.8091496729941
                                        },
                                        {
                                            "Children": [],
                                            "Date": "2025-09-19T20:23:32.9733087+00:00",
                                            "Description": "Gorgeous",
                                            "Name": "Fantastic Metal Chips",
                                            "Price": 49.23675278909915
                                        },
                                        {
                                            "Children": [],
                                            "Date": "2025-09-30T20:23:32.9733107+00:00",
                                            "Description": "Practical",
                                            "Name": "Generic Plastic Gloves",
                                            "Price": 40.675770041676586
                                        }
                                    ],
                                    "CreateUser": {
                                        "ConfirmPassword": "",
                                        "Password": "",
                                        "UserName": "Jonny",
                                        "Addresses": [],
                                        "Company": "",
                                        "CreatedBy": null,
                                        "Department": "",
                                        "Description": "",
                                        "Email": "john.doe@mail.com",
                                        "EscalateTo": null,
                                        "Ext": "",
                                        "FirstName": "John",
                                        "FullName": "John Doe",
                                        "Gender": "",
                                        "Groups": [],
                                        "ImageID": null,
                                        "LastName": "Doe",
                                        "PhoneNumber": "",
                                        "PreferredLanguage": 0,
                                        "Rights": "",
                                        "Roles": [],
                                        "SubscriptionExpires": null,
                                        "Title": "",
                                        "UserID": "748caa80-4659-4f65-b23b-9b2aa57daffd",
                                        "Website": "",
                                        "ID": "2c974aa6-6638-466d-a44d-82399ee4d79a"
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
                                        "Accuracy": 7.102653757155252,
                                        "Altitude": 2146.1675239352385,
                                        "AltitudeReferenceSystem": 0,
                                        "Course": 205.4780901990637,
                                        "ID": "7a62fbea-acd6-435e-b3f2-ae26da526010",
                                        "InstanceID": "9f51e54e-7f1b-47cb-b75f-b2392941c0ee",
                                        "IsFromMockProvider": false,
                                        "Latitude": 18414.676462605123,
                                        "Longitude": 3548.936961772835,
                                        "Speed": 9.489828372317621,
                                        "Timestamp": "2025-09-21T20:23:32.9757092+00:00",
                                        "VerticalAccuracy": 86.06466547889896
                                    },
                                    "User": {
                                        "Addresses": [],
                                        "Company": "",
                                        "CreatedBy": null,
                                        "Department": "",
                                        "Description": "",
                                        "Email": "john.doe@mail.com",
                                        "EscalateTo": null,
                                        "Ext": "",
                                        "FirstName": "John",
                                        "FullName": "John Doe",
                                        "Gender": "",
                                        "Groups": [],
                                        "ImageID": null,
                                        "LastName": "Doe",
                                        "PhoneNumber": "",
                                        "PreferredLanguage": 0,
                                        "Rights": "",
                                        "Roles": [],
                                        "SubscriptionExpires": null,
                                        "Title": "",
                                        "UserID": "ff33e10d-df44-41fd-9a8d-21f476c694cc",
                                        "UserName": "Jonny",
                                        "Website": "",
                                        "ID": "ff121e0e-cfb9-439b-ad46-cd904a223bfa"
                                    },
                                    "email":   "bite@me.com",
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
                                        "Value": "Sunday, September 21, 2025"
                                    }
                                }
                                """;


    private static readonly TestJson               _test           = TestJson.Debug;
    private static readonly JsonSerializerSettings jsonNetSettings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };


    [GlobalSetup] public void Setup() => Debugger.Launch();


    [Benchmark] [Category("Serialize")]   public string    Serialize_TestJson_SystemTextJson()        => _test.ToJson();
    [Benchmark] [Category("Serialize")]   public string    Serialize_Pretty_TestJson_SystemTextJson() => JsonSerializer.Serialize(_test, ExperimentContext.Default.TestJson);
    [Benchmark] [Category("Deserialize")] public TestJson? Deserialize_TestJson_JsonNet()             => JsonConvert.DeserializeObject<TestJson>(JSON);
    [Benchmark] [Category("Deserialize")] public TestJson  Deserialize_TestJson_SystemTextJson()      => JSON.FromJson<TestJson>(ExperimentContext.Default);


    public static void Test()
    {
        JsonNet_SystemTextJson_Benchmarks benchmarks = new();

        using ( StopWatch.Start(nameof(Serialize_TestJson_SystemTextJson)) ) { benchmarks.Serialize_TestJson_SystemTextJson(); }

        using ( StopWatch.Start(nameof(Serialize_Pretty_TestJson_SystemTextJson)) ) { benchmarks.Serialize_Pretty_TestJson_SystemTextJson(); }

        using ( StopWatch.Start(nameof(Deserialize_TestJson_JsonNet)) ) { benchmarks.Deserialize_TestJson_JsonNet(); }

        using ( StopWatch.Start(nameof(Deserialize_TestJson_SystemTextJson)) ) { benchmarks.Deserialize_TestJson_SystemTextJson(); }
    }
}
