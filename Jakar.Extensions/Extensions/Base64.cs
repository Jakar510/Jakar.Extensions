#nullable enable
namespace Jakar.Extensions;


public static class Base64
{
    public static byte[] FromBase64String( this string b64 ) => Convert.FromBase64String( b64 );


    public static TResult JsonFromBase64String<TResult>( this string b64 ) => b64.JsonFromBase64String<TResult>( Encoding.Default );
    public static TResult JsonFromBase64String<TResult>( this string b64, Encoding encoding )
    {
        byte[] bytes = b64.FromBase64String();
        string temp  = encoding.GetString( bytes );
        return temp.FromJson<TResult>();
    }


    public static string ToBase64( this object jsonSerializablePayload ) => jsonSerializablePayload.ToBase64( Encoding.Default );
    public static string ToBase64( this object jsonSerializablePayload, Encoding encoding )
    {
        string temp = jsonSerializablePayload.ToJson();
        return temp.ToBase64( encoding );
    }


    public static string ToBase64( this string data ) => data.ToBase64( Encoding.Default );
    public static string ToBase64( this string data, Encoding encoding )
    {
        byte[] payload = encoding.GetBytes( data );

        return Convert.ToBase64String( payload );
    }
    public static string ToBase64( this byte[]               payload ) => Convert.ToBase64String( payload );
    public static string ToBase64( this Memory<byte>         payload ) => payload.Span.ToBase64();
    public static string ToBase64( this ReadOnlyMemory<byte> payload ) => payload.Span.ToBase64();
    public static string ToBase64( this Span<byte> payload )
    {
        ReadOnlySpan<byte> span = payload;
        return span.ToBase64();
    }
    public static string ToBase64( this ReadOnlySpan<byte> payload ) => Convert.ToBase64String( payload );


    public static MemoryStream ToStreamFromBase64String( this string b64 )
    {
        byte[] buffer = b64.FromBase64String();
        return new MemoryStream( buffer );
    }
}
