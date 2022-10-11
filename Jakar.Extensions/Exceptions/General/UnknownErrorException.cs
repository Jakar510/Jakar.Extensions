// unset


#nullable enable
namespace Jakar.Extensions;


public class UnknownErrorException : Exception
{
    public UnknownErrorException() { }
    public UnknownErrorException( string message ) : base( message ) { }
    public UnknownErrorException( string message, Exception inner ) : base( message, inner ) { }
}
