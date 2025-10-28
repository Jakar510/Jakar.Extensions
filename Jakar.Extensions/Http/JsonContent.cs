namespace Jakar.Extensions;


public class JsonContent( string content, Encoding encoding ) : StringContent(content, encoding, MimeTypeNames.Application.JSON)
{
    public JsonContent( string content ) : this(content, Encoding.Default) { }
}
