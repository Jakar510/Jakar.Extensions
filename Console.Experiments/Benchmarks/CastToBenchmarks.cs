// Jakar.Extensions :: Experiments
// 03/05/2023  11:59 PM

namespace Experiments.Benchmarks;


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
|    Direct_Cast_Float |  0.0103 ns | 0.0112 ns | 0.0100 ns |  0.0075 ns |    1 |      - |         - |
|     Direct_Cast_Byte |  0.0110 ns | 0.0106 ns | 0.0099 ns |  0.0083 ns |    1 |      - |         - |
|    Direct_Cast_SByte |  0.0113 ns | 0.0156 ns | 0.0145 ns |  0.0000 ns |    1 |      - |         - |
|   Direct_Cast_UShort |  0.0122 ns | 0.0174 ns | 0.0162 ns |  0.0032 ns |    1 |      - |         - |
|  Direct_Cast_Decimal |  0.0160 ns | 0.0088 ns | 0.0078 ns |  0.0174 ns |    1 |      - |         - |
|   Direct_Cast_Double |  0.0272 ns | 0.0164 ns | 0.0153 ns |  0.0275 ns |    2 |      - |         - |
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
|  Convert_Cast_Double | 15.9265 ns | 0.3421 ns | 0.4326 ns | 16.0037 ns |    7 | 0.0057 |      48 B |
| Convert_Cast_Decimal | 15.9284 ns | 0.2323 ns | 0.2173 ns | 15.9601 ns |    7 | 0.0057 |      48 B |
|    Convert_Cast_UInt | 15.9304 ns | 0.3476 ns | 0.4519 ns | 15.8726 ns |    7 | 0.0057 |      48 B |
|   Convert_Cast_Float | 16.0535 ns | 0.3500 ns | 0.3890 ns | 15.9020 ns |    7 | 0.0057 |      48 B |
|  Convert_Cast_UShort | 16.5318 ns | 0.2924 ns | 0.2592 ns | 16.5622 ns |    8 | 0.0057 |      48 B |
|   Convert_Cast_Short | 16.7063 ns | 0.3574 ns | 0.4116 ns | 16.6977 ns |    8 | 0.0057 |      48 B |
|    Convert_Cast_Long | 17.3874 ns | 0.2886 ns | 0.2559 ns | 17.3700 ns |    9 | 0.0057 |      48 B |
|    Convert_Cast_Byte | 23.3791 ns | 0.3680 ns | 0.3443 ns | 23.4378 ns |   10 | 0.0057 |      48 B |
*/
[ SimpleJob( RuntimeMoniker.HostProcess ), Orderer( SummaryOrderPolicy.FastestToSlowest ), RankColumn, MemoryDiagnoser ]
public class CastToBenchmarks
{
    // ReSharper disable once ConvertToConstant.Global

    public static SupportedLanguage Value => SupportedLanguage.English;


    [ Benchmark ] public byte    Convert_Cast_Byte()    => Convert.ToByte( Value );
    [ Benchmark ] public sbyte   Convert_Cast_SByte()   => Convert.ToSByte( Value );
    [ Benchmark ] public short   Convert_Cast_Short()   => Convert.ToInt16( Value );
    [ Benchmark ] public ushort  Convert_Cast_UShort()  => Convert.ToUInt16( Value );
    [ Benchmark ] public int     Convert_Cast_Int()     => Convert.ToInt32( Value );
    [ Benchmark ] public uint    Convert_Cast_UInt()    => Convert.ToUInt32( Value );
    [ Benchmark ] public long    Convert_Cast_Long()    => Convert.ToInt32( Value );
    [ Benchmark ] public ulong   Convert_Cast_ULong()   => Convert.ToUInt64( Value );
    [ Benchmark ] public float   Convert_Cast_Float()   => Convert.ToSingle( Value );
    [ Benchmark ] public double  Convert_Cast_Double()  => Convert.ToDouble( Value );
    [ Benchmark ] public decimal Convert_Cast_Decimal() => Convert.ToDecimal( Value );


    [ Benchmark ] public byte    Cast_Byte()    => Value.AsByte();
    [ Benchmark ] public sbyte   Cast_SByte()   => Value.AsSByte();
    [ Benchmark ] public short   Cast_Short()   => Value.AsShort();
    [ Benchmark ] public ushort  Cast_UShort()  => Value.AsUShort();
    [ Benchmark ] public int     Cast_Int()     => Value.AsInt();
    [ Benchmark ] public uint    Cast_UInt()    => Value.AsUInt();
    [ Benchmark ] public long    Cast_Long()    => Value.AsLong();
    [ Benchmark ] public ulong   Cast_ULong()   => Value.AsULong();
    [ Benchmark ] public float   Cast_Float()   => Value.AsFloat();
    [ Benchmark ] public double  Cast_Double()  => Value.AsDouble();
    [ Benchmark ] public decimal Cast_Decimal() => Value.AsDecimal();


    [ Benchmark ] public byte    Direct_Cast_Byte()    => (byte)Value;
    [ Benchmark ] public sbyte   Direct_Cast_SByte()   => (sbyte)Value;
    [ Benchmark ] public short   Direct_Cast_Short()   => (short)Value;
    [ Benchmark ] public ushort  Direct_Cast_UShort()  => (ushort)Value;
    [ Benchmark ] public int     Direct_Cast_Int()     => (int)Value;
    [ Benchmark ] public uint    Direct_Cast_UInt()    => (uint)Value;
    [ Benchmark ] public long    Direct_Cast_Long()    => (long)Value;
    [ Benchmark ] public ulong   Direct_Cast_ULong()   => (ulong)Value;
    [ Benchmark ] public float   Direct_Cast_Float()   => (float)Value;
    [ Benchmark ] public double  Direct_Cast_Double()  => (double)Value;
    [ Benchmark ] public decimal Direct_Cast_Decimal() => (decimal)Value;
}
