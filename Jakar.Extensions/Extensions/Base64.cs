namespace Jakar.Extensions;


public static class Base64
{
    public static byte[] FromBase64String( this string b64 ) => Convert.FromBase64String(b64);


    public static MemoryStream ToStreamFromBase64String( this string b64 )
    {
        byte[] buffer = b64.FromBase64String();
        return new MemoryStream(buffer);
    }


    public static string ToBase64<TValue>( this TValue jsonSerializablePayload )
        where TValue : IJsonModel<TValue> => jsonSerializablePayload.ToBase64(Encoding.Default);
    public static string ToBase64<TValue>( this TValue jsonSerializablePayload, Encoding encoding )
        where TValue : IJsonModel<TValue>
    {
        string temp = jsonSerializablePayload.ToJson();
        return temp.ToBase64(encoding);
    }


    public static string ToBase64<TValue>( this TValue jsonSerializablePayload, JsonTypeInfo<TValue> info ) => jsonSerializablePayload.ToBase64(info, Encoding.Default);
    public static string ToBase64<TValue>( this TValue jsonSerializablePayload, JsonTypeInfo<TValue> info, Encoding encoding )
    {
        string temp = jsonSerializablePayload.ToJson(info);
        return temp.ToBase64(encoding);
    }


    public static string ToBase64( this string data ) => data.ToBase64(Encoding.Default);
    public static string ToBase64( this string data, Encoding encoding )
    {
        byte[] payload = encoding.GetBytes(data);
        return Convert.ToBase64String(payload);
    }
    public static string ToBase64( this byte[] payload ) => Convert.ToBase64String(payload);
    public static string ToBase64( this ref readonly Memory<byte> payload )
    {
        ReadOnlySpan<byte> span = payload.Span;
        return span.ToBase64();
    }
    public static string ToBase64( this ref readonly ReadOnlyMemory<byte> payload )
    {
        ReadOnlySpan<byte> span = payload.Span;
        return span.ToBase64();
    }
    public static string ToBase64( this ref readonly Span<byte> payload )
    {
        ReadOnlySpan<byte> span = payload;
        return span.ToBase64();
    }
    public static string ToBase64( this ref readonly ReadOnlySpan<byte> payload ) => Convert.ToBase64String(payload);


    public static TValue JsonFromBase64String<TValue>( this string b64 )
        where TValue : IJsonModel<TValue> => b64.JsonFromBase64String<TValue>(Encoding.Default);
    public static TValue JsonFromBase64String<TValue>( this string b64, Encoding encoding )
        where TValue : IJsonModel<TValue>
    {
        byte[] bytes = b64.FromBase64String();
        string temp  = encoding.GetString(bytes);
        return temp.FromJson<TValue>();
    }
}
