namespace Jakar.Extensions;


public class NotLoggedInException : Exception
{
    public NotLoggedInException() { }
    public NotLoggedInException( string message ) : base( message ) { }
    public NotLoggedInException( string message, Exception inner ) : base( message, inner ) { }
}
