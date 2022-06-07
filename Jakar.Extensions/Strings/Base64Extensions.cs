#nullable enable
namespace Jakar.Extensions.Strings;


public static class Base64Extensions
{
    public static byte[] FromBase64String( this string s ) => Convert.FromBase64String(s);

    public static TResult JsonFromBase64String<TResult>( this string s, Encoding? encoding = default )
    {
        byte[] bytes = s.FromBase64String();
        string temp  = ( encoding ?? Encoding.Default ).GetString(bytes);
        return temp.FromJson<TResult>();
    }

    public static string ToBase64( this object jsonSerializablePayload )
    {
        string temp = jsonSerializablePayload.ToJson();
        return temp.ToBase64();
    }


    public static string ToBase64( this string data, Encoding? encoding = default )
    {
        byte[] payload = ( encoding ?? Encoding.Default ).GetBytes(data);

        return Convert.ToBase64String(payload);
    }

    public static MemoryStream ToStreamFromBase64String( this string s )
    {
        byte[] buffer = s.FromBase64String();
        return new MemoryStream(buffer);
    }
}
