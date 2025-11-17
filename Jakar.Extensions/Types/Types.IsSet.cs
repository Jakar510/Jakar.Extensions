namespace Jakar.Extensions;


public static partial class Types
{
    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool IsSet() => type.HasInterface(typeof(ISet<>));
        public bool IsSet( [NotNullWhen(true)] out Type? itemType, [NotNullWhen(true)] out bool? isBuiltInType )
        {
            if ( type.IsSet(out IReadOnlyList<Type>? itemTypes) )
            {
                itemType      = itemTypes[0];
                isBuiltInType = itemType.IsBuiltInType();
                return true;
            }

            isBuiltInType = null;
            itemType      = null;
            return false;
        }
        public bool IsSet( [NotNullWhen(true)] out Type? itemType )
        {
            if ( type.IsSet(out IReadOnlyList<Type>? itemTypes) )
            {
                itemType = itemTypes[0];
                return true;
            }

            itemType = null;
            return false;
        }
        public bool IsSet( [NotNullWhen(true)] out IReadOnlyList<Type>? itemTypes )
        {
            if ( type.IsGenericType && type.IsSet() )
            {
                itemTypes = type.GetGenericArguments();
                return true;
            }

            foreach ( Type interfaceType in type.GetInterfaces() )
            {
                if ( interfaceType == typeof(ISet<>) )
                {
                    itemTypes = interfaceType.GetGenericArguments();
                    return true;
                }

                if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISet<>) )
                {
                    itemTypes = interfaceType.GetGenericArguments();
                    return true;
                }
            }

            itemTypes = null;
            return false;
        }
    }
}
