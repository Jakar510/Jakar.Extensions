// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


#if NET6_0_OR_GREATER
#endif



namespace Jakar.Extensions;


public static class Hashes
{
    public static string Hash( this HashAlgorithm hasher, in ReadOnlySpan<byte> data )
    {
        using IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent( IFileData.HASH_SIZE_LIMIT );
        Span<byte>               span   = buffer.Memory.Span;
        if ( hasher.TryComputeHash( data, span, out int bytesWritten ) is false ) { throw new InvalidOperationException( nameof(span) ); }

        span = span[..bytesWritten];
        Span<char>   hexChars     = stackalloc char[span.Length * 2];
        const string HEX_ALPHABET = "0123456789ABCDEF";

        for ( int i = 0; i < span.Length; i++ )
        {
            byte b = span[i];
            hexChars[i * 2]     = HEX_ALPHABET[b >> 4];
            hexChars[i * 2 + 1] = HEX_ALPHABET[b & 0x0F];
        }
        
        return hexChars.ToString();
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ReadOnlySpan<byte> data )
    {
        using var hasher = MD5.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ReadOnlySpan<byte> data )
    {
        using var hasher = SHA1.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ReadOnlySpan<byte> data )
    {
        using var hasher = SHA256.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ReadOnlySpan<byte> data )
    {
        using var hasher = SHA384.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ReadOnlySpan<byte> data )
    {
        using var hasher = SHA512.Create();
        return hasher.Hash( data );
    }


#if NET6_0_OR_GREATER
    public static async ValueTask<string> HashAsync( this HashAlgorithm hasher, ReadOnlyMemory<byte> data )
    {
        await using var stream = new MemoryStream();
        await stream.WriteAsync( data );
        stream.Seek( 0, SeekOrigin.Begin );
        byte[] hash = await hasher.ComputeHashAsync( stream );

        return BitConverter.ToString( hash );
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static async ValueTask<string> HashAsync_MD5( this ReadOnlyMemory<byte> data )
    {
        using var hasher = MD5.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static async ValueTask<string> HashAsync_SHA1( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA1.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static async ValueTask<string> HashAsync_SHA256( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA256.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static async ValueTask<string> HashAsync_SHA384( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA384.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static async ValueTask<string> HashAsync_SHA512( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA512.Create();
        return await hasher.HashAsync( data );
    }


    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static async ValueTask<string> HashAsync_MD5( this byte[] data )
    {
        using var hasher = MD5.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static async ValueTask<string> HashAsync_SHA1( this byte[] data )
    {
        using var hasher = SHA1.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static async ValueTask<string> HashAsync_SHA256( this byte[] data )
    {
        using var hasher = SHA256.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static async ValueTask<string> HashAsync_SHA384( this byte[] data )
    {
        using var hasher = SHA384.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static async ValueTask<string> HashAsync_SHA512( this byte[] data )
    {
        using var hasher = SHA512.Create();
        return await hasher.HashAsync( data );
    }
    public static async ValueTask<string> HashAsync( this byte[] data, HashAlgorithm hasher )
    {
        await using var stream = new MemoryStream( data );
        byte[]          hash = await hasher.ComputeHashAsync( stream );
        return BitConverter.ToString( hash );
    }
#endif


#if NET7_0_OR_GREATER
    public static UInt128 Hash( this string data ) => Hash( data, Encoding.Default );
    public static UInt128 Hash( this string data, Encoding encoding )
    {
        OneOf<byte[], string> one = IFileData.GetData( data );

        return one.IsT0
                   ? Hash( one.AsT0 )
                   : Hash( one.AsT1.AsSpan(), encoding );
    }
    public static UInt128 Hash( this ReadOnlySpan<char> data, Encoding encoding )
    {
        using IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent( encoding.GetByteCount( data ) );
        Span<byte>               span = buffer.Memory.Span;
        int                      size = encoding.GetBytes( data, span );
        span = span[..size];
        return Hash( span );
    }
    public static UInt128 Hash( this ReadOnlySpan<byte> data ) => XxHash128.HashToUInt128( data );
#endif
}
