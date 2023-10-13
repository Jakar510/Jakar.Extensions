#nullable enable
namespace Jakar.Extensions;


public class CredentialsException : Exception
{
    public CredentialsException() { }
    public CredentialsException( string? message ) : base( message ) { }
    public CredentialsException( string? message, Exception? inner ) : base( message, inner ) { }


    public static CredentialsException Create( string? user, Exception? inner = default ) => new($"User: '{user ?? "null"}'", inner);


    public static bool IsValid( [ NotNullWhen( true ) ] string? user, [ NotNullWhen( true ) ] string? password ) => string.IsNullOrWhiteSpace( user ) is false && string.IsNullOrWhiteSpace( password ) is false;
    public static void ThrowIfInvalid( string? user, string? password )
    {
        if ( IsValid( user, password ) ) { throw Create( user ); }
    }
}
