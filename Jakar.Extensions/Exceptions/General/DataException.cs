// Jakar.Extensions :: Jakar.Extensions
// 09/22/2022  4:21 PM

namespace Jakar.Extensions;


public class DataException : Exception
{
    public DataException() { }
    public DataException( string message ) : base( message ) { }
    public DataException( string message, Exception inner ) : base( message, inner ) { }
}
