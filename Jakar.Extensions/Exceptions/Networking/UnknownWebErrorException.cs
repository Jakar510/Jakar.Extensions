#nullable enable
namespace Jakar.Extensions.Exceptions.Networking;


/// <summary>
///     A web exception of unknown type occurred.
/// </summary>
public sealed class UnknownWebErrorException : WebException
{
    public CancellationToken Token { get; }
    public UnknownWebErrorException() { }
    public UnknownWebErrorException( string       message ) : base(message) { }
    public UnknownWebErrorException( string       message, Exception         inner ) : base(message, inner) { }
    public UnknownWebErrorException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }
    public UnknownWebErrorException( string       message, WebException      source, CancellationToken  token ) : this(message, source ?? throw new NullReferenceException(nameof(source)), source.Status, source.Response, token) { }
    public UnknownWebErrorException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }

    public UnknownWebErrorException( string message, Exception inner, WebExceptionStatus status, WebResponse response, CancellationToken token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
