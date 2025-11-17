namespace Jakar.Extensions;


public static partial class Types
{
    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool IsCollection() => type.HasInterface<ICollection>() || type.HasInterface(typeof(ICollection<>));
        public bool IsCollection( [NotNullWhen(true)] out Type? itemType, [NotNullWhen(true)] out bool? isBuiltInType )
        {
            if ( type.IsCollection(out IReadOnlyList<Type>? itemTypes) )
            {
                itemType      = itemTypes[0];
                isBuiltInType = itemType.IsBuiltInType();
                return true;
            }

            isBuiltInType = null;
            itemType      = null;
            return false;
        }
        public bool IsCollection( [NotNullWhen(true)] out Type? itemType )
        {
            if ( type.IsCollection(out IReadOnlyList<Type>? itemTypes) )
            {
                itemType = itemTypes[0];
                return true;
            }

            itemType = null;
            return false;
        }
        public bool IsCollection( [NotNullWhen(true)] out IReadOnlyList<Type>? itemTypes )
        {
            if ( type.IsGenericType && type.IsCollection() )
            {
                itemTypes = type.GetGenericArguments();
                return true;
            }

            foreach ( Type interfaceType in type.GetInterfaces() )
            {
                if ( !interfaceType.IsGenericType ) { continue; }

                if ( interfaceType == typeof(ICollection<>) )
                {
                    itemTypes = interfaceType.GetGenericArguments();
                    return true;
                }

                if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>) )
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
