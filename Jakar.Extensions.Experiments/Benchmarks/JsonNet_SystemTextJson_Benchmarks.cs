using System.Diagnostics;
using Newtonsoft.Json;



namespace Jakar.Extensions.Experiments.Benchmarks;


/*


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
    private static readonly JsonSerializerSettings jsonNetSettings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize };


    [GlobalSetup] public void Setup() => Debugger.Launch();


    [Benchmark] [Category("Serialize")]   public string    Serialize_TestJson_SystemTextJson()   => _test.ToJson();
    [Benchmark] [Category("Serialize")]   public string    Serialize_TestJson_JsonNet()          => JsonConvert.SerializeObject(_test, Formatting.Indented);
    [Benchmark] [Category("Deserialize")] public TestJson? Deserialize_TestJson_JsonNet()        => JsonConvert.DeserializeObject<TestJson>(JSON);
    [Benchmark] [Category("Deserialize")] public TestJson  Deserialize_TestJson_SystemTextJson() => JSON.FromJson<TestJson>();


    public static void Test()
    {
        JsonNet_SystemTextJson_Benchmarks benchmarks = new();

        using ( StopWatch.Start(nameof(Serialize_TestJson_SystemTextJson)) ) { benchmarks.Serialize_TestJson_SystemTextJson(); }

        using ( StopWatch.Start(nameof(Serialize_TestJson_JsonNet)) ) { benchmarks.Serialize_TestJson_JsonNet(); }

        using ( StopWatch.Start(nameof(Deserialize_TestJson_JsonNet)) ) { benchmarks.Deserialize_TestJson_JsonNet(); }

        using ( StopWatch.Start(nameof(Deserialize_TestJson_SystemTextJson)) ) { benchmarks.Deserialize_TestJson_SystemTextJson(); }
    }
}
