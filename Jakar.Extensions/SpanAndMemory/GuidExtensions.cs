// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  5:45 PM

#nullable enable
using OneOf.Types;



namespace Jakar.Extensions;


public static class GuidExtensions
{
    private const char EQUALS     = '=';
    private const char HYPHEN     = '-';
    private const char PLUS       = '+';
    private const byte PLUS_BYTE  = (byte)'+';
    private const char SLASH      = '/';
    private const byte SLASH_BYTE = (byte)'/';
    private const char UNDERSCORE = '_';


    public static bool TryAsGuid( this Span<char> value, [NotNullWhen( true )] out Guid? result )
    {
        result = AsGuid( value );
        return result.HasValue;
    }
    public static bool TryAsGuid( this ReadOnlySpan<char> value, [NotNullWhen( true )] out Guid? result )
    {
        result = AsGuid( value );
        return result.HasValue;
    }
    public static bool TryAsGuid( this string value, [NotNullWhen( true )] out Guid? result ) => value.AsSpan()
                                                                                                      .TryAsGuid( out result );


    public static bool TryWriteBytes( this Guid value, out Memory<byte> result )
    {
        Span<byte> span = stackalloc byte[16];

        if ( value.TryWriteBytes( span ) )
        {
            result = span.AsMemory();
            return true;
        }

        result = default;
        return false;
    }
    public static bool TryWriteBytes( this Guid value, out ReadOnlyMemory<byte> result )
    {
        Span<byte> span = stackalloc byte[16];

        if ( value.TryWriteBytes( span ) )
        {
            result = span.AsMemory();
            return true;
        }

        result = default;
        return false;
    }


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static Guid? AsGuid( this string value ) => AsGuid( value.AsSpan() );


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static Guid? AsGuid( this ReadOnlySpan<char> value )
    {
        if ( Guid.TryParse( value, out Guid result ) ) { return result; }

        Span<char> base64Chars = stackalloc char[24];

        for ( int i = 0; i < 22; i++ )
        {
            base64Chars[i] = value[i] switch
                             {
                                 HYPHEN     => SLASH,
                                 UNDERSCORE => PLUS,
                                 _          => value[i],
                             };
        }

        base64Chars[22] = EQUALS;
        base64Chars[23] = EQUALS;

        Span<byte> idBytes = stackalloc byte[16];

        return Convert.TryFromBase64Chars( base64Chars, idBytes, out int bytesWritten )
                   ? new Guid( idBytes[..bytesWritten] )
                   : default;
    }


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static string ToBase64( this Guid value ) => new(value.AsSpan());


    /// <summary>
    ///     <see href="https://www.youtube.com/watch?v=B2yOjLyEZk0"> Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name="value"> </param>
    /// <returns> </returns>
    public static ReadOnlySpan<char> AsSpan( this Guid value )
    {
        Span<byte> base64Bytes = stackalloc byte[24];
        Span<char> result      = stackalloc char[22];

        Span<byte> idBytes = stackalloc byte[16];
        if ( !value.TryWriteBytes( idBytes ) ) { throw new InvalidOperationException(); }

        System.Buffers.Text.Base64.EncodeToUtf8( idBytes, base64Bytes, out _, out int bytesWritten );


        for ( int i = 0; i < bytesWritten; i++ )
        {
            result[i] = base64Bytes[i] switch
                        {
                            SLASH_BYTE => HYPHEN,
                            PLUS_BYTE  => UNDERSCORE,
                            _          => (char)base64Bytes[i],
                        };
        }

        return MemoryMarshal.CreateReadOnlySpan( ref result.GetPinnableReference(), result.Length );
    }
}
