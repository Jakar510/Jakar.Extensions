#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     The connection was prematurely closed.
/// </summary>
public sealed class ConnectionClosedException : WebException
{
    public CancellationToken Token { get; }
    public ConnectionClosedException() { }
    public ConnectionClosedException( string       message ) : base(message) { }
    public ConnectionClosedException( string       message, Exception         inner ) : base(message, inner) { }
    public ConnectionClosedException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }
    public ConnectionClosedException( string       message, WebException      source, CancellationToken  token ) : this(message, source ?? throw new NullReferenceException(nameof(source)), source.Status, source.Response, token) { }
    public ConnectionClosedException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }

    public ConnectionClosedException( string message, Exception inner, WebExceptionStatus status, WebResponse response, CancellationToken token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
