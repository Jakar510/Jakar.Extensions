namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static bool IsDictionary( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type type ) => type.HasInterface<IDictionary>() || type.HasInterface( typeof(IDictionary<,>) );


    public static bool IsDictionary( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type classType, [NotNullWhen( true )] out Type? keyType, [NotNullWhen( true )] out Type? valueType, [NotNullWhen( true )] out bool? isKeyBuiltInType, [NotNullWhen( true )] out bool? isValueBuiltInType )
    {
        if ( classType.IsDictionary( out IReadOnlyList<Type>? itemTypes ) )
        {
            keyType            = itemTypes[0];
            valueType          = itemTypes[1];
            isKeyBuiltInType   = keyType.IsBuiltInType();
            isValueBuiltInType = valueType.IsBuiltInType();
            return true;
        }

        isKeyBuiltInType   = null;
        isValueBuiltInType = null;
        valueType          = null;
        keyType            = null;
        return false;
    }


    public static bool IsDictionary( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type classType, [NotNullWhen( true )] out Type? keyType, [NotNullWhen( true )] out Type? valueType )
    {
        if ( classType.IsDictionary( out IReadOnlyList<Type>? itemTypes ) )
        {
            keyType   = itemTypes[0];
            valueType = itemTypes[1];
            return true;
        }

        valueType = null;
        keyType   = null;
        return false;
    }


    public static bool IsDictionary( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type classType, [NotNullWhen( true )] out IReadOnlyList<Type>? itemTypes )
    {
        if ( classType.IsGenericType && classType.IsDictionary() )
        {
            itemTypes = classType.GetGenericArguments();
            return true;
        }

        foreach ( Type interfaceType in classType.GetInterfaces() )
        {
            if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>) )
            {
                itemTypes = interfaceType.GetGenericArguments();
                return true;
            }
        }

        itemTypes = null;
        return false;
    }


    public static bool IsDictionaryEntry( this Type type )      => type.IsGenericType      && type.GetGenericTypeDefinition()      == typeof(DictionaryEntry);
    public static bool IsKeyValuePair( this    Type classType ) => classType.IsGenericType && classType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);


    public static bool IsKeyValuePair( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type classType, [NotNullWhen( true )] out Type? keyType, [NotNullWhen( true )] out Type? valueType )
    {
        if ( classType.IsKeyValuePair( out IReadOnlyList<Type>? itemTypes ) )
        {
            keyType   = itemTypes[0];
            valueType = itemTypes[1];
            return true;
        }

        valueType = null;
        keyType   = null;
        return false;
    }

    public static bool IsKeyValuePair( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.Interfaces )] this Type classType, [NotNullWhen( true )] out IReadOnlyList<Type>? itemTypes )
    {
        if ( classType.IsKeyValuePair() )
        {
            itemTypes = classType.GetGenericArguments();
            return true;
        }

        foreach ( Type interfaceType in classType.GetInterfaces() )
        {
            if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>) )
            {
                itemTypes = interfaceType.GetGenericArguments();
                return true;
            }
        }

        itemTypes = null;
        return false;
    }
}
