#nullable enable
namespace Jakar.Extensions.Exceptions.General;


public class IncompleteDataException : Exception
{
    public IncompleteDataException() { }
    public IncompleteDataException( string message ) : base(message) { }
    public IncompleteDataException( string message, Exception innerException ) : base(message, innerException) { }
}
