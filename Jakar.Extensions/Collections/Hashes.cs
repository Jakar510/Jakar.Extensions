// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


#if NET6_0
#nullable enable
namespace Jakar.Extensions.Collections;


public static class Hashes
{
    public static async Task<string> HashAsync( this byte[] data, CancellationToken token )
    {
        using var       md5    = SHA256.Create();
        await using var stream = new MemoryStream(data);
        byte[]          hash   = await md5.ComputeHashAsync(stream, token);

        return BitConverter.ToString(hash);
    }
}



#endif
