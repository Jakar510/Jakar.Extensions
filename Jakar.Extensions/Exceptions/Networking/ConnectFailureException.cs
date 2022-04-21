namespace Jakar.Extensions.Exceptions.Networking;


/// <summary>
///     The remote service point could not be contacted at the transport level.
/// </summary>
public sealed class ConnectFailureException : WebException
{
    public CancellationToken Token { get; }
    public ConnectFailureException() { }
    public ConnectFailureException( string       message ) : base(message) { }
    public ConnectFailureException( string       message, Exception         inner ) : base(message, inner) { }
    public ConnectFailureException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }
    public ConnectFailureException( string       message, WebException      source, CancellationToken  token ) : this(message, source ?? throw new NullReferenceException(nameof(source)), source.Status, source.Response, token) { }
    public ConnectFailureException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }

    public ConnectFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse response, CancellationToken token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
