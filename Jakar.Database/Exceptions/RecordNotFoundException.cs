// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:38 PM

namespace Jakar.Database;


public class DuplicateRecordException : NotFoundException
{
    public DuplicateRecordException() : base() { }
    public DuplicateRecordException( string message ) : base(message) { }
    public DuplicateRecordException( string message, Exception inner ) : base(message, inner) { }
}



public class RecordNotFoundException : NotFoundException
{
    public RecordNotFoundException() : base() { }
    public RecordNotFoundException( string message ) : base(message) { }
    public RecordNotFoundException( string message, Exception inner ) : base(message, inner) { }
}



public class RecordNotFoundException<TClass> : RecordNotFoundException
    where TClass : BaseRecord
{
    public RecordNotFoundException() : base() { }
    public RecordNotFoundException( TClass record ) : base(record.ToString()) { }
    public RecordNotFoundException( string message, Exception inner ) : base(message, inner) { }
}
