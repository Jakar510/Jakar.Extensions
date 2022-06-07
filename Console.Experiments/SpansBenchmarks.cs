// Jakar.Extensions :: Console.Experiments
// 05/10/2022  10:26 AM

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Jakar.Extensions.SpanAndMemory;
using JetBrains.Annotations;



#nullable enable
namespace Console.Experiments;


/*
 */
[SimpleJob(RuntimeMoniker.HostProcess)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[MemoryDiagnoser]
public class SpansBenchmarks
{
    private const string OLD       = "0365BC9B";
    private const string ALPHABET  = "abcdefghijklmnopqrstuvwxyz";
    private const string TEST      = $"{ALPHABET}__{OLD}-3DE3-4B75-9F7E-2A0F23EFA5A2";
    private const string NEW_VALUE = "----NEW-VALUE";


    [Params("", ALPHABET, TEST)] public string Value { get; set; } = string.Empty;


    [Benchmark] public ReadOnlySpan<char> AsSpan() => ( (ReadOnlySpan<char>)Value ).AsSpan();
    [Benchmark] public ReadOnlySpan<char> RemoveAll_Single() => Spans.RemoveAll(Value, '1');
    [Benchmark] public ReadOnlySpan<char> RemoveAll_Params() => Spans.RemoveAll(Value, '1', '3', 'F', 'A');
    [Benchmark] public bool ContainsAny() => Spans.ContainsAny(Value, '1', '3', 'F', 'A');
    [Benchmark] public bool ContainsNone() => Spans.ContainsNone(Value, '1', '3', 'F', 'A');
    [Benchmark] public bool StartsWith() => Spans.StartsWith(Value, '1');
    [Benchmark] public bool EndsWith() => Spans.EndsWith(Value, '1');
    [Benchmark] public bool Contains_span() => Spans.Contains(Value,  '2');
    [Benchmark] public bool Contains_value() => Spans.Contains(Value, NEW_VALUE);
    [Benchmark] public bool IsNullOrWhiteSpace() => Spans.IsNullOrWhiteSpace(Value);
    [Benchmark] public ReadOnlySpan<char> Replace() => Spans.Replace<char>(Value, OLD, NEW_VALUE);
    [Benchmark] public ReadOnlySpan<char> Join() => Spans.Join<char>(Value, NEW_VALUE);
    [Benchmark] public ReadOnlySpan<char> Slice_False() => Spans.Slice(Value, 'z', '4', false);
    [Benchmark] public ReadOnlySpan<char> Slice_True() => Spans.Slice(Value,  'z', '4', true);
}
