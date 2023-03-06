// Jakar.Extensions :: Experiments
// 03/05/2023  11:59 PM

namespace Experiments;


/*
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1265/22H2/2022Update/SunValley2)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=7.0.201
  [Host]     : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2


|               Method |       Mean |     Error |    StdDev | Rank |   Gen0 | Allocated |
|--------------------- |-----------:|----------:|----------:|-----:|-------:|----------:|
|         Cast_Decimal |         NA |        NA |        NA |    ? |      - |         - |
|            Cast_Byte |  0.4679 ns | 0.0099 ns | 0.0088 ns |    1 |      - |         - |
|            Cast_UInt |  0.4682 ns | 0.0039 ns | 0.0034 ns |    1 |      - |         - |
|           Cast_SByte |  0.4719 ns | 0.0133 ns | 0.0118 ns |    1 |      - |         - |
|             Cast_Int |  0.4757 ns | 0.0223 ns | 0.0209 ns |    1 |      - |         - |
|           Cast_Float |  0.6963 ns | 0.0153 ns | 0.0136 ns |    2 |      - |         - |
|          Cast_UShort |  0.6971 ns | 0.0161 ns | 0.0135 ns |    2 |      - |         - |
|           Cast_Short |  0.6986 ns | 0.0102 ns | 0.0085 ns |    2 |      - |         - |
|           Cast_ULong |  0.7205 ns | 0.0289 ns | 0.0270 ns |    2 |      - |         - |
|          Cast_Double |  0.7344 ns | 0.0183 ns | 0.0171 ns |    2 |      - |         - |
|            Cast_Long |  0.7450 ns | 0.0412 ns | 0.0385 ns |    2 |      - |         - |
|   Convert_Cast_Float | 14.9148 ns | 0.1952 ns | 0.1730 ns |    3 | 0.0057 |      48 B |
|   Convert_Cast_Short | 15.3025 ns | 0.3313 ns | 0.3544 ns |    4 | 0.0057 |      48 B |
|  Convert_Cast_Double | 15.5998 ns | 0.3254 ns | 0.2885 ns |    4 | 0.0057 |      48 B |
| Convert_Cast_Decimal | 15.9660 ns | 0.3458 ns | 0.3234 ns |    5 | 0.0057 |      48 B |
|   Convert_Cast_ULong | 16.0299 ns | 0.3289 ns | 0.3378 ns |    5 | 0.0057 |      48 B |
|    Convert_Cast_UInt | 16.3756 ns | 0.3510 ns | 0.4042 ns |    5 | 0.0057 |      48 B |
|  Convert_Cast_UShort | 16.4035 ns | 0.3333 ns | 0.3117 ns |    5 | 0.0057 |      48 B |
|     Convert_Cast_Int | 16.4593 ns | 0.2638 ns | 0.2467 ns |    5 | 0.0057 |      48 B |
|    Convert_Cast_Long | 16.7381 ns | 0.3544 ns | 0.2960 ns |    5 | 0.0057 |      48 B |
|   Convert_Cast_SByte | 16.9971 ns | 0.2982 ns | 0.2790 ns |    5 | 0.0057 |      48 B |
|    Convert_Cast_Byte | 20.2118 ns | 0.4003 ns | 0.4610 ns |    6 | 0.0057 |      48 B |
 */
[SimpleJob( RuntimeMoniker.HostProcess )]
[Orderer( SummaryOrderPolicy.FastestToSlowest )]
[RankColumn]
[MemoryDiagnoser]
public class CastToBenchmarks
{
    public const SupportedLanguage LANGUAGE = SupportedLanguage.English;


    [Benchmark] public byte Convert_Cast_Byte() => Convert.ToByte( LANGUAGE );
    [Benchmark] public sbyte Convert_Cast_SByte() => Convert.ToSByte( LANGUAGE );
    [Benchmark] public short Convert_Cast_Short() => Convert.ToInt16( LANGUAGE );
    [Benchmark] public ushort Convert_Cast_UShort() => Convert.ToUInt16( LANGUAGE );
    [Benchmark] public int Convert_Cast_Int() => Convert.ToInt32( LANGUAGE );
    [Benchmark] public uint Convert_Cast_UInt() => Convert.ToUInt32( LANGUAGE );
    [Benchmark] public long Convert_Cast_Long() => Convert.ToInt32( LANGUAGE );
    [Benchmark] public ulong Convert_Cast_ULong() => Convert.ToUInt64( LANGUAGE );
    [Benchmark] public float Convert_Cast_Float() => Convert.ToSingle( LANGUAGE );
    [Benchmark] public double Convert_Cast_Double() => Convert.ToDouble( LANGUAGE );
    [Benchmark] public decimal Convert_Cast_Decimal() => Convert.ToDecimal( LANGUAGE );


    [Benchmark] public byte Cast_Byte() => LANGUAGE.AsByte();
    [Benchmark] public sbyte Cast_SByte() => LANGUAGE.AsSByte();
    [Benchmark] public short Cast_Short() => LANGUAGE.AsShort();
    [Benchmark] public ushort Cast_UShort() => LANGUAGE.AsUShort();
    [Benchmark] public int Cast_Int() => LANGUAGE.AsInt();
    [Benchmark] public uint Cast_UInt() => LANGUAGE.AsUInt();
    [Benchmark] public long Cast_Long() => LANGUAGE.AsLong();
    [Benchmark] public ulong Cast_ULong() => LANGUAGE.AsULong();
    [Benchmark] public float Cast_Float() => LANGUAGE.AsFloat();
    [Benchmark] public double Cast_Double() => LANGUAGE.AsDouble();
    [Benchmark] public decimal Cast_Decimal() => LANGUAGE.AsDecimal();
}
