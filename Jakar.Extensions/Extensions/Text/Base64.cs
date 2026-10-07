namespace Jakar.Extensions;


public static class Base64
{
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



    extension( string self )
    {
        public byte[] FromBase64String() => Convert.FromBase64String(self);
        public MemoryStream ToStreamFromBase64String()
        {
            byte[] buffer = self.FromBase64String();
            return new MemoryStream(buffer);
        }


        public string ToBase64() => self.ToBase64(Encoding.Default);
        public string ToBase64( Encoding encoding )
        {
            byte[] payload = encoding.GetBytes(self);
            return Convert.ToBase64String(payload);
        }
    }



    extension<TValue>( TValue jsonSerializablePayload )
    {
        public string ToBase64() => jsonSerializablePayload.ToBase64(Encoding.Default);
        public string ToBase64( Encoding encoding )
        {
            string temp = Json.Serialize(jsonSerializablePayload, false);
            return temp.ToBase64(encoding);
        }
        public string ToBase64( Encoding encoding, JsonTypeInfo<TValue> info )
        {
            string temp = JsonModel.ToJson(jsonSerializablePayload, info, false);
            return temp.ToBase64(encoding);
        }
    }



    extension( string b64 )
    {
        public TValue JsonFromBase64String<TValue>()                    => b64.JsonFromBase64String<TValue>(Encoding.Default);
        public TValue JsonFromBase64String<TValue>( Encoding encoding ) => b64.JsonFromBase64String(Json.GetTypeInfo<TValue>(), encoding);
        public TValue JsonFromBase64String<TValue>( JsonTypeInfo<TValue> info, Encoding? encoding = null )
        {
            byte[] bytes = b64.FromBase64String();
            string temp  = ( encoding ?? Encoding.Default ).GetString(bytes);
            return temp.FromJson(info);
        }
    }
}
