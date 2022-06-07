// unset


#nullable enable
namespace Jakar.Extensions.Exceptions.General;


public class UnknownErrorException : Exception
{
    public UnknownErrorException() { }
    public UnknownErrorException( string message ) : base(message) { }
    public UnknownErrorException( string message, Exception inner ) : base(message, inner) { }
}
