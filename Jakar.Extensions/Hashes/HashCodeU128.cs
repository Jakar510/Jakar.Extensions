// Jakar.Extensions :: Jakar.Extensions
// 06/13/2025  09:54

namespace Jakar.Extensions;


public struct HashCodeU64()
{
    private ulong _hash = 14695981039346656037UL; // FNV offset basis


    public void Add<T>( params ReadOnlySpan<T> values )
    {
        foreach ( T value in values ) { Add( value ); }
    }
    public void Add<T>( T value )
    {
        ulong valHash = (ulong)(value?.GetHashCode() ?? 0);
        _hash ^= valHash;
        _hash *= 1099511628211UL; // FNV prime
    }
    public ulong ToHashCode() => _hash;
    public string ToBase64()
    {
        ulong      hash  = _hash;
        Span<byte> bytes = stackalloc byte[20];
        hash.TryFormat( bytes, out int bytesWritten );
        return Convert.ToBase64String( bytes[..bytesWritten] );
    }
}



public struct HashCodeU128()
{
    private UInt128 _hash = new(0x6eed0e9da4d94a4f, 0x9e3779b97f4a7c15); // SplitMix64 seeds


    public void Add<T>( params ReadOnlySpan<T> values )
    {
        foreach ( T value in values ) { Add( value ); }
    }
    public void Add<T>( T value )
    {
        ulong valHash = (ulong)(value?.GetHashCode() ?? 0);

        _hash += valHash;
        _hash ^= _hash >> 32;
        _hash *= new UInt128( 0x94d049bb133111eb, 0x2545f4914f6cdd1d ); // mix constant
    }
    public UInt128 ToHashCode() => _hash;
    public string ToBase64()
    {
        UInt128    hash  = _hash;
        Span<byte> bytes = stackalloc byte[40];
        hash.TryFormat( bytes, out int bytesWritten );
        return Convert.ToHexString( bytes[..bytesWritten] );
    }
}
