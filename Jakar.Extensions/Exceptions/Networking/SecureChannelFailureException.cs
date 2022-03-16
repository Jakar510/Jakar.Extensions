namespace Jakar.Extensions.Exceptions.Networking;


/// <summary>
///  An error occurred while establishing a connection using SSL.
/// </summary>
public sealed class SecureChannelFailureException : WebException
{
    public CancellationToken Token { get; }
    public SecureChannelFailureException() { }
    public SecureChannelFailureException( string       message ) : base(message) { }
    public SecureChannelFailureException( string       message, Exception         inner ) : base(message, inner) { }
    public SecureChannelFailureException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }

    public SecureChannelFailureException( string message, WebException source, CancellationToken token ) : this(message,
                                                                                                                source ?? throw new NullReferenceException(nameof(source)),
                                                                                                                source.Status,
                                                                                                                source.Response,
                                                                                                                token) { }

    public SecureChannelFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }

    public SecureChannelFailureException( string             message,
                                          Exception          inner,
                                          WebExceptionStatus status,
                                          WebResponse        response,
                                          CancellationToken  token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
