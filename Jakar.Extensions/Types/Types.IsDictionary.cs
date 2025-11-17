namespace Jakar.Extensions;


public static partial class Types
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool IsDictionary() => type.HasInterface<IDictionary>() || type.HasInterface(typeof(IDictionary<,>));
        public bool IsDictionary( [NotNullWhen(true)] out Type? keyType, [NotNullWhen(true)] out Type? valueType, [NotNullWhen(true)] out bool? isKeyBuiltInType, [NotNullWhen(true)] out bool? isValueBuiltInType )
        {
            if ( type.IsDictionary(out IReadOnlyList<Type>? itemTypes) )
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
        public bool IsDictionary( [NotNullWhen(true)] out Type? keyType, [NotNullWhen(true)] out Type? valueType )
        {
            if ( type.IsDictionary(out IReadOnlyList<Type>? itemTypes) )
            {
                keyType   = itemTypes[0];
                valueType = itemTypes[1];
                return true;
            }

            valueType = null;
            keyType   = null;
            return false;
        }
        public bool IsDictionary( [NotNullWhen(true)] out IReadOnlyList<Type>? itemTypes )
        {
            if ( type.IsGenericType && type.IsDictionary() )
            {
                itemTypes = type.GetGenericArguments();
                return true;
            }

            foreach ( Type interfaceType in type.GetInterfaces() )
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
    }



    extension( Type   type )
    {
        public bool IsDictionaryEntry() => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DictionaryEntry);
        public bool IsKeyValuePair()    => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
    }



    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type classType )
    {
        public bool IsKeyValuePair( [NotNullWhen(true)] out Type? keyType, [NotNullWhen(true)] out Type? valueType )
        {
            if ( classType.IsKeyValuePair(out IReadOnlyList<Type>? itemTypes) )
            {
                keyType   = itemTypes[0];
                valueType = itemTypes[1];
                return true;
            }

            valueType = null;
            keyType   = null;
            return false;
        }
        public bool IsKeyValuePair( [NotNullWhen(true)] out IReadOnlyList<Type>? itemTypes )
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
}
