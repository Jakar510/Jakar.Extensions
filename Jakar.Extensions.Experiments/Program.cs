using System.Reflection;
using TypeExtensions = Jakar.Extensions.TypeExtensions;


HashSet<string> set = [..typeof(TypeExtensions).GetMethods().Select( GetName )];
set.ExceptWith( typeof(object).GetMethods().Select( GetName ) );


foreach ( string name in set ) { Console.WriteLine( $"- {name}" ); }

return;
static string GetName( MethodInfo x ) => x.Name;
