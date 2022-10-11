// Jakar.Extensions :: Jakar.Extensions
// 09/22/2022  4:13 PM

namespace Jakar.Extensions;


public sealed class NotAcceptableException : Exception
{
    public NotAcceptableException() { }
    public NotAcceptableException( string message ) : base( message ) { }
    public NotAcceptableException( string message, Exception inner ) : base( message, inner ) { }
}
