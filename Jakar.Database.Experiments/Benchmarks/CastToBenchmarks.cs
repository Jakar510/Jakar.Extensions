// Jakar.Extensions :: Experiments
// 03/05/2023  11:59 PM

namespace Jakar.Database.Experiments.Benchmarks;


/*
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1265/22H2/2022Update/SunValley2)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=7.0.201
  [Host]     : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2


|               Method |       Mean |     Error |    StdDev |     Median | Rank |   Gen0 | Allocated |
|--------------------- |-----------:|----------:|----------:|-----------:|-----:|-------:|----------:|
|         Cast_Decimal |         NA |        NA |        NA |         NA |    ? |      - |         - |
|    Direct_Cast_Short |  0.0013 ns | 0.0043 ns | 0.0036 ns |  0.0000 ns |    1 |      - |         - |
|     Direct_Cast_UInt |  0.0017 ns | 0.0042 ns | 0.0040 ns |  0.0000 ns |    1 |      - |         - |
|     Direct_Cast_Long |  0.0053 ns | 0.0089 ns | 0.0079 ns |  0.0004 ns |    1 |      - |         - |
|    Direct_Cast_ULong |  0.0065 ns | 0.0092 ns | 0.0086 ns |  0.0007 ns |    1 |      - |         - |
|      Direct_Cast_Int |  0.0069 ns | 0.0103 ns | 0.0091 ns |  0.0017 ns |    1 |      - |         - |
|     Direct_Cast_Byte |  0.0110 ns | 0.0106 ns | 0.0099 ns |  0.0083 ns |    1 |      - |         - |
|    Direct_Cast_SByte |  0.0113 ns | 0.0156 ns | 0.0145 ns |  0.0000 ns |    1 |      - |         - |
|   Direct_Cast_UShort |  0.0122 ns | 0.0174 ns | 0.0162 ns |  0.0032 ns |    1 |      - |         - |
|           Cast_Float |  0.4554 ns | 0.0110 ns | 0.0098 ns |  0.4515 ns |    3 |      - |         - |
|            Cast_Byte |  0.4628 ns | 0.0057 ns | 0.0047 ns |  0.4630 ns |    4 |      - |         - |
|           Cast_Short |  0.4635 ns | 0.0063 ns | 0.0049 ns |  0.4637 ns |    4 |      - |         - |
|          Cast_Double |  0.4673 ns | 0.0180 ns | 0.0168 ns |  0.4654 ns |    4 |      - |         - |
|            Cast_Long |  0.4686 ns | 0.0036 ns | 0.0032 ns |  0.4686 ns |    4 |      - |         - |
|          Cast_UShort |  0.4698 ns | 0.0072 ns | 0.0060 ns |  0.4712 ns |    4 |      - |         - |
|             Cast_Int |  0.4704 ns | 0.0158 ns | 0.0132 ns |  0.4667 ns |    4 |      - |         - |
|           Cast_SByte |  0.4744 ns | 0.0154 ns | 0.0136 ns |  0.4683 ns |    4 |      - |         - |
|           Cast_ULong |  0.4811 ns | 0.0170 ns | 0.0150 ns |  0.4757 ns |    4 |      - |         - |
|            Cast_UInt |  0.4813 ns | 0.0144 ns | 0.0127 ns |  0.4849 ns |    4 |      - |         - |
|     Convert_Cast_Int | 14.6146 ns | 0.2721 ns | 0.3239 ns | 14.4679 ns |    5 | 0.0057 |      48 B |
|   Convert_Cast_SByte | 15.4363 ns | 0.3298 ns | 0.4050 ns | 15.4846 ns |    6 | 0.0057 |      48 B |
|   Convert_Cast_ULong | 15.8487 ns | 0.3006 ns | 0.2952 ns | 15.7553 ns |    7 | 0.0057 |      48 B |
|    Convert_Cast_UInt | 15.9304 ns | 0.3476 ns | 0.4519 ns | 15.8726 ns |    7 | 0.0057 |      48 B |
|  Convert_Cast_UShort | 16.5318 ns | 0.2924 ns | 0.2592 ns | 16.5622 ns |    8 | 0.0057 |      48 B |
|   Convert_Cast_Short | 16.7063 ns | 0.3574 ns | 0.4116 ns | 16.6977 ns |    8 | 0.0057 |      48 B |
|    Convert_Cast_Long | 17.3874 ns | 0.2886 ns | 0.2559 ns | 17.3700 ns |    9 | 0.0057 |      48 B |
|    Convert_Cast_Byte | 23.3791 ns | 0.3680 ns | 0.3443 ns | 23.4378 ns |   10 | 0.0057 |      48 B |
*/
#pragma warning disable CA1822
[Config(typeof(BenchmarkConfig))]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[SimpleJob(RuntimeMoniker.HostProcess)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[MemoryDiagnoser]
public class CastToBenchmarks
{
    // ReSharper disable once ConvertToConstant.Global

    public static SupportedLanguage Value => SupportedLanguage.English;


    [BenchmarkCategory("Convert")] [Benchmark] public byte   Convert_Cast_Byte()   => Convert.ToByte(Value);
    [BenchmarkCategory("Convert")] [Benchmark] public sbyte  Convert_Cast_SByte()  => Convert.ToSByte(Value);
    [BenchmarkCategory("Convert")] [Benchmark] public short  Convert_Cast_Short()  => Convert.ToInt16(Value);
    [BenchmarkCategory("Convert")] [Benchmark] public ushort Convert_Cast_UShort() => Convert.ToUInt16(Value);
    [BenchmarkCategory("Convert")] [Benchmark] public int    Convert_Cast_Int()    => Convert.ToInt32(Value);
    [BenchmarkCategory("Convert")] [Benchmark] public uint   Convert_Cast_UInt()   => Convert.ToUInt32(Value);
    [BenchmarkCategory("Convert")] [Benchmark] public long   Convert_Cast_Long()   => Convert.ToInt32(Value);
    [BenchmarkCategory("Convert")] [Benchmark] public ulong  Convert_Cast_ULong()  => Convert.ToUInt64(Value);


    [BenchmarkCategory("Cast")] [Benchmark] public byte   Cast_Byte()   => Value.AsByte();
    [BenchmarkCategory("Cast")] [Benchmark] public sbyte  Cast_SByte()  => Value.AsSByte();
    [BenchmarkCategory("Cast")] [Benchmark] public short  Cast_Short()  => Value.AsShort();
    [BenchmarkCategory("Cast")] [Benchmark] public ushort Cast_UShort() => Value.AsUShort();
    [BenchmarkCategory("Cast")] [Benchmark] public int    Cast_Int()    => Value.AsInt();
    [BenchmarkCategory("Cast")] [Benchmark] public uint   Cast_UInt()   => Value.AsUInt();
    [BenchmarkCategory("Cast")] [Benchmark] public long   Cast_Long()   => Value.AsLong();
    [BenchmarkCategory("Cast")] [Benchmark] public ulong  Cast_ULong()  => Value.AsULong();


    [BenchmarkCategory("DirectCast")] [Benchmark] public byte   Direct_Cast_Byte()   => (byte)Value;
    [BenchmarkCategory("DirectCast")] [Benchmark] public sbyte  Direct_Cast_SByte()  => (sbyte)Value;
    [BenchmarkCategory("DirectCast")] [Benchmark] public short  Direct_Cast_Short()  => (short)Value;
    [BenchmarkCategory("DirectCast")] [Benchmark] public ushort Direct_Cast_UShort() => (ushort)Value;
    [BenchmarkCategory("DirectCast")] [Benchmark] public int    Direct_Cast_Int()    => (int)Value;
    [BenchmarkCategory("DirectCast")] [Benchmark] public uint   Direct_Cast_UInt()   => (uint)Value;
    [BenchmarkCategory("DirectCast")] [Benchmark] public long   Direct_Cast_Long()   => (long)Value;
    [BenchmarkCategory("DirectCast")] [Benchmark] public ulong  Direct_Cast_ULong()  => (ulong)Value;
}



#pragma warning restore CA1822
