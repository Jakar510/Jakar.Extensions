// Jakar.Extensions :: Console.Experiments
// 06/10/2022  3:48 PM

using System;
using System.Collections.Generic;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Jakar.Extensions.Strings;
using Jakar.Mapper;



namespace Console.Experiments;


/*
 
|             Method |           Mean |         Error |        StdDev | Rank |   Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|------------------- |---------------:|--------------:|--------------:|-----:|--------:|-------:|-------:|----------:|
|            GetName |       3.630 ns |     0.0366 ns |     0.0343 ns |    1 |       - |      - |      - |         - |
|           GetValue |      32.182 ns |     0.3599 ns |     0.3366 ns |    2 |       - |      - |      - |         - |
| GetValueReflection |      86.913 ns |     1.7248 ns |     1.7712 ns |    3 |       - |      - |      - |         - |
|         GetContext | 153,028.868 ns | 2,932.6377 ns | 2,880.2423 ns |    4 | 10.9863 | 5.3711 | 0.2441 |  93,000 B |

*/



[SimpleJob(RuntimeMoniker.HostProcess)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[MemoryDiagnoser]
public class MapperBenchmarks
{
    private static readonly Node         _nodes        = Node.Demo();
    private static readonly Type         _nodeType     = typeof(Node);
    private static readonly PropertyInfo _nameProperty = _nodeType.GetProperty(nameof(Node.Name))!;
    private static readonly MethodInfo   _getName      = _nameProperty.GetMethod!;


    [Benchmark] public string GetName() => _nodes.Name;
    [Benchmark] public MContext<Node> GetContext() => MContext.GetContext(_nodes);
    [Benchmark] public object? GetValue() => MContext.GetContext(_nodes).GetValue(nameof(Node.Name));
    [Benchmark] public object? GetValueReflection() => _getName.Invoke(_nodes, default);

    // [Benchmark] public Func<object, object?> Create_GetMethod() => MContext.Create_GetMethod(_nameProperty);
}
