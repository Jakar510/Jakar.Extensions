#nullable enable
namespace Jakar.Extensions;


public class NoGraphDataException : Exception
{
    public NoGraphDataException() { }
    public NoGraphDataException( string message ) : base(message) { }
    public NoGraphDataException( string message, Exception inner ) : base(message, inner) { }
}
