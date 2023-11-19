﻿// Jakar.Extensions :: Console.Experiments
// 05/10/2022  10:26 AM

using System.Runtime.InteropServices;
using BenchmarkDotNet.Configs;



namespace Experiments.Benchmarks;


/*
 */



[ SimpleJob( RuntimeMoniker.HostProcess ), Orderer( SummaryOrderPolicy.FastestToSlowest ), RankColumn, MemoryDiagnoser ]
public class SpansBenchmarks
{
    private const string ALPHABET  = "abcdefghijklmnopqrstuvwxyz";
    private const string NEW_VALUE = "----NEW-VALUE";
    private const string OLD       = "0365BC9B";
    private const string TEST      = $"{ALPHABET}__{OLD}-3DE3-4B75-9F7E-2A0F23EFA5A2";


    [ Params( "", ALPHABET, TEST ) ] public string Value { get; set; } = string.Empty;


    [ Benchmark ] public bool Contains_span()  => Value.Contains( '2' );
    [ Benchmark ] public bool Contains_value() => Spans.Contains( Value, NEW_VALUE );
    [ Benchmark ]
    public bool ContainsAny()
    {
        ReadOnlySpan<char> buffer = stackalloc char[4]
        {
            '1',
            '3',
            'F',
            'A'
        };

        return Spans.ContainsAny( Value, buffer );
    }
    [ Benchmark ]
    public bool ContainsNone()
    {
        ReadOnlySpan<char> buffer = stackalloc char[4]
        {
            '1',
            '3',
            'F',
            'A'
        };

        return Spans.ContainsNone( Value, buffer );
    }
    [ Benchmark ] public bool EndsWith()           => Spans.EndsWith( Value, '1' );
    [ Benchmark ] public bool IsNullOrWhiteSpace() => Spans.IsNullOrWhiteSpace( Value );
    [ Benchmark ] public bool StartsWith()         => Spans.StartsWith( Value, '1' );


    [ Benchmark ]
    public ReadOnlySpan<char> AsSpan()
    {
        ReadOnlySpan<char> span = Value;
        return span.AsSpan();
    }
    [ Benchmark ] public ReadOnlySpan<char> Join() => Spans.Join<char>( Value, NEW_VALUE );
    [ Benchmark ]
    public ReadOnlySpan<char> RemoveAll_Params()
    {
        Span<char> span = stackalloc char[Value.Length];
        Value.CopyTo( span );

        ReadOnlySpan<char> buffer = stackalloc char[4]
        {
            '1',
            '3',
            'F',
            'A'
        };

        Span<char> result = span.RemoveAll( buffer );
        return MemoryMarshal.CreateReadOnlySpan( ref result.GetPinnableReference(), result.Length );
    }
    [ Benchmark ] public ReadOnlySpan<char> RemoveAll_Single() => Spans.RemoveAll( Value, '1' );
    [ Benchmark ] public ReadOnlySpan<char> Replace()          => Spans.Replace<char>( Value, OLD, NEW_VALUE );
    [ Benchmark ] public ReadOnlySpan<char> Slice_False()      => Spans.Slice( Value, 'z', '4', false );
    [ Benchmark ] public ReadOnlySpan<char> Slice_True()       => Spans.Slice( Value, 'z', '4', true );
}
