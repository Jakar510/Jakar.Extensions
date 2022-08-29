#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     A Server certificate could not be verified.
/// </summary>
public sealed class TrustFailureException : WebException
{
    public CancellationToken Token { get; }
    public TrustFailureException() { }
    public TrustFailureException( string       message ) : base(message) { }
    public TrustFailureException( string       message, Exception         inner ) : base(message, inner) { }
    public TrustFailureException( WebException source,  CancellationToken token ) : this(source.Message, source, token) { }
    public TrustFailureException( string       message, WebException      source, CancellationToken  token ) : this(message, source ?? throw new NullReferenceException(nameof(source)), source.Status, source.Response, token) { }
    public TrustFailureException( string       message, Exception         inner,  WebExceptionStatus status, WebResponse? response ) : base(message, inner, status, response) { }

    public TrustFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse? response, CancellationToken token ) : base(message, inner, status, response)
    {
        Token         = token;
        Data["token"] = Token.ToString();
    }
}
