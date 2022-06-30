#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     The response was not a valid HTTP response.
/// </summary>
public sealed class ServerProtocolViolationException : WebException
{
    public CancellationToken Token { get; }
    public ServerProtocolViolationException() { }
    public ServerProtocolViolationException( string       message ) : base(message) { }
    public ServerProtocolViolationException( string       message, Exception         inner ) : base(message, inner) { }
    public ServerProtocolViolationException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }
    public ServerProtocolViolationException( string       message, WebException      source, CancellationToken  token ) : this(message, source ?? throw new NullReferenceException(nameof(source)), source.Status, source.Response, token) { }
    public ServerProtocolViolationException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }

    public ServerProtocolViolationException( string message, Exception inner, WebExceptionStatus status, WebResponse response, CancellationToken token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
