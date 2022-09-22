#nullable enable
namespace Jakar.Extensions;


public class IncompleteDataException : DataException
{
    public IncompleteDataException() { }
    public IncompleteDataException( string message ) : base(message) { }
    public IncompleteDataException( string message, Exception innerException ) : base(message, innerException) { }
}
