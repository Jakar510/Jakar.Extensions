#nullable enable
namespace Jakar.Extensions;


/// <summary> A web exception of unknown type occurred. </summary>
public sealed class ActivelyRefusedConnectionException : WebException
{
    public const string            REFUSED = "No connection could be made because the target machine actively refused it";
    public       CancellationToken Token { get; }
    public ActivelyRefusedConnectionException() { }
    public ActivelyRefusedConnectionException( string       message ) : base( message ) { }
    public ActivelyRefusedConnectionException( string       message, Exception         inner ) : base( message, inner ) { }
    public ActivelyRefusedConnectionException( WebException source,  CancellationToken token ) : this( source.Message, source, token ) { }
    public ActivelyRefusedConnectionException( string       message, WebException      source, CancellationToken  token ) : this( message, source ?? throw new NullReferenceException( nameof(source) ), source.Status, source.Response, token ) { }
    public ActivelyRefusedConnectionException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse? response ) : base( message, inner, status, response ) { }

    public ActivelyRefusedConnectionException( string message, Exception inner, WebExceptionStatus status, WebResponse? response, CancellationToken token ) : base( message, inner, status, response )
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
