// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:38 PM

namespace Jakar.Database;


public class RecordNotFoundException : NotFoundException
{
    public RecordNotFoundException() : base() { }
    public RecordNotFoundException( string message ) : base( message ) { }
    public RecordNotFoundException( string message, Exception inner ) : base( message, inner ) { }
}



public class RecordNotFoundException<TRecord> : RecordNotFoundException where TRecord : BaseRecord
{
    public RecordNotFoundException() : base() { }
    public RecordNotFoundException( TRecord record ) : base( record.ToString() ) { }
    public RecordNotFoundException( string  message, Exception inner ) : base( message, inner ) { }
}
