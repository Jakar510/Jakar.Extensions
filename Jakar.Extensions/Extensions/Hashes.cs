// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


#nullable enable
namespace Jakar.Extensions;


public static class Hashes
{
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static ReadOnlySpan<char> Hash_MD5( this ReadOnlyMemory<byte> data ) => data.Hash(MD5.Create());
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static ReadOnlySpan<char> Hash_SHA1( this ReadOnlyMemory<byte> data ) => data.Hash(SHA1.Create());
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static ReadOnlySpan<char> Hash_SHA256( this ReadOnlyMemory<byte> data ) => data.Hash(SHA256.Create());
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static ReadOnlySpan<char> Hash_SHA384( this ReadOnlyMemory<byte> data ) => data.Hash(SHA384.Create());
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static ReadOnlySpan<char> Hash_SHA512( this ReadOnlyMemory<byte> data ) => data.Hash(SHA512.Create());
    private static ReadOnlySpan<char> Hash( this ReadOnlyMemory<byte> data, HashAlgorithm hasher )
    {
        using ( hasher )
        {
            using var stream = new MemoryStream();
            stream.Write(data.AsReadOnlySpan());
            stream.Seek(0, SeekOrigin.Begin);
            Span<byte> hash = stackalloc byte[1024];

            if ( !hasher.TryComputeHash(data.Span, hash, out int bytesWritten) ) { throw new InvalidOperationException(nameof(hash)); }

            Span<char> span = stackalloc char[bytesWritten + 1];
            for ( var i = 0; i < bytesWritten; i++ ) { span[i] = (char)hash[i]; }

            return MemoryMarshal.CreateReadOnlySpan(ref span.GetPinnableReference(), bytesWritten);
        }
    }


#if NET6_0
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static ValueTask<string> HashAsync_MD5( this ReadOnlyMemory<byte> data ) => data.HashAsync(MD5.Create());
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static ValueTask<string> HashAsync_SHA1( this ReadOnlyMemory<byte> data ) => data.HashAsync(SHA1.Create());
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static ValueTask<string> HashAsync_SHA256( this ReadOnlyMemory<byte> data ) => data.HashAsync(SHA256.Create());
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static ValueTask<string> HashAsync_SHA384( this ReadOnlyMemory<byte> data ) => data.HashAsync(SHA384.Create());
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static ValueTask<string> HashAsync_SHA512( this ReadOnlyMemory<byte> data ) => data.HashAsync(SHA512.Create());
    public static async ValueTask<string> HashAsync( this ReadOnlyMemory<byte> data, HashAlgorithm hasher )
    {
        using ( hasher )
        {
            await using var stream = new MemoryStream();
            await stream.WriteAsync(data);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] hash = await hasher.ComputeHashAsync(stream);

            return BitConverter.ToString(hash);
        }
    }


    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static ValueTask<string> HashAsync_MD5( this byte[] data ) => data.HashAsync(MD5.Create());
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static ValueTask<string> HashAsync_SHA1( this byte[] data ) => data.HashAsync(SHA1.Create());
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static ValueTask<string> HashAsync_SHA256( this byte[] data ) => data.HashAsync(SHA256.Create());
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static ValueTask<string> HashAsync_SHA384( this byte[] data ) => data.HashAsync(SHA384.Create());
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static ValueTask<string> HashAsync_SHA512( this byte[] data ) => data.HashAsync(SHA512.Create());
    public static async ValueTask<string> HashAsync( this byte[] data, HashAlgorithm hasher )
    {
        using ( hasher )
        {
            await using var stream = new MemoryStream(data);
            byte[]          hash   = await hasher.ComputeHashAsync(stream);
            return BitConverter.ToString(hash);
        }
    }
#endif
}
