namespace Jakar.Extensions.Types;


public static partial class TypeExtensions
{
    public static ParameterDetails GetParameterInfo( this ParameterInfo parameter ) => new(parameter);
    public static MethodDetails    MethodInfo( this       MethodBase    method )    => new(method);


    public static string  MethodName( this      MethodBase method ) => method.Name;
    public static string  MethodSignature( this MethodBase method ) => $"{method.Name}( {string.Join(", ", method.GetParameters().Select(x => x.ParameterType.FullName))} )";
    public static string? MethodClass( this     MethodBase method ) => method.DeclaringType?.FullName;
}
