#nullable enable
namespace Jakar.Extensions.Exceptions.Networking;


public class EmptyServerResponseException : Exception
{
    public EmptyServerResponseException() { }

    public EmptyServerResponseException( string message ) : base(message) { }

    public EmptyServerResponseException( string message, Exception inner ) : base(message, inner) { }
}
