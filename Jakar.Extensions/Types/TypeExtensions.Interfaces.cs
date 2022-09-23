#nullable enable
namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    public static bool HasInterface<T>( this PropertyInfo prop ) => prop.PropertyType.HasInterface<T>();
    public static bool HasInterface<T>( this Type         type ) => type.HasInterface(typeof(T));

    public static bool HasInterface( this Type type, Type interfaceType )
    {
        Type[] interfaces = type.GetInterfaces();
        if ( interfaces.Contains(interfaceType) ) { return true; }

        return interfaces.Any(t => t == interfaceType || ( t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType ));
    }
}
