namespace Jakar.Extensions.Exceptions.Networking;


/// <summary>
/// The name resolver service could not resolve the host name.
/// </summary>
public sealed class NameResolutionException : WebException
{
    public CancellationToken Token { get; }
    public NameResolutionException() { }
    public NameResolutionException( string       message ) : base(message) { }
    public NameResolutionException( string       message, Exception         inner ) : base(message, inner) { }
    public NameResolutionException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }

    public NameResolutionException( string message, WebException source, CancellationToken token ) : this(message,
                                                                                                          source ?? throw new NullReferenceException(nameof(source)),
                                                                                                          source.Status,
                                                                                                          source.Response,
                                                                                                          token) { }

    public NameResolutionException( string message, Exception inner, WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }

    public NameResolutionException( string             message,
                                    Exception          inner,
                                    WebExceptionStatus status,
                                    WebResponse        response,
                                    CancellationToken  token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
