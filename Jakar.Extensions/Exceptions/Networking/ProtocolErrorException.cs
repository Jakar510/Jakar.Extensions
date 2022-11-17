#nullable enable
namespace Jakar.Extensions;


/// <summary> A response was received from server but was indicated a protocol-level error. </summary>
public sealed class ProtocolErrorException : WebException
{
    public CancellationToken Token { get; }
    public ProtocolErrorException() { }
    public ProtocolErrorException( string       message ) : base( message ) { }
    public ProtocolErrorException( string       message, Exception         inner ) : base( message, inner ) { }
    public ProtocolErrorException( WebException source,  CancellationToken token ) : this( source.Message, source, token ) { }
    public ProtocolErrorException( string       message, WebException      source, CancellationToken  token ) : this( message, source ?? throw new NullReferenceException( nameof(source) ), source.Status, source.Response, token ) { }
    public ProtocolErrorException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse? response ) : base( message, inner, status, response ) { }

    public ProtocolErrorException( string message, Exception inner, WebExceptionStatus status, WebResponse? response, CancellationToken token ) : base( message, inner, status, response )
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
