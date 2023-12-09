namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    public static bool HasInterface<T>(
    #if NET6_0_OR_GREATER
        [ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces ) ]
    #endif
        this Type type
    ) => type.HasInterface( typeof(T) );


    public static bool HasInterface(
    #if NET6_0_OR_GREATER
        [ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces ) ]
    #endif
        this Type type,
        Type interfaceType
    )
    {
        Type[] interfaces = type.GetInterfaces();
        if ( interfaces.Contains( interfaceType ) ) { return true; }

        return interfaces.Any( t => t == interfaceType || t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType );
    }
}
