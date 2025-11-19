// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  5:45 PM

using System.Buffers.Text;
using System.Text;



namespace Jakar.Extensions;


[InlineArray(16)]
public struct GuidBytes
{
    private byte __value;
}



public static class Guids
{
    public static bool TryAsGuid( in this Span<char> value, [NotNullWhen(true)] out Guid? result )
    {
        ReadOnlySpan<char> span = value;
        result = span.AsGuid();
        return result.HasValue;
    }
    public static bool TryAsGuid( in this ReadOnlySpan<char> value, [NotNullWhen(true)] out Guid? result )
    {
        result = value.AsGuid();
        return result.HasValue;
    }
    public static bool TryAsGuid( this string value, [NotNullWhen(true)] out Guid? result )
    {
        ReadOnlySpan<char> span = value;
        return span.TryAsGuid(out result);
    }


    public static bool TryWriteBytes( in this Guid value, [MustDisposeResource] out Buffer<byte> result )
    {
        result = new Buffer<byte>(16);
        if ( value.TryWriteBytes(result.Span) ) { return true; }

        result.Dispose();
        result = default;
        return false;
    }


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static Guid? AsGuid( this string value )
    {
        ReadOnlySpan<char> span = value;
        return AsGuid(in span);
    }


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static Guid? AsGuid( in this ReadOnlySpan<char> value )
    {
        if ( Guid.TryParse(value, out Guid result) ) { return result; }

        Span<char> base64Chars = stackalloc char[24];

        for ( int i = 0; i < 22; i++ )
        {
            base64Chars[i] = value[i] switch
                             {
                                 HYPHEN     => SLASH,
                                 UNDERSCORE => PLUS,
                                 _          => value[i]
                             };
        }

        base64Chars[22] = EQUALS;
        base64Chars[23] = EQUALS;

        Span<byte> idBytes = stackalloc byte[16];

        return Convert.TryFromBase64Chars(base64Chars, idBytes, out int bytesWritten)
                   ? new Guid(idBytes[..bytesWritten])
                   : Guid.Empty;
    }


    public static string NewBase64()
    {
        Guid id = Guid.CreateVersion7(DateTimeOffset.UtcNow);
        return NewBase64(in id);
    }
    public static string NewBase64( in this DateTimeOffset timeStamp )
    {
        Guid id = Guid.CreateVersion7(timeStamp);
        return NewBase64(in id);
    }
    public static string NewBase64( in this DateTimeOffset? timeStamp )
    {
        Guid id = timeStamp.HasValue
                      ? Guid.CreateVersion7(timeStamp.Value)
                      : Guid.NewGuid();

        return NewBase64(in id);
    }



    extension( in Guid self )
    {
        public string NewBase64()
        {
            Span<char> result = stackalloc char[22];
            self.AsSpan(ref result, out int bytesWritten);
            return new string(result[..bytesWritten]);
        }

        /// <summary>
        ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
        /// </summary>
        /// <returns> </returns>
        public string ToBase64()
        {
            Span<char> result = stackalloc char[22];
            self.AsSpan(ref result, out int bytesWritten);
            return new string(result[..bytesWritten]);
        }
        public string ToHex()
        {
            Span<byte> result = stackalloc byte[32];
            self.AsSpan(ref result, out int bytesWritten);
            return Convert.ToHexString(result[..bytesWritten]);
        }

        /// <summary>
        ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
        /// </summary>
        public bool AsSpan( scoped ref Span<char> result, out int bytesWritten )
        {
            Guard.IsGreaterThanOrEqualTo(result.Length, 22);
            Span<byte> base64Bytes = stackalloc byte[24];
            Span<byte> idBytes     = stackalloc byte[16];
            if ( !self.TryWriteBytes(idBytes, BitConverter.IsLittleEndian, out bytesWritten) ) { throw new InvalidOperationException("Guid.TryWriteBytes failed"); }

            System.Buffers.Text.Base64.EncodeToUtf8(idBytes, base64Bytes, out _, out bytesWritten);
            result = result[..bytesWritten];

            for ( int i = 0; i < bytesWritten; i++ )
            {
                result[i] = base64Bytes[i] switch
                            {
                                SLASH_BYTE => HYPHEN,
                                PLUS_BYTE  => UNDERSCORE,
                                _          => (char)base64Bytes[i]
                            };
            }

            return true;
        }
        public bool AsSpan( scoped ref Span<byte> result, out int bytesWritten, StandardFormat format = default )
        {
            Guard.IsGreaterThanOrEqualTo(result.Length, 16);
            return Utf8Formatter.TryFormat(self, result, out bytesWritten, format);
        }
        public (long Lower, long Upper) AsLong() =>
            self.AsLong(out long lower, out long upper)
                ? ( lower, upper )
                : throw new InvalidOperationException("Guid.TryWriteBytes failed");
        public (ulong Lower, ulong Upper) AsULong() =>
            self.AsLong(out ulong lower, out ulong upper)
                ? ( lower, upper )
                : throw new InvalidOperationException("Guid.TryWriteBytes failed");
        public bool AsLong( out long lower, out long upper )
        {
            const int  SIZE = sizeof(long);
            Span<byte> span = stackalloc byte[SIZE * 2];

            if ( self.TryWriteBytes(span) )
            {
                lower = BitConverter.ToInt64(span[..SIZE]);
                upper = BitConverter.ToInt64(span[SIZE..]);
                return true;
            }

            lower = 0;
            upper = 0;
            return false;
        }
        public bool AsLong( out ulong lower, out ulong upper )
        {
            const int  SIZE = sizeof(ulong);
            Span<byte> span = stackalloc byte[SIZE * 2];

            if ( self.TryWriteBytes(span) )
            {
                lower = BitConverter.ToUInt64(span[..SIZE]);
                upper = BitConverter.ToUInt64(span[SIZE..]);
                return true;
            }

            lower = 0;
            upper = 0;
            return false;
        }
    }



    public static Guid AsGuid( in this (long Lower, long Upper) value )
    {
        const int  SIZE = sizeof(long);
        Span<byte> span = stackalloc byte[SIZE * 2];

        if ( BitConverter.TryWriteBytes(span[..SIZE], value.Lower) && BitConverter.TryWriteBytes(span[SIZE..], value.Upper) ) { return new Guid(span); }

        throw new InvalidOperationException("BitConverter.TryWriteBytes failed");
    }
    public static Guid AsGuid( in this (ulong Lower, ulong Upper) value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];

        return BitConverter.TryWriteBytes(span[..SIZE], value.Lower) && BitConverter.TryWriteBytes(span[SIZE..], value.Upper)
                   ? new Guid(span)
                   : throw new InvalidOperationException("BitConverter.TryWriteBytes failed");
    }
    public static Guid AsGuid( in this long value )
    {
        const int  SIZE = sizeof(long);
        Span<byte> span = stackalloc byte[SIZE * 2];
        span.Clear();

        if ( !BitConverter.TryWriteBytes(span, value) ) { throw new InvalidOperationException("BitConverter.TryWriteBytes failed"); }

        return new Guid(span);
    }
    public static Guid AsGuid( in this ulong value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];
        span.Clear();

        if ( !BitConverter.TryWriteBytes(span, value) ) { throw new InvalidOperationException("BitConverter.TryWriteBytes failed"); }

        return new Guid(span);
    }


    public static Guid AsGuid( in this Int128 value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];
        if ( !value.TryFormat(span, out int bytesWritten) ) { throw new InvalidOperationException("BitConverter.TryWriteBytes failed"); }

        return new Guid(span[..bytesWritten]);
    }
    public static Guid AsGuid( in this UInt128 value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];
        if ( !value.TryFormat(span, out int bytesWritten) ) { throw new InvalidOperationException("BitConverter.TryWriteBytes failed"); }

        return new Guid(span[..bytesWritten]);
    }



    extension( in Guid value )
    {
        public Int128 AsInt128()
        {
            const int  SIZE = sizeof(ulong);
            Span<byte> span = stackalloc byte[SIZE * 2];

            return value.TryWriteBytes(span)
                       ? new Int128(BitConverter.ToUInt64(span[..SIZE]), BitConverter.ToUInt64(span[SIZE..]))
                       : throw new InvalidOperationException("Guid.TryWriteBytes failed");
        }
        public UInt128 AsUInt128()
        {
            const int  SIZE = sizeof(ulong);
            Span<byte> span = stackalloc byte[SIZE * 2];

            return value.TryWriteBytes(span)
                       ? new UInt128(BitConverter.ToUInt64(span[..SIZE]), BitConverter.ToUInt64(span[SIZE..]))
                       : throw new InvalidOperationException("Guid.TryWriteBytes failed");
        }
    }
}
