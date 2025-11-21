namespace Jakar.Extensions;


public static partial class Types
{
    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool HasInterface<TValue>() => type.HasInterface(typeof(TValue));
        public bool HasInterface( Type interfaceType )
        {
            Type[] interfaces = type.GetInterfaces();
            if ( interfaces.Contains(interfaceType) ) { return true; }

            return interfaces.Any(t => t == interfaceType || ( t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType ));
        }
    }
}
