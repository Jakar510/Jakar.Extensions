namespace Jakar.Extensions;


public class ApiDisabledException : Exception
{
    public ApiDisabledException() { }
    public ApiDisabledException( string message ) : base( message ) { }
    public ApiDisabledException( string message, Exception? inner ) : base( message, inner ) { }
}
