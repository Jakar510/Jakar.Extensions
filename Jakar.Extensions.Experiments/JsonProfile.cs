// Jakar.Extensions :: Experiments
// 10/13/2026

using System.Linq.Expressions;
using System.Reflection;



namespace Experiments;


/// <summary>
///     Runs one <see cref="JakarJson_Benchmarks"/> method in a tight loop for a sampling profiler (dotnet-trace), with the benchmark's own setup:
///     <c>dotnet-trace collect --profile cpu-sampling -o x.nettrace -- Experiments.exe --profile JakarJson_FromUtf8 7500 10</c>, then <c>dotnet-trace report x.nettrace topN -n 40</c>.
/// </summary>
public static class JsonProfile
{
    /// <param name="args"> <c>[method] [lines = 7500] [seconds = 10]</c>; no method lists them. </param>
    public static void Run( string[] args )
    {
        MethodInfo[] methods = [.. typeof(JakarJson_Benchmarks).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(static m => m.GetCustomAttributes(typeof(BenchmarkAttribute), false).Length > 0)];

        if ( args.Length == 0 )
        {
            foreach ( MethodInfo method in methods.OrderBy(static m => m.Name) ) { Console.WriteLine(method.Name); }

            return;
        }

        MethodInfo target = methods.Single(m => m.Name == args[0]);
        int        lines  = args.Length > 1 ? int.Parse(args[1]) : 7_500;
        double     limit  = args.Length > 2 ? double.Parse(args[2]) : 10;

        JakarJson_Benchmarks benchmark = new() { Lines = lines };
        benchmark.Setup();

        // A compiled call (boxing value results), so the loop costs one delegate call per iteration and no reflection while sampling.
        Func<object?> call = Expression.Lambda<Func<object?>>(Expression.Convert(Expression.Call(Expression.Constant(benchmark), target), typeof(object))).Compile();

        for ( int i = 0; i < 200; i++ ) { call(); } // reach tier-1 code before the profiler's window matters

        Stopwatch timer = Stopwatch.StartNew();
        long      count = 0;

        while ( timer.Elapsed.TotalSeconds < limit )
        {
            for ( int i = 0; i < 64; i++ ) { GC.KeepAlive(call()); }

            count += 64;
        }

        Console.WriteLine($"{target.Name} [Lines={lines}]: {count:N0} calls, {timer.Elapsed.TotalMilliseconds * 1_000_000 / count:N0} ns/call");
    }

}
