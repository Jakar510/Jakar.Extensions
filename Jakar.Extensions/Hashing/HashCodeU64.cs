// Jakar.Extensions :: Jakar.Extensions
// 06/13/2025  09:54

namespace Jakar.Extensions;


public struct HashCodeU64()
{
    private ulong __hash = 14695981039346656037UL; // FNV offset basis


    public void Add<T>( params ReadOnlySpan<T> values )
    {
        foreach ( T value in values ) { Add(value); }
    }
    public void Add<T>( T value )
    {
        ulong valHash = (ulong)( value?.GetHashCode() ?? 0 );
        __hash ^= valHash;
        __hash *= 1099511628211UL; // FNV prime
    }
    public ulong ToHashCode() => __hash;
    public string ToBase64()
    {
        ulong      hash  = __hash;
        Span<byte> bytes = stackalloc byte[20];
        hash.TryFormat(bytes, out int bytesWritten);
        return Convert.ToBase64String(bytes[..bytesWritten]);
    }
}
