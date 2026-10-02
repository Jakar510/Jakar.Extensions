namespace Jakar.Extensions;


public static partial class Types
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type )
    {
        public bool IsDictionary() => type.HasInterface<IDictionary>() || type.HasInterface(typeof(IDictionary<,>));

        public bool IsDictionary( [NotNullWhen(true)] out Type? keyType, [NotNullWhen(true)] out Type? valueType, [NotNullWhen(true)] out bool? isKeyBuiltInType, [NotNullWhen(true)] out bool? isValueBuiltInType )
        {
            if ( type.IsDictionary(out ReadOnlySpan<Type> arguments) )
            {
                keyType            = arguments[0];
                valueType          = arguments[1];
                isKeyBuiltInType   = keyType.IsAnyBuiltInType();
                isValueBuiltInType = valueType.IsAnyBuiltInType();
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
            if ( type.IsDictionary(out ReadOnlySpan<Type> arguments) )
            {
                keyType   = arguments[0];
                valueType = arguments[1];
                return true;
            }

            valueType = null;
            keyType   = null;
            return false;
        }
        public bool IsDictionary( out ReadOnlySpan<Type> arguments )
        {
            if ( type.IsGenericType && type.IsDictionary() )
            {
                arguments = type.GetGenericArguments();
                return true;
            }

            foreach ( Type interfaceType in type.GetInterfaces() )
            {
                if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>) )
                {
                    arguments = interfaceType.GetGenericArguments();
                    return true;
                }
            }

            arguments = null;
            return false;
        }
    }



    extension( Type self )
    {
        public bool IsDictionaryEntry() => self == typeof(DictionaryEntry);
        public bool IsKeyValuePair()    => self.IsGenericType && self.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
    }



    extension( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type self )
    {
        public bool IsKeyValuePair( [NotNullWhen(true)] out Type? keyType, [NotNullWhen(true)] out Type? valueType )
        {
            if ( self.IsKeyValuePair(out ReadOnlySpan<Type> arguments) )
            {
                keyType   = arguments[0];
                valueType = arguments[1];
                return true;
            }

            valueType = null;
            keyType   = null;
            return false;
        }
        public bool IsKeyValuePair( out ReadOnlySpan<Type> arguments )
        {
            if ( self.IsKeyValuePair() )
            {
                arguments = self.GetGenericArguments();
                return true;
            }

            foreach ( Type interfaceType in self.GetInterfaces() )
            {
                if ( interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>) )
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
