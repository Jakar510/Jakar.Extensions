namespace Jakar.Extensions;


public static partial class Types
{
    public static MethodDetails    MethodInfo( this       MethodBase    method )    => new(method);
    public static ParameterDetails GetParameterInfo( this ParameterInfo parameter ) => new(parameter);


    extension( MethodBase method )
    {
        public string  MethodName()      => method.Name;
        public string  MethodSignature() => $"{method.Name}( {string.Join(", ", method.GetParameters().Select(static x => x.ParameterType.FullName))} )";
        public string? MethodClass()     => method.DeclaringType?.FullName;
    }
}
