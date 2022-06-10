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
 
|             Method |       Mean |     Error |    StdDev | Rank | Allocated |
|------------------- |-----------:|----------:|----------:|-----:|----------:|
|            GetName |  0.1999 ns | 0.0546 ns | 0.0510 ns |    1 |         - |
|         GetContext | 29.6088 ns | 0.5710 ns | 0.7623 ns |    2 |         - |
| GetValueReflection | 48.2611 ns | 1.0202 ns | 1.5271 ns |    3 |         - |
|           GetValue | 49.5775 ns | 0.4678 ns | 0.4147 ns |    4 |         - |

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
