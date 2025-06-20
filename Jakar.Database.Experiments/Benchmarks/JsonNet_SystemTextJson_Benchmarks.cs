using Jakar.Extensions;



namespace Jakar.Database.Experiments.Benchmarks;


#pragma warning disable CA1822 // Mark members as static
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



[Config( typeof(BenchmarkConfig) ), GroupBenchmarksBy( BenchmarkLogicalGroupRule.ByCategory ), SimpleJob( RuntimeMoniker.HostProcess ), Orderer( SummaryOrderPolicy.FastestToSlowest ), RankColumn, MemoryDiagnoser, SuppressMessage( "ReSharper", "InconsistentNaming" )]
public class JsonNet_SystemTextJson_Benchmarks
{
    private const string JSON = """
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
    private static readonly Node _node = JSON.FromJson<Node>();


    [Benchmark, Category( "Serialize" )] public string? ToStringSerialize()             => _node.ToString();
    [Benchmark, Category( "Serialize" )] public string? JsonNetSerialize()              => _node.ToJson();
    [Benchmark, Category( "Serialize" )] public string? JsonNetSerializePretty()        => _node.ToPrettyJson();
    [Benchmark, Category( "Serialize" )] public string? SystemTextJsonSerialize()       => JsonSerializer.Serialize( _node, NodeContext.Default.Node );
    [Benchmark, Category( "Serialize" )] public string? SystemTextJsonSerializePretty() => JsonSerializer.Serialize( _node, NodeContext.Pretty );


    [Benchmark, Category( "Deserialize" )] public Node? FakerDeserialize()          => NodeFaker.Instance.Generate();
    [Benchmark, Category( "Deserialize" )] public Node? JsonNetDeserialize()        => JSON.FromJson<Node>();
    [Benchmark, Category( "Deserialize" )] public Node? SystemTextJsonDeserialize() => JsonSerializer.Deserialize( JSON, NodeContext.Default.Node );


    public static async ValueTask SaveAsync()
    {
        using TelemetrySpan span = TelemetrySpan.Create();
        LocalFile           file = "test.json";
        await file.WriteAsync( JSON );
        Console.WriteLine( file.FullPath );
    }
}



public sealed class NodeFaker : Faker<Node>
{
    public static readonly NodeFaker Instance = new();
    private                uint      _depth   = (uint)Random.Shared.Next( 5, 10 );


    public NodeFaker()
    {
        RuleFor( x => x.Name,        f => f.Commerce.ProductName() );
        RuleFor( x => x.Description, f => f.Commerce.ProductAdjective() );
        RuleFor( x => x.Price,       f => f.Random.Double( 1, 100 ) );
        RuleFor( x => x.Date,        f => DateTimeOffset.UtcNow - TimeSpan.FromDays( f.Random.Int( -10, 10 ) ) );
        RuleFor( x => x.Children,    GetChildren );
    }
    private Node[] GetChildren( Faker f )
    {
        if ( _depth > 0 )
        {
            uint depth = _depth;
            _depth = depth / 2;
            return Generate( (int)depth ).ToArray();
        }

        return [];
    }
}



public sealed record Node
{
    public Node[]         Children    { get; init; } = [];
    public DateTimeOffset Date        { get; init; }
    public string         Description { get; init; } = string.Empty;
    public string         Name        { get; init; } = string.Empty;
    public double         Price       { get; init; }

    // [ JsonExtensionData ]                                public Dictionary<string, JToken>?      JsonExtensionData { get; set; }
    // [ System.Text.Json.Serialization.JsonExtensionData ] public JsonObject? ExtensionData     { get; set; }


    public Node() { }
    public Node( string name, string value, double price, DateTimeOffset date, params Node[] children )
    {
        Name        = name;
        Description = value;
        Price       = price;
        Date        = date;
        Children    = children;
    }
}



[JsonSerializable( typeof(Node) )]
public partial class NodeContext : JsonSerializerContext
{
    public static JsonSerializerOptions Pretty => new()
                                                  {
                                                      WriteIndented               = true,
                                                      AllowTrailingCommas         = true,
                                                      PropertyNameCaseInsensitive = true,
                                                      IncludeFields               = false,
                                                      TypeInfoResolver            = Default
                                                  };
}



#pragma warning restore CA1822 // Mark members as static
