namespace Jakar.Extensions;


public static partial class Types
{
    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool IsList() => type.HasInterface<IList>() || type.HasInterface(typeof(IList<>));
        public bool IsList( [NotNullWhen(true)] out Type? itemType, [NotNullWhen(true)] out bool? isBuiltInType )
        {
            if ( type.IsList(out ReadOnlySpan<Type> arguments) )
            {
                itemType      = arguments[0];
                isBuiltInType = itemType.IsBuiltInType();
                return true;
            }

            isBuiltInType = null;
            itemType      = null;
            return false;
        }
        public bool IsList( [NotNullWhen(true)] out Type? itemType )
        {
            if ( type.IsList(out ReadOnlySpan<Type> arguments) )
            {
                itemType = arguments[0];
                return true;
            }

            itemType = null;
            return false;
        }
        public bool IsList( [NotNullWhen(true)] out ReadOnlySpan<Type> arguments )
        {
            if ( type.IsGenericType && type.IsList() )
            {
                arguments = type.GetGenericArguments();
                return true;
            }

            foreach ( Type interfaceType in type.GetInterfaces() )
            {
                if ( !interfaceType.IsGenericType ) { continue; }

                if ( interfaceType == typeof(IList<>) )
                {
                    arguments = interfaceType.GetGenericArguments();
                    return true;
                }

                if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>) )
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
