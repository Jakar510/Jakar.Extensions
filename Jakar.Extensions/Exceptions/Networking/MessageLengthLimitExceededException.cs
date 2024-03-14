namespace Jakar.Extensions;


/// <summary> A complete response was not received from the remote server. </summary>
public sealed class MessageLengthLimitExceededException : WebException
{
    public CancellationToken Token { get; }
    public MessageLengthLimitExceededException() { }
    public MessageLengthLimitExceededException( string       message ) : base( message ) { }
    public MessageLengthLimitExceededException( string       message, Exception         inner ) : base( message, inner ) { }
    public MessageLengthLimitExceededException( WebException source,  CancellationToken token ) : this( source.Message, source, token ) { }
    public MessageLengthLimitExceededException( string       message, WebException      source, CancellationToken  token ) : this( message, source ?? throw new NullReferenceException( nameof(source) ), source.Status, source.Response, token ) { }
    public MessageLengthLimitExceededException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse? response ) : base( message, inner, status, response ) { }

    public MessageLengthLimitExceededException( string message, Exception inner, WebExceptionStatus status, WebResponse? response, CancellationToken token ) : base( message, inner, status, response )
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
