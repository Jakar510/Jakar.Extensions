namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    public static bool IsSet( this PropertyInfo propertyInfo ) => propertyInfo.PropertyType.IsSet();
    public static bool IsSet( this Type         type )         => type.HasInterface( typeof(ISet<>) );

    public static bool IsSet( this PropertyInfo propertyInfo, [ NotNullWhen( true ) ] out Type? itemType, [ NotNullWhen( true ) ] out bool? isBuiltInType ) => propertyInfo.PropertyType.IsCollection( out itemType, out isBuiltInType );

    public static bool IsSet( this Type classType, [ NotNullWhen( true ) ] out Type? itemType, [ NotNullWhen( true ) ] out bool? isBuiltInType )
    {
        if ( classType.IsSet( out IReadOnlyList<Type>? itemTypes ) )
        {
            itemType      = itemTypes[0];
            isBuiltInType = itemType.IsBuiltInType();
            return true;
        }

        isBuiltInType = null;
        itemType      = null;
        return false;
    }

    public static bool IsSet( this PropertyInfo propertyInfo, [ NotNullWhen( true ) ] out Type? itemType ) => propertyInfo.PropertyType.IsSet( out itemType );

    public static bool IsSet( this Type propertyType, [ NotNullWhen( true ) ] out Type? itemType )
    {
        if ( propertyType.IsSet( out IReadOnlyList<Type>? itemTypes ) )
        {
            itemType = itemTypes[0];
            return true;
        }

        itemType = null;
        return false;
    }

    public static bool IsSet( this Type classType, [ NotNullWhen( true ) ] out IReadOnlyList<Type>? itemTypes )
    {
        if ( classType.IsGenericType && classType.IsSet() )
        {
            itemTypes = classType.GetGenericArguments();
            return true;
        }

        foreach ( Type interfaceType in classType.GetInterfaces() )
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
