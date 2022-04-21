namespace Jakar.Extensions.Types;


public static partial class TypeExtensions
{
    public static bool IsList( this PropertyInfo propertyInfo ) => propertyInfo.PropertyType.IsList();
    public static bool IsList( this Type         type ) => type.HasInterface<IList>() || type.HasInterface(typeof(IList<>));

    public static bool IsList( this PropertyInfo propertyInfo, [NotNullWhen(true)] out Type? itemType, [NotNullWhen(true)] out bool? isBuiltInType ) => propertyInfo.PropertyType.IsList(out itemType, out isBuiltInType);

    public static bool IsList( this Type classType, [NotNullWhen(true)] out Type? itemType, [NotNullWhen(true)] out bool? isBuiltInType )
    {
        if ( classType.IsList(out IReadOnlyList<Type>? itemTypes) )
        {
            itemType      = itemTypes[0];
            isBuiltInType = itemType.IsBuiltInType();
            return true;
        }

        isBuiltInType = null;
        itemType      = null;
        return false;
    }

    public static bool IsList( this PropertyInfo propertyInfo, [NotNullWhen(true)] out Type? itemType ) => propertyInfo.PropertyType.IsList(out itemType);

    public static bool IsList( this Type propertyType, [NotNullWhen(true)] out Type? itemType )
    {
        if ( propertyType.IsList(out IReadOnlyList<Type>? itemTypes) )
        {
            itemType = itemTypes[0];
            return true;
        }

        itemType = null;
        return false;
    }

    public static bool IsList( this Type classType, [NotNullWhen(true)] out IReadOnlyList<Type>? itemTypes )
    {
        if ( classType.IsGenericType && classType.IsList() )
        {
            itemTypes = classType.GetGenericArguments();
            return true;
        }

        foreach ( Type interfaceType in classType.GetInterfaces() )
        {
            if ( !interfaceType.IsGenericType ) { continue; }

            if ( interfaceType == typeof(IList<>) )
            {
                itemTypes = interfaceType.GetGenericArguments();
                return true;
            }

            if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>) )
            {
                itemTypes = interfaceType.GetGenericArguments();
                return true;
            }
        }

        itemTypes = null;
        return false;
    }
}
