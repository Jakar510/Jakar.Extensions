namespace Jakar.Extensions;


public class HeaderException( HttpRequestHeader key, Type actual, params Type[] expected ) : ExpectedValueTypeException<HttpRequestHeader>(key, actual, expected)
{
    public HeaderException( HttpRequestHeader name, object value, params Type[] expected ) : this(name, value.GetType(), expected) { }
}
