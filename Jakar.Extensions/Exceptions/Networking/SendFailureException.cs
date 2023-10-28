namespace Jakar.Extensions;


/// <summary> A complete response was not received from the remote server. </summary>
public sealed class SendFailureException : WebException
{
    public CancellationToken Token { get; }
    public SendFailureException() { }
    public SendFailureException( string       message ) : base( message ) { }
    public SendFailureException( string       message, Exception         inner ) : base( message, inner ) { }
    public SendFailureException( WebException source,  CancellationToken token ) : this( source.Message, source, token ) { }
    public SendFailureException( string       message, WebException      source, CancellationToken  token ) : this( message, source ?? throw new NullReferenceException( nameof(source) ), source.Status, source.Response, token ) { }
    public SendFailureException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse? response ) : base( message, inner, status, response ) { }

    public SendFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse? response, CancellationToken token ) : base( message, inner, status, response )
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
