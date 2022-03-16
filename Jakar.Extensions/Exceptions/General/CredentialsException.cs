namespace Jakar.Extensions.Exceptions.General;


public class CredentialsException : Exception
{
    public CredentialsException() { }
    public CredentialsException( string? user, string? password ) : base(CreateMessage(user, password)) { }
    public CredentialsException( string? user, string? password, Exception? inner ) : base(CreateMessage(user, password), inner) { }


    protected static string CreateMessage( string? user, string? password ) => $"User: \"{user?.ToUpper() ?? "null"}\" | password: \"{password?.ToUpper() ?? "null"}\"";

    public static void ThrowIfInvalid( string? user, string? password )
    {
        if ( string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) ) { throw new CredentialsException(user, password); }
    }
}
