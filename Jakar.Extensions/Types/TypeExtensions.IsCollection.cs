namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    public static bool IsCollection(
    #if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )]
    #endif
        this Type type
    ) => type.HasInterface<ICollection>() || type.HasInterface( typeof(ICollection<>) );


    public static bool IsCollection(
    #if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )]
    #endif
        this Type classType,
        [NotNullWhen( true )] out Type? itemType,
        [NotNullWhen( true )] out bool? isBuiltInType
    )
    {
        if ( classType.IsCollection( out IReadOnlyList<Type>? itemTypes ) )
        {
            itemType      = itemTypes[0];
            isBuiltInType = itemType.IsBuiltInType();
            return true;
        }

        isBuiltInType = null;
        itemType      = null;
        return false;
    }


    public static bool IsCollection(
    #if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )]
    #endif
        this Type propertyType,
        [NotNullWhen( true )] out Type? itemType
    )
    {
        if ( propertyType.IsCollection( out IReadOnlyList<Type>? itemTypes ) )
        {
            itemType = itemTypes[0];
            return true;
        }

        itemType = null;
        return false;
    }

    public static bool IsCollection(
    #if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )]
    #endif
        this Type classType,
        [NotNullWhen( true )] out IReadOnlyList<Type>? itemTypes
    )
    {
        if ( classType.IsGenericType && classType.IsCollection() )
        {
            itemTypes = classType.GetGenericArguments();
            return true;
        }

        foreach ( Type interfaceType in classType.GetInterfaces() )
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
