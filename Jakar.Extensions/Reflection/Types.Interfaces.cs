namespace Jakar.Extensions;


public static partial class Types
{
    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool HasInterface<TValue>()
        {
            ReadOnlySpan<Type> interfaces = type.GetInterfaces();

            // A plain loop rather than a lambda: a lambda inside this DynamicallyAccessedMembers-annotated extension block is IL2111.
            foreach ( Type t in interfaces )
            {
                if ( t == typeof(TValue) ) { return true; }

                if ( t.IsGenericType && t.GetGenericTypeDefinition() == typeof(TValue) ) { return true; }
            }

            return false;
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
