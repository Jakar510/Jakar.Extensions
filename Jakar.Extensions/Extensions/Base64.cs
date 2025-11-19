namespace Jakar.Extensions;


public static class Base64
{
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
        where TValue : IJsonModel<TValue>
    {
        public string ToBase64() => jsonSerializablePayload.ToBase64(Encoding.Default);
        public string ToBase64( Encoding encoding )
        {
            string temp = jsonSerializablePayload.ToJson();
            return temp.ToBase64(encoding);
        }
    }



    extension<TValue>( TValue jsonSerializablePayload )
    {
        public string ToBase64( JsonTypeInfo<TValue> info ) => jsonSerializablePayload.ToBase64(info, Encoding.Default);
        public string ToBase64( JsonTypeInfo<TValue> info, Encoding encoding )
        {
            string temp = jsonSerializablePayload.ToJson(info);
            return temp.ToBase64(encoding);
        }
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



    extension( string b64 )
    {
        public TValue JsonFromBase64String<TValue>()
            where TValue : IJsonModel<TValue> => b64.JsonFromBase64String<TValue>(Encoding.Default);
        public TValue JsonFromBase64String<TValue>( Encoding encoding )
            where TValue : IJsonModel<TValue>
        {
            byte[] bytes = b64.FromBase64String();
            string temp  = encoding.GetString(bytes);
            return temp.FromJson<TValue>();
        }
    }
}
