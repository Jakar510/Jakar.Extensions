#nullable enable
namespace Jakar.Extensions;


/// <summary> The remote service point could not be contacted at the transport level. </summary>
public sealed class PipelineFailureException : WebException
{
    public CancellationToken Token { get; }
    public PipelineFailureException() { }
    public PipelineFailureException( string       message ) : base( message ) { }
    public PipelineFailureException( string       message, Exception         inner ) : base( message, inner ) { }
    public PipelineFailureException( WebException source,  CancellationToken token ) : this( source.Message, source, token ) { }
    public PipelineFailureException( string       message, WebException      source, CancellationToken  token ) : this( message, source ?? throw new NullReferenceException( nameof(source) ), source.Status, source.Response, token ) { }
    public PipelineFailureException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse? response ) : base( message, inner, status, response ) { }

    public PipelineFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse? response, CancellationToken token ) : base( message, inner, status, response )
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
