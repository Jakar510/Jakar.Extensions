// dotnet run -c Release -- --bench [BenchmarkDotNet arguments, e.g. --filter *JakarJson*]

if ( args.Length > 0 && args[0] == "--bench" )
{
    BenchmarkSwitcher.FromAssembly(typeof(JakarJson_Benchmarks).Assembly).Run(args[1..]);
    return;
}

UserModel user = new();
string    json = user.ToJson();
Console.WriteLine(json);
