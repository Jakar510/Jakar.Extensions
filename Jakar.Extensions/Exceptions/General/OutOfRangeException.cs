namespace Jakar.Extensions;


public class OutOfRangeException : ArgumentOutOfRangeException
{
    public const string DEFAULT_MESSAGE = "out of range";


    public OutOfRangeException() { }
    public OutOfRangeException( string message ) : base( message ) { }
    public OutOfRangeException( string message,   Exception? inner ) : base( message, inner ) { }
    public OutOfRangeException( string paramName, object?    actualValue, string message = DEFAULT_MESSAGE ) : base( paramName, actualValue, message ) { }


    public static OutOfRangeException Create( object? actualValue, string message = DEFAULT_MESSAGE, [CallerArgumentExpression( nameof(actualValue) )] string paramName = BaseRecord.EMPTY ) => new(paramName, actualValue, message);
}
