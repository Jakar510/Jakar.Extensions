#nullable enable
namespace Jakar.Extensions.Exceptions.General;


public class RefreshDataIsNullException : Exception
{
    public RefreshDataIsNullException() { }
    public RefreshDataIsNullException( string message ) : base(message) { }
    public RefreshDataIsNullException( string message, Exception inner ) : base(message, inner) { }
}
