// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  5:45 PM

using System.Buffers.Text;
using System.Runtime.InteropServices;



#nullable enable
namespace Jakar.Extensions.SpanAndMemory;


public static class GuidExtensions
{
    private const char EQUALS     = '=';
    private const char HYPHEN     = '-';
    private const char UNDERSCORE = '_';
    private const char SLASH      = '/';
    private const byte SLASH_BYTE = (byte)'/';
    private const char PLUS       = '+';
    private const byte PLUS_BYTE  = (byte)'+';


    /// <summary>
    ///     <see href = "https://www.youtube.com/watch?v=B2yOjLyEZk0" > Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name = "value" > </param>
    /// <returns> </returns>
    public static string AsBase64( this Guid value )
    {
        Span<byte> base64Bytes = stackalloc byte[24];
        Span<char> result      = stackalloc char[22];

        value.As(out ReadOnlySpan<byte> idBytes);
        Base64.EncodeToUtf8(idBytes, base64Bytes, out _, out _);


        for ( var i = 0; i < 22; i++ )
        {
            result[i] = base64Bytes[i] switch
                        {
                            SLASH_BYTE => HYPHEN,
                            PLUS_BYTE  => UNDERSCORE,
                            _          => (char)base64Bytes[i]
                        };
        }

        return new string(result);
    }


    /// <summary>
    ///     <see href = "https://www.youtube.com/watch?v=B2yOjLyEZk0" > Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name = "value" > </param>
    /// <returns> </returns>
    public static Guid? AsGuid( this string value ) => value.AsSpan().AsGuid();

    /// <summary>
    ///     <see href = "https://www.youtube.com/watch?v=B2yOjLyEZk0" > Writing C# without allocating ANY memory </see>
    /// </summary>
    /// <param name = "value" > </param>
    /// <returns> </returns>
    public static Guid? AsGuid( this ReadOnlySpan<char> value )
    {
        Span<char> base64Chars = stackalloc char[24];

        for ( var i = 0; i < 22; i++ )
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

        return Convert.TryFromBase64Chars(base64Chars, idBytes, out _)
                   ? new Guid(idBytes)
                   : default;
    }


    public static bool TryAsGuid( this Span<char> value, [NotNullWhen(true)] out Guid? result )
    {
        if ( Guid.TryParse(value, out Guid guid) )
        {
            result = guid;
            return true;
        }

        result = AsGuid(value);
        return result.HasValue;
    }
    public static bool TryAsGuid( this ReadOnlySpan<char> value, [NotNullWhen(true)] out Guid? result )
    {
        if ( Guid.TryParse(value, out Guid guid) )
        {
            result = guid;
            return true;
        }

        result = AsGuid(value);
        return result.HasValue;
    }
    public static bool TryAsGuid( this string value, [NotNullWhen(true)] out Guid? result ) => value.AsSpan().TryAsGuid(out result);


    public static bool As( this Guid value, out Memory<byte> result )
    {
        var span = new Span<byte>();

        if ( value.As(ref span) )
        {
            result = span.AsMemory();
            return true;
        }

        result = default;
        return false;
    }
    public static bool As( this Guid value, out ReadOnlyMemory<byte> result )
    {
        var span = new Span<byte>();

        if ( value.As(ref span) )
        {
            result = span.AsMemory();
            return true;
        }

        result = default;
        return false;
    }


    public static bool As( this Guid value, ref Span<byte> result ) => MemoryMarshal.TryWrite(result, ref value);
    public static bool As( this Guid value, out ReadOnlySpan<byte> result )
    {
        var span = new Span<byte>();

        if ( value.As(ref span) )
        {
            result = span;
            return true;
        }

        result = default;
        return false;
    }
}
