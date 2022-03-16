namespace Jakar.Extensions.Exceptions.Networking;


public sealed class RequestAbortedException : WebException
{
    public CancellationToken Token { get; }
    public RequestAbortedException() { }
    public RequestAbortedException( string       message ) : base(message) { }
    public RequestAbortedException( string       message, Exception         inner ) : base(message, inner) { }
    public RequestAbortedException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }

    public RequestAbortedException( string message, WebException source, CancellationToken token ) : this(message,
                                                                                                          source ?? throw new NullReferenceException(nameof(source)),
                                                                                                          source.Status,
                                                                                                          source.Response,
                                                                                                          token) { }

    public RequestAbortedException( string message, Exception inner, WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }

    public RequestAbortedException( string             message,
                                    Exception          inner,
                                    WebExceptionStatus status,
                                    WebResponse        response,
                                    CancellationToken  token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
