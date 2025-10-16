// Jakar.Extensions :: Jakar.Extensions
// 10/15/2025  00:30

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;



namespace Jakar.Extensions;


//, Stream outStream



public static class HexConverter
{
    // ==============================================================
    // CORE: HEX → ULONG SPAN
    // ==============================================================

    public static bool TryHexToUInt64Span( this scoped ref readonly ReadOnlySpan<char> hex, scoped Span<ulong> destination, bool isBigEndian = false )
    {
        if ( hex.Length % 2 != 0 ) { return false; }

        int        byteCount = hex.Length / 2;
        Span<byte> bytes     = stackalloc byte[byteCount];
        if ( !hex.TryHexToBytes(bytes) ) { return false; }

        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        int ulongCount = Math.Min(bytes.Length / sizeof(ulong), destination.Length);

        ReadOnlySpan<ulong> asUlong = MemoryMarshal.Cast<byte, ulong>(bytes); // Reinterpret bytes directly (endian-agnostic)

        asUlong[..ulongCount]
           .CopyTo(destination);

        return true;
    }


    // ==============================================================
    // CORE: HEX → BYTES
    // ==============================================================

    public static bool TryHexToBytes( this scoped ref readonly ReadOnlySpan<char> hex, scoped Span<byte> bytes )
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


    // ==============================================================
    // BYTES → HEX STRING
    // ==============================================================

    public static string ToHexString( ReadOnlySpan<byte> bytes )
    {
        const string HEX_CHARS = "0123456789ABCDEF";

        return string.Create(bytes.Length * 2,
                             bytes,
                             static ( chars, src ) =>
                             {
                                 for ( int i = 0; i < src.Length; i++ )
                                 {
                                     byte b = src[i];
                                     chars[i * 2]     = HEX_CHARS[b >> 4];
                                     chars[i * 2 + 1] = HEX_CHARS[b & 0xF];
                                 }
                             });
    }

    public static bool TryHexToBytes( this ReadOnlySpan<char> hex, [NotNullWhen(true)] out byte[]? bytes )
    {
        if ( hex.Length % 2 != 0 )
        {
            bytes = null;
            return false;
        }

        bytes = new byte[hex.Length / 2];
        return hex.TryHexToBytes(bytes);
    }


    // ==============================================================
    // STRING ⇄ STREAM
    // ==============================================================

    public static Stream ToStream( ReadOnlySpan<char> hex, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[hex.Length / 2];
        if ( !hex.TryHexToBytes(bytes) ) { throw new FormatException("Invalid hex string."); }

        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        return new MemoryStream(bytes.ToArray(), writable: false);
    }


    public static MemoryStream ToMemoryStream( this Stream input )
    {
        if ( input is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> seg) ) { return new MemoryStream(seg.Array!, seg.Offset, seg.Count, writable: false); }

        using IMemoryOwner<byte> rented    = MemoryPool<byte>.Shared.Rent((int)input.Length);
        int                      bytesRead = input.Read(rented.Memory.Span);

        return new MemoryStream(rented.Memory.Slice(0, bytesRead)
                                      .ToArray(),
                                writable: false);
    }


    public static byte[] ToBytes( this Stream stream )
    {
        if ( stream is MemoryStream ms ) { return ms.ToArray(); }

        using MemoryStream buffer = new();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }


    // ==============================================================
    // STRING → INTEGER (ENDIAN AWARE)
    // ==============================================================

    public static bool TryHexToInt( this ReadOnlySpan<char> hex, out int value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[4];

        if ( !hex.TryHexToBytes(bytes) )
        {
            value = 0;
            return false;
        }

        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        value = BitConverter.ToInt32(bytes);
        return true;
    }

    public static bool TryHexToUInt( this ReadOnlySpan<char> hex, out uint value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[4];

        if ( !hex.TryHexToBytes(bytes) )
        {
            value = 0;
            return false;
        }

        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        value = BitConverter.ToUInt32(bytes);
        return true;
    }

    public static bool TryHexToLong( this ReadOnlySpan<char> hex, out long value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[8];

        if ( !hex.TryHexToBytes(bytes) )
        {
            value = 0;
            return false;
        }

        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        value = BitConverter.ToInt64(bytes);
        return true;
    }

    public static bool TryHexToULong( this ReadOnlySpan<char> hex, out ulong value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[8];

        if ( !hex.TryHexToBytes(bytes) )
        {
            value = 0;
            return false;
        }

        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        value = BitConverter.ToUInt64(bytes);
        return true;
    }


    // ==============================================================
    // INTEGER → HEX STRING (ENDIAN AWARE)
    // ==============================================================

    public static string ToHexString( ulong value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[8];
        BitConverter.TryWriteBytes(bytes, value);
        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        return ToHexString(bytes);
    }

    public static string ToHexString( uint value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[4];
        BitConverter.TryWriteBytes(bytes, value);
        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        return ToHexString(bytes);
    }

    public static string ToHexString( long value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[8];
        BitConverter.TryWriteBytes(bytes, value);
        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        return ToHexString(bytes);
    }

    public static string ToHexString( int value, bool isBigEndian = false )
    {
        Span<byte> bytes = stackalloc byte[4];
        BitConverter.TryWriteBytes(bytes, value);
        if ( isBigEndian && BitConverter.IsLittleEndian ) { bytes.Reverse(); }

        return ToHexString(bytes);
    }
}
