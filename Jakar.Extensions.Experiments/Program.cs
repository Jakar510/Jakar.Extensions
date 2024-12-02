/*
HashSet<string> set = [..typeof(TypeExtensions).GetMethods().Select( GetName )];
set.ExceptWith( typeof(object).GetMethods().Select( GetName ) );


foreach ( string name in set ) { Console.WriteLine( $"- {name}" ); }

return;
static string GetName( MethodInfo x ) => x.Name;
*/


/*
const string VALUE    = "Hello, World!";
Encoding     encoding = Encoding.Default;
string       base64   = Convert.ToBase64String( encoding.GetBytes( VALUE ) );
Console.WriteLine( string.Equals( encoding.GetString( DataProtector.GetBytes( base64, encoding ) ), VALUE ) );
*/


Console.WriteLine( DateTimeOffset.UtcNow.ToString() );


foreach ( Status status in Enum.GetValues<Status>() )
{
    Console.WriteLine( $"public static Error {status}( in StringValues errors = default, string? instance = null, string? detail = null, string? title = null, string type = {status.ToString().ToSnakeCase().ToUpper()}_TYPE ) => new(Status.{status}, type, title ?? Titles.{status}, detail, instance, errors);" );
    
    /*
     Console.WriteLine( $"""
                        public const string {status.ToString().ToSnakeCase().ToUpper()}_TYPE = "{status}";
                        """ );
    */
}


foreach ( Status status in Enum.GetValues<Status>() )
{
    // Console.WriteLine( $"public static Error {status}( in StringValues errors = default, string? instance = null, string? detail = null, string? title = null, string type = {status.ToString().ToSnakeCase().ToUpper()}_TYPE ) => new(Status.{status}, type, title ?? Titles.Validation, detail, instance, errors);" );
    Console.WriteLine( $$"""
                         string {{status}} { get; }
                         """ );
}



foreach ( Status status in Enum.GetValues<Status>() )
{
    // Console.WriteLine( $"public static Error {status}( in StringValues errors = default, string? instance = null, string? detail = null, string? title = null, string type = {status.ToString().ToSnakeCase().ToUpper()}_TYPE ) => new(Status.{status}, type, title ?? Titles.Validation, detail, instance, errors);" );
    Console.WriteLine( $$"""
                         string {{status}} => "{{status}}";
                         """ );
}
