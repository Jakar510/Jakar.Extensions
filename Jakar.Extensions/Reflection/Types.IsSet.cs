namespace Jakar.Extensions;


public static partial class Types
{
    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool IsSet() => type.HasInterface(typeof(ISet<>));

        public bool IsSet( [NotNullWhen(true)] out Type? itemType, [NotNullWhen(true)] out bool? isBuiltInType )
        {
            if ( type.IsSet(out ReadOnlySpan<Type> arguments) )
            {
                itemType      = arguments[0];
                isBuiltInType = itemType.IsAnyBuiltInType();
                return true;
            }

            isBuiltInType = null;
            itemType      = null;
            return false;
        }

        public bool IsSet( [NotNullWhen(true)] out Type? itemType )
        {
            if ( type.IsSet(out ReadOnlySpan<Type> arguments) )
            {
                itemType = arguments[0];
                return true;
            }

            itemType = null;
            return false;
        }

        public bool IsSet( out ReadOnlySpan<Type> arguments )
        {
            if ( type.IsGenericType && type.IsSet() )
            {
                arguments = type.GetGenericArguments();
                return true;
            }

            foreach ( Type interfaceType in type.GetInterfaces() )
            {
                if ( interfaceType == typeof(ISet<>) )
                {
                    arguments = interfaceType.GetGenericArguments();
                    return true;
                }

                if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISet<>) )
                {
                    arguments = interfaceType.GetGenericArguments();
                    return true;
                }
            }

            arguments = null;
            return false;
        }
    }
}
