// Jakar.Extensions :: Jakar.Extensions
// 10/15/2025  00:30

namespace Jakar.Extensions;


public static class HexConverter
{
    public static bool TryHexToUInt64Span( this scoped ref readonly ReadOnlySpan<char> hex, scoped Span<ulong> destination )
    {
        if ( hex.Length % 2 != 0 ) { return false; }

        int        byteCount = hex.Length / 2;
        Span<byte> bytes     = stackalloc byte[byteCount];
        if ( !hex.TryHexToBytes(bytes) ) { return false; }

        int ulongCount = Math.Min(bytes.Length / sizeof(ulong), destination.Length);

        ReadOnlySpan<ulong> asUlong = MemoryMarshal.Cast<byte, ulong>(bytes); // Reinterpret bytes directly (endian-agnostic)

        asUlong[..ulongCount]
           .CopyTo(destination);

        return true;
    }
    private static bool TryHexToBytes( this scoped ref readonly ReadOnlySpan<char> hex, scoped Span<byte> bytes )
    {
        if ( bytes.Length * 2 != hex.Length ) { return false; }

        for ( int i = 0; i < bytes.Length; i++ )
        {
            int hi = FromHex(hex[i * 2]);
            int lo = FromHex(hex[i * 2 + 1]);
            if ( ( hi | lo ) < 0 ) { return false; }

            bytes[i] = (byte)( ( hi << 4 ) | lo );
        }

        return true;
    }


    private static int FromHex( char c )
    {
        if ( (uint)( c - '0' ) <= 9 ) { return c - '0'; }

        if ( (uint)( c - 'A' ) <= 5 ) { return c - 'A' + 10; }

        if ( (uint)( c - 'a' ) <= 5 ) { return c - 'a' + 10; }

        return -1;
    }
}
