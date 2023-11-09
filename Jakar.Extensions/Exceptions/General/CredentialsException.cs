namespace Jakar.Extensions;


public class CredentialsException : Exception
{
    public CredentialsException() { }
    public CredentialsException( string? message ) : base( message ) { }
    public CredentialsException( string? message, Exception? inner ) : base( message, inner ) { }


    public static CredentialsException Create( string? user, Exception? inner = default ) => new($"User: '{user ?? "null"}'", inner);


    public static bool IsValid( [ NotNullWhen(        true ) ] string? user, [ NotNullWhen( true ) ] string? password ) => IsInvalid( user, password ) is false;
    public static bool IsInvalid( [ NotNullWhen(      true ) ] string? user, [ NotNullWhen( true ) ] string? password ) => string.IsNullOrWhiteSpace( user ) || string.IsNullOrWhiteSpace( password );
    public static void ThrowIfInvalid( [ NotNullWhen( true ) ] string? user, [ NotNullWhen( true ) ] string? password )
    {
        if ( IsInvalid( user, password ) ) { throw Create( user ); }
    }
}
