namespace Jakar.Extensions;


public static partial class Types
{
    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool HasInterface<TValue>()
        {
            ReadOnlySpan<Type> interfaces = type.GetInterfaces();
            if ( interfaces.Contains(typeof(TValue)) ) { return true; }

            return interfaces.Any(static t => t == typeof(TValue) || ( t.IsGenericType && t.GetGenericTypeDefinition() == typeof(TValue) ));
        }

        public bool HasInterface( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type interfaceType )
        {
            ReadOnlySpan<Type> interfaces = type.GetInterfaces();
            if ( interfaces.Contains(interfaceType) ) { return true; }

            foreach ( Type t in interfaces )
            {
                if ( t == interfaceType ) { return true; }
                
                if ( t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType ) { return true; }
            }

            return false;
        }
    }
}
