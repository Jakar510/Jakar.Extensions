// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:38 PM

namespace Jakar.Database;


public class RecordNotFoundException : Exception
{
    public RecordNotFoundException() : base() { }
    public RecordNotFoundException( string message ) : base(message) { }
    public RecordNotFoundException( string message, Exception inner ) : base(message, inner) { }
}
