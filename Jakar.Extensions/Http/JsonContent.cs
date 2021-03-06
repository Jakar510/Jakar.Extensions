using System.Net.Http;


namespace Jakar.Extensions.Http;


public class JsonContent : StringContent
{
    public JsonContent( string content ) : this(content, Encoding.Default) { }
    public JsonContent( string content, Encoding encoding ) : base(content, encoding, MimeTypeNames.Application.JSON) { }
}
