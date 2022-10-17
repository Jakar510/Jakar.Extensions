// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


#nullable enable
namespace Jakar.Extensions;


public static class Hashes
{
    public static ReadOnlySpan<char> Hash( this ReadOnlyMemory<byte> data, HashAlgorithm hasher )
    {
        using var stream = new MemoryStream();
        stream.Write( data.AsReadOnlySpan() );
        stream.Seek( 0, SeekOrigin.Begin );
        Span<byte> hash = stackalloc byte[1024];

        if (!hasher.TryComputeHash( data.Span, hash, out int bytesWritten )) { throw new InvalidOperationException( nameof(hash) ); }

        Span<char> span = stackalloc char[bytesWritten + 1];
        for (int i = 0; i < bytesWritten; i++) { span[i] = (char)hash[i]; }

        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), bytesWritten );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "MD5" />
    /// </summary>
    public static ReadOnlySpan<char> Hash_MD5( this ReadOnlyMemory<byte> data )
    {
        using var hasher = MD5.Create();
        return data.Hash( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA1" />
    /// </summary>
    public static ReadOnlySpan<char> Hash_SHA1( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA1.Create();
        return data.Hash( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA256" />
    /// </summary>
    public static ReadOnlySpan<char> Hash_SHA256( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA256.Create();
        return data.Hash( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA384" />
    /// </summary>
    public static ReadOnlySpan<char> Hash_SHA384( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA384.Create();
        return data.Hash( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA512" />
    /// </summary>
    public static ReadOnlySpan<char> Hash_SHA512( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA512.Create();
        return data.Hash( hasher );
    }


#if NET6_0
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "MD5" />
    /// </summary>
    public static async ValueTask<string> HashAsync_MD5( this ReadOnlyMemory<byte> data )
    {
        using var hasher = MD5.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA1" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA1( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA1.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA256" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA256( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA256.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA384" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA384( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA384.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA512" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA512( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA512.Create();
        return await data.HashAsync( hasher );
    }
    public static async ValueTask<string> HashAsync( this ReadOnlyMemory<byte> data, HashAlgorithm hasher )
    {
        await using var stream = new MemoryStream();
        await stream.WriteAsync( data );
        stream.Seek( 0, SeekOrigin.Begin );
        byte[] hash = await hasher.ComputeHashAsync( stream );

        return BitConverter.ToString( hash );
    }


    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "MD5" />
    /// </summary>
    public static async ValueTask<string> HashAsync_MD5( this byte[] data )
    {
        using var hasher = MD5.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA1" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA1( this byte[] data )
    {
        using var hasher = SHA1.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA256" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA256( this byte[] data )
    {
        using var hasher = SHA256.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA384" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA384( this byte[] data )
    {
        using var hasher = SHA384.Create();
        return await data.HashAsync( hasher );
    }
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA512" />
    /// </summary>
    public static async ValueTask<string> HashAsync_SHA512( this byte[] data )
    {
        using var hasher = SHA512.Create();
        return await data.HashAsync( hasher );
    }
    public static async ValueTask<string> HashAsync( this byte[] data, HashAlgorithm hasher )
    {
        await using var stream = new MemoryStream( data );
        byte[]          hash   = await hasher.ComputeHashAsync( stream );
        return BitConverter.ToString( hash );
    }
#endif
}
