#nullable enable
namespace Jakar.Extensions.Exceptions.General;


public class NoGraphDataException : Exception
{
    public NoGraphDataException() { }
    public NoGraphDataException( string message ) : base(message) { }
    public NoGraphDataException( string message, Exception inner ) : base(message, inner) { }
}
