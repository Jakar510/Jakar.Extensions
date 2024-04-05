using System.Net.Http;



namespace Jakar.Extensions.Experiments;


internal static class PasswordChecker
{
    private static readonly Uri _source = new("https://raw.githubusercontent.com/danielmiessler/SecLists/master/Passwords/Common-Credentials/10-million-password-list-top-1000000.txt");


    public static async ValueTask Run()
    {
        using HttpClient client  = new();
        string           content = await client.GetStringAsync( _source ).ConfigureAwait( false );
        LocalFile        raw     = "raw.txt";
        await raw.WriteAsync( content ).ConfigureAwait( false );

        string[]  passwords    = content.Split( '\n', StringSplitOptions.RemoveEmptyEntries );
        LocalFile passwordJson = "passwords.json";
        await passwordJson.WriteAsync( passwords.ToPrettyJson() ).ConfigureAwait( false );

        List<string> results = [];
        CheckPasswords( passwords, results );

        LocalFile valid = "valid.json";
        await valid.WriteAsync( results.ToPrettyJson() ).ConfigureAwait( false );
        Console.WriteLine( $"Valid passwords: {results.Count}" );
        Console.WriteLine( $"Valid passwords file: {valid.FullPath}" );
    }
    private static void CheckPasswords( scoped in ReadOnlySpan<string> passwords, in List<string> results )
    {
        PasswordValidator validator = PasswordValidator.Default;
        results.EnsureCapacity( passwords.Length );

        foreach ( string password in passwords )
        {
            if ( validator.Validate( password ) ) { results.Add( password ); }
        }
    }
}
