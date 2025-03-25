namespace Jakar.Extensions;


public static partial class Types
{
    public static bool HasInterface<TValue>( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type type ) => type.HasInterface( typeof(TValue) );


    public static bool HasInterface( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type type, Type interfaceType )
    {
        Type[] interfaces = type.GetInterfaces();
        if ( interfaces.Contains( interfaceType ) ) { return true; }

        return interfaces.Any( t => t == interfaceType || t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType );
    }
}
