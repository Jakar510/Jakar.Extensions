// Jakar.Extensions :: Console.Experiments
// 05/02/2022  4:11 PM

using System.Security.Cryptography;


#nullable enable
namespace Console.Experiments;


/*
GetObject()
|               Method |        Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|--------------------- |------------:|----------:|----------:|-----:|-------:|----------:|
| Create_StringBuilder |    10.61 ns |  0.267 ns |  0.366 ns |    1 | 0.0124 |     104 B |
|      Jsonizer_ToJson |   590.33 ns |  8.061 ns |  8.625 ns |    2 | 0.1392 |   1,168 B |
|       JsonNet_ToJson | 1,211.94 ns |  9.424 ns |  8.815 ns |    3 | 0.2232 |   1,880 B |
|     JsonNet_FromJson | 1,548.61 ns | 19.588 ns | 18.322 ns |    4 | 0.3452 |   2,888 B |



GetObjects()
|               Method |         Mean |       Error |     StdDev | Rank |  Gen 0 |  Gen 1 | Allocated |
|--------------------- |-------------:|------------:|-----------:|-----:|-------:|-------:|----------:|
| Create_StringBuilder |     8.517 ns |   0.1071 ns |  0.1002 ns |    1 | 0.0124 |      - |     104 B |
|      Jsonizer_ToJson | 3,621.338 ns |  45.0247 ns | 39.9132 ns |    2 | 1.0567 | 0.0191 |   8,856 B |
|       JsonNet_ToJson | 6,555.885 ns |  51.7494 ns | 45.8745 ns |    3 | 0.6638 | 0.0076 |   5,608 B |
|     JsonNet_FromJson | 8,667.429 ns | 102.1275 ns | 95.5301 ns |    4 | 0.5035 |      - |   4,248 B |



GetManyObjects()
|           Method |    Mean |    Error |   StdDev | Rank |       Gen 0 |      Gen 1 |     Gen 2 | Allocated |
|----------------- |--------:|---------:|---------:|-----:|------------:|-----------:|----------:|----------:|
|  Jsonizer_ToJson | 1.920 s | 0.0344 s | 0.0287 s |    1 | 111000.0000 | 56000.0000 | 1000.0000 |  2,301 MB |
|   JsonNet_ToJson | 2.471 s | 0.0223 s | 0.0198 s |    2 |  98000.0000 | 33000.0000 | 1000.0000 |  1,341 MB |
| JsonNet_FromJson | 3.413 s | 0.0410 s | 0.0364 s |    3 |  64000.0000 | 23000.0000 |         - |    518 MB |



GetObjects()   --- Manually serializing JsonNet.ToJson ----
|           Method |     Mean |     Error |    StdDev | Rank |  Gen 0 |  Gen 1 | Allocated |
|----------------- |---------:|----------:|----------:|-----:|-------:|-------:|----------:|
|  Jsonizer_ToJson | 4.061 us | 0.0376 us | 0.0333 us |    1 | 0.5951 | 0.0076 |      5 KB |
|   JsonNet_ToJson | 6.793 us | 0.0686 us | 0.0641 us |    2 | 0.6638 | 0.0076 |      5 KB |
| JsonNet_FromJson | 8.914 us | 0.0858 us | 0.0717 us |    3 | 0.5035 |      - |      4 KB |
 */
[SimpleJob(RuntimeMoniker.HostProcess)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[MemoryDiagnoser]
public class JsonizerBenchmarks
{
    private static readonly RandomNumberGenerator _random;
    private static readonly Test                  _source;
    private static readonly string                _json;
    static JsonizerBenchmarks()
    {
        _random = RandomNumberGenerator.Create();
        _source = GetObjects();
        _json = _source.ToJson(Formatting.Indented);
    }


    public static string GenerateToken( in int length = 32 )
    {
        var randomNumber = new byte[length];
        _random.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }


    public static Test GetManyObjects()
    {
        var children = new List<Test>(100);

        for ( var i = 0; i < 5000; i++ )
        {
            int size   = Test.random.Next(250, 500);
            var nested = new List<Test>(size);

            for ( var j = 0; j < size; j++ ) { nested.Add(new Test(GenerateToken())); }

            children.Add(new Test(GenerateToken(), nested));
        }

        var test = new Test("Root", children);
        return test;
    }
    public static Test GetObjects()
    {
        var first  = new Test("First",  new Test("1.1"), new Test("1.2"));
        var second = new Test("Second", new Test("2.1"));
        var test   = new Test("Root",   first, second);
        return test;
    }
    public static Test GetObject() => new(string.Empty);


    [Benchmark] public Test JsonNet_FromJson() => _json.FromJson<Test>();
    [Benchmark] public string JsonNet_ToJson() => _source.ToJson(Formatting.Indented);
    [Benchmark] public string Jsonizer_ToJson() => _source.ToJson();


    // [Benchmark] public StringBuilder Create_StringBuilder() => new();
}
