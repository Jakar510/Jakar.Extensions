// Jakar.Extensions :: Jakar.Extensions
// 09/22/2022  4:17 PM

namespace Jakar.Extensions;


public class NotFoundException : Exception
{
    public NotFoundException() : base() { }
    public NotFoundException( string message ) : base(message) { }
    public NotFoundException( string message, Exception inner ) : base(message, inner) { }
}