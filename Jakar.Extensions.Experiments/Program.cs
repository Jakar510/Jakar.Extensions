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
