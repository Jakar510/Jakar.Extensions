#nullable enable
namespace Jakar.Extensions;


/// <summary> The request was canceled, the System.Net.WebRequest.Abort method was called, or an unclassifiable error occurred. This is the default value for System.Net.WebException.Status. </summary>
public sealed class ReceiveFailureException : WebException
{
    public CancellationToken Token { get; }
    public ReceiveFailureException() { }
    public ReceiveFailureException( string       message ) : base( message ) { }
    public ReceiveFailureException( string       message, Exception         inner ) : base( message, inner ) { }
    public ReceiveFailureException( WebException source,  CancellationToken token ) : this( source.Message, source, token ) { }
    public ReceiveFailureException( string       message, WebException      source, CancellationToken  token ) : this( message, source ?? throw new NullReferenceException( nameof(source) ), source.Status, source.Response, token ) { }
    public ReceiveFailureException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse? response ) : base( message, inner, status, response ) { }

    public ReceiveFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse? response, CancellationToken token ) : base( message, inner, status, response )
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
