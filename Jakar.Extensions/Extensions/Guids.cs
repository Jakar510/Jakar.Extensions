// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  5:45 PM

namespace Jakar.Extensions;


public static class Guids
{
    private const char EQUALS     = '=';
    private const char HYPHEN     = '-';
    private const char PLUS       = '+';
    private const byte PLUS_BYTE  = (byte)'+';
    private const char SLASH      = '/';
    private const byte SLASH_BYTE = (byte)'/';
    private const char UNDERSCORE = '_';


    public static bool TryAsGuid( this ref readonly Span<char> value, [NotNullWhen( true )] out Guid? result )
    {
        ReadOnlySpan<char> span = value;
        result = span.AsGuid();
        return result.HasValue;
    }
    public static bool TryAsGuid( this ref readonly ReadOnlySpan<char> value, [NotNullWhen( true )] out Guid? result )
    {
        result = value.AsGuid();
        return result.HasValue;
    }
    public static bool TryAsGuid( this string value, [NotNullWhen( true )] out Guid? result )
    {
        ReadOnlySpan<char> span = value;
        return span.TryAsGuid( out result );
    }


    public static bool TryWriteBytes( this ref readonly Guid value, out Span<byte> result )
    {
        Span<byte> span = stackalloc byte[16];

        if ( value.TryWriteBytes( span ) )
        {
            result = MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
            return true;
        }

        result = Span<byte>.Empty;
        return false;
    }
    public static bool TryWriteBytes( this ref readonly Guid value, out ReadOnlySpan<byte> result )
    {
        Span<byte> span = stackalloc byte[16];

        if ( value.TryWriteBytes( span ) )
        {
            result = MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
            return true;
        }

        result = ReadOnlySpan<byte>.Empty;
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
        return AsGuid( in span );
    }


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static Guid? AsGuid( this ref readonly ReadOnlySpan<char> value )
    {
        if ( Guid.TryParse( value, out Guid result ) ) { return result; }

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

        return Convert.TryFromBase64Chars( base64Chars, idBytes, out int bytesWritten )
                   ? new Guid( idBytes[..bytesWritten] )
                   : Guid.Empty;
    }

    
    public static string NewBase64()
    {
        Guid id = Guid.CreateVersion7( DateTimeOffset.UtcNow );
        return NewBase64( in id );
    }
    public static string NewBase64( this ref readonly DateTimeOffset timeStamp )
    {
        Guid id = Guid.CreateVersion7( timeStamp );
        return NewBase64( in id );
    }
    public static string NewBase64( this ref readonly DateTimeOffset? timeStamp )
    {
        Guid id = timeStamp.HasValue
                      ? Guid.CreateVersion7( timeStamp.Value )
                      : Guid.NewGuid();

        return NewBase64( in id );
    }
    public static string NewBase64( this ref readonly Guid id )
    {
        Span<char> result = stackalloc char[22];
        id.AsSpan( ref result );
        return new string( result );
    }


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static string ToBase64( this ref readonly Guid value )
    {
        Span<char> result = stackalloc char[22];
        value.AsSpan( ref result );
        return new string( result );
    }


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <param name="result"> </param>
    /// <returns> </returns>
    public static bool AsSpan( this ref readonly Guid value, scoped ref Span<char> result )
    {
        Guard.IsGreaterThanOrEqualTo( result.Length, 22 );
        Span<byte> base64Bytes = stackalloc byte[24];
        Span<byte> idBytes     = stackalloc byte[16];
        if ( value.TryWriteBytes( idBytes ) is false ) { throw new InvalidOperationException( "Guid.TryWriteBytes failed" ); }

        System.Buffers.Text.Base64.EncodeToUtf8( idBytes, base64Bytes, out _, out int bytesWritten );
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


    public static (long Lower, long Upper) AsLong( this ref readonly Guid value ) =>
        value.AsLong( out long lower, out long upper )
            ? (lower, upper)
            : throw new InvalidOperationException( "Guid.TryWriteBytes failed" );
    public static (ulong Lower, ulong Upper) AsULong( this ref readonly Guid value ) =>
        value.AsLong( out ulong lower, out ulong upper )
            ? (lower, upper)
            : throw new InvalidOperationException( "Guid.TryWriteBytes failed" );


    public static bool AsLong( this ref readonly Guid value, out long lower, out long upper )
    {
        const int  SIZE = sizeof(long);
        Span<byte> span = stackalloc byte[SIZE * 2];

        if ( value.TryWriteBytes( span ) )
        {
            lower = BitConverter.ToInt64( span[..SIZE] );
            upper = BitConverter.ToInt64( span[SIZE..] );
            return true;
        }

        lower = 0;
        upper = 0;
        return false;
    }
    public static bool AsLong( this ref readonly Guid value, out ulong lower, out ulong upper )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];

        if ( value.TryWriteBytes( span ) )
        {
            lower = BitConverter.ToUInt64( span[..SIZE] );
            upper = BitConverter.ToUInt64( span[SIZE..] );
            return true;
        }

        lower = 0;
        upper = 0;
        return false;
    }


    public static Guid AsGuid( this ref readonly (long Lower, long Upper) value )
    {
        const int  SIZE = sizeof(long);
        Span<byte> span = stackalloc byte[SIZE * 2];

        if ( BitConverter.TryWriteBytes( span[..SIZE], value.Lower ) && BitConverter.TryWriteBytes( span[SIZE..], value.Upper ) ) { return new Guid( span ); }

        throw new InvalidOperationException( "BitConverter.TryWriteBytes failed" );
    }
    public static Guid AsGuid( this ref readonly (ulong Lower, ulong Upper) value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];

        return BitConverter.TryWriteBytes( span[..SIZE], value.Lower ) && BitConverter.TryWriteBytes( span[SIZE..], value.Upper )
                   ? new Guid( span )
                   : throw new InvalidOperationException( "BitConverter.TryWriteBytes failed" );
    }
    public static Guid AsGuid( this ref readonly long value )
    {
        const int  SIZE = sizeof(long);
        Span<byte> span = stackalloc byte[SIZE * 2];
        span.Clear();

        if ( BitConverter.TryWriteBytes( span, value ) is false ) { throw new InvalidOperationException( "BitConverter.TryWriteBytes failed" ); }

        return new Guid( span );
    }
    public static Guid AsGuid( this ref readonly ulong value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];
        span.Clear();

        if ( BitConverter.TryWriteBytes( span, value ) is false ) { throw new InvalidOperationException( "BitConverter.TryWriteBytes failed" ); }

        return new Guid( span );
    }


    public static Guid AsGuid( this ref readonly Int128 value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];
        if ( value.TryFormat( span, out int bytesWritten ) is false ) { throw new InvalidOperationException( "BitConverter.TryWriteBytes failed" ); }

        return new Guid( span );
    }
    public static Guid AsGuid( this ref readonly UInt128 value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];
        if ( value.TryFormat( span, out int bytesWritten ) is false ) { throw new InvalidOperationException( "BitConverter.TryWriteBytes failed" ); }

        return new Guid( span );
    }
    public static Int128 AsInt128( this ref readonly Guid value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];

        return value.TryWriteBytes( span )
                   ? new Int128( BitConverter.ToUInt64( span[..SIZE] ), BitConverter.ToUInt64( span[SIZE..] ) )
                   : throw new InvalidOperationException( "Guid.TryWriteBytes failed" );
    }
    public static UInt128 AsUInt128( this ref readonly Guid value )
    {
        const int  SIZE = sizeof(ulong);
        Span<byte> span = stackalloc byte[SIZE * 2];

        return value.TryWriteBytes( span )
                   ? new UInt128( BitConverter.ToUInt64( span[..SIZE] ), BitConverter.ToUInt64( span[SIZE..] ) )
                   : throw new InvalidOperationException( "Guid.TryWriteBytes failed" );
    }
}
