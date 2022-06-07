#nullable enable
namespace Jakar.Extensions.Exceptions.Networking;


public class HeaderException : ExpectedValueTypeException<HttpRequestHeader>
{
    public HeaderException( HttpRequestHeader name, object value,  params Type[] expected ) : this(name, value.GetType(), expected) { }
    public HeaderException( HttpRequestHeader key,  Type   actual, params Type[] expected ) : base(key, actual, expected) { }
}
