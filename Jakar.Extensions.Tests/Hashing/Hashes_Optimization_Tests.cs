// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Buffers;
using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


/// <summary> Pins the outputs of the optimized <see cref="Hashes"/> paths to the BCL (and to the previous formats). </summary>
[TestFixture]
[TestOf(typeof(Hashes))]
public class Hashes_Optimization_Tests : Assert
{
    private static readonly string __long = new('x', 5000); // larger than the stack buffer, exercises the pooled path


    [TestCase("")] [TestCase("abc")] [TestCase("héllo wörld ✓")] public void HashStrings_MatchTheBcl( string input )
    {
        byte[] bytes = Encoding.Default.GetBytes(input);

        this.AreEqual(Convert.ToHexString(MD5.HashData(bytes)),    input.Hash_MD5());
        this.AreEqual(Convert.ToHexString(SHA1.HashData(bytes)),   input.Hash_SHA1());
        this.AreEqual(Convert.ToHexString(SHA256.HashData(bytes)), input.Hash_SHA256());
        this.AreEqual(Convert.ToHexString(SHA384.HashData(bytes)), input.Hash_SHA384());
        this.AreEqual(Convert.ToHexString(SHA512.HashData(bytes)), input.Hash_SHA512());
    }

    [Test] public void KnownVectors()
    {
        this.AreEqual("900150983CD24FB0D6963F7D28E17F72",                                 "abc".Hash_MD5());
        this.AreEqual("A9993E364706816ABA3E25717850C26C9CD0D89D",                         "abc".Hash_SHA1());
        this.AreEqual("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD", "abc".Hash_SHA256());
    }

    [Test] public void LongInput_And_OtherEncodings_MatchTheBcl()
    {
        this.AreEqual(Convert.ToHexString(SHA256.HashData(Encoding.Default.GetBytes(__long))), __long.Hash_SHA256());
        this.AreEqual(Convert.ToHexString(SHA256.HashData(Encoding.UTF32.GetBytes("abc"))),    "abc".Hash_SHA256(Encoding.UTF32));

        ReadOnlySpan<char> span = __long;
        this.AreEqual(Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(__long))), span.Hash_SHA512(Encoding.UTF8));
    }

    [Test] public void HashAlgorithmExtensions_MatchTheBcl()
    {
        using SHA256 hasher = SHA256.Create();
        byte[]       bytes  = Encoding.Default.GetBytes(__long);

        this.AreEqual(Convert.ToHexString(SHA256.HashData(bytes)), hasher.Hash(bytes.AsSpan()));
        this.AreEqual(Convert.ToHexString(SHA256.HashData(bytes)), hasher.Hash(Encoding.Default, __long.AsSpan()));
    }

    [Test] public async Task HashAsync_KeepsBitConverterFormat()
    {
        byte[] bytes = Encoding.UTF8.GetBytes("abc");

        this.AreEqual(BitConverter.ToString(MD5.HashData(bytes)),    await bytes.HashAsync_MD5());
        this.AreEqual(BitConverter.ToString(SHA1.HashData(bytes)),   await bytes.HashAsync_SHA1());
        this.AreEqual(BitConverter.ToString(SHA256.HashData(bytes)), await bytes.HashAsync_SHA256());
        this.AreEqual(BitConverter.ToString(SHA384.HashData(bytes)), await bytes.HashAsync_SHA384());
        this.AreEqual(BitConverter.ToString(SHA512.HashData(bytes)), await bytes.HashAsync_SHA512());

        ReadOnlyMemory<byte> memory = bytes;
        this.AreEqual(BitConverter.ToString(SHA256.HashData(bytes)), await memory.HashAsync_SHA256());

        using SHA384 hasher = SHA384.Create();
        this.AreEqual(BitConverter.ToString(SHA384.HashData(bytes)), await hasher.HashAsync(memory));
        this.AreEqual(BitConverter.ToString(SHA384.HashData(bytes)), await bytes.HashAsync(hasher));
    }

    [Test] public void HashAsync_NullArray_FaultsTheTask()
    {
        ValueTask<string> result = ( (byte[])null! ).HashAsync_SHA256();
        this.IsTrue(result.IsFaulted);
        Assert.IsInstanceOf<ArgumentNullException>(result.AsTask().Exception!.InnerException);
    }


    // ─── TryGetData ──────────────────────────────────────────────────────────

    [Test] public void TryGetData_MatchesConvertFromBase64String()
    {
        byte[] large = RandomNumberGenerator.GetBytes(4000);

        foreach ( string input in new[] { "", "AQID", "AQ ID\r\n", Convert.ToBase64String(large) } )
        {
            OneOf.OneOf<byte[], string> result = input.TryGetData();
            this.IsTrue(result.IsT0);
            this.AreEqual(Convert.FromBase64String(input), result.AsT0);
        }

        foreach ( string input in new[] { "not base64!", "abc", "====" } )
        {
            OneOf.OneOf<byte[], string> result = input.TryGetData();
            this.IsTrue(result.IsT1);
            Assert.AreSame(input, result.AsT1);
        }
    }


    // ─── Unmanaged spans ─────────────────────────────────────────────────────

    [Test] public void UnmanagedSpans_HashTheirBytesInPlace()
    {
        ReadOnlySpan<int>    ints    = [1, 2, 3, int.MaxValue];
        ReadOnlySpan<double> doubles = [1.5, -2.25, double.NaN];

        this.AreEqual(XxHash64.HashToUInt64(MemoryMarshal.AsBytes(ints), 7),         ints.Hash(7));
        this.AreEqual(XxHash128.HashToUInt128(MemoryMarshal.AsBytes(doubles), 7),    doubles.Hash128(7));
        this.AreEqual(XxHash64.HashToUInt64(MemoryMarshal.AsBytes("text".AsSpan())), "text".Hash());
        this.AreEqual(0UL,                                                           ReadOnlySpan<int>.Empty.Hash());
        this.AreEqual(UInt128.Zero,                                                  ReadOnlySpan<int>.Empty.Hash128());
    }


    // ─── String spans ────────────────────────────────────────────────────────

    [Test] public void StringSpans_AreDeterministic_EvenWithDirtyPooledArrays()
    {
        ReadOnlySpan<string> values   = ["alpha", "beta", "gamma"];
        byte[]               expected = new byte[( 5 + 4 + 5 ) * sizeof(char)]; // legacy layout: ASCII bytes, zero-padded to 2 bytes per char
        Encoding.UTF8.GetBytes("alphabetagamma", expected);

        for ( int i = 0; i < 3; i++ )
        {
            PolluteArrayPool();
            this.AreEqual(XxHash64.HashToUInt64(expected),   values.Hash());
            this.AreEqual(XxHash128.HashToUInt128(expected), values.Hash128());
        }
    }

    [Test] public void StringSpans_WithMultiByteCharacters_DoNotOverflowOrOverlap()
    {
        ReadOnlySpan<string> values = ["日本語", "中文字"]; // 3 UTF-8 bytes per char: more bytes than 2 per char
        byte[]               bytes  = Encoding.UTF8.GetBytes("日本語中文字");

        this.AreEqual(XxHash64.HashToUInt64(bytes), values.Hash());
        PolluteArrayPool();
        this.AreEqual(XxHash64.HashToUInt64(bytes), values.Hash());
    }


    private static void PolluteArrayPool()
    {
        for ( int size = 16; size <= 4096; size *= 2 )
        {
            byte[] array = ArrayPool<byte>.Shared.Rent(size);
            array.AsSpan().Fill(0xFF);
            ArrayPool<byte>.Shared.Return(array);
        }
    }
}
