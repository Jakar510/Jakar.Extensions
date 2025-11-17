namespace Jakar.Extensions;


public static partial class Types
{
    extension( Type type )
    {
        public bool IsAnyBuiltInType() => type.IsBuiltInType() || type.IsBuiltInNullableType();
        public bool IsBuiltInNullableType() => type.IsOneOfType(typeof(bool?),
                                                                typeof(byte?),
                                                                typeof(sbyte?),
                                                                typeof(Guid?),
                                                                typeof(char?),
                                                                typeof(DateTime?),
                                                                typeof(int?),
                                                                typeof(uint?),
                                                                typeof(short?),
                                                                typeof(ushort?),
                                                                typeof(long?),
                                                                typeof(ulong?),
                                                                typeof(float?),
                                                                typeof(double?),
                                                                typeof(decimal?),
                                                                typeof(TimeSpan?));
        public bool IsBuiltInType() => type.IsOneOfType(typeof(bool),
                                                        typeof(byte),
                                                        typeof(sbyte),
                                                        typeof(Guid),
                                                        typeof(char),
                                                        typeof(string),
                                                        typeof(DateTime),
                                                        typeof(int),
                                                        typeof(uint),
                                                        typeof(short),
                                                        typeof(ushort),
                                                        typeof(long),
                                                        typeof(ulong),
                                                        typeof(float),
                                                        typeof(double),
                                                        typeof(decimal),
                                                        typeof(TimeSpan));
        public bool IsGenericType()                                  => type.IsGenericType(typeof(JsonNode));
        public bool IsGenericType( params ReadOnlySpan<Type> types ) => type.IsAnyBuiltInType() || type.IsGenericType || type.IsOneOfType(types);
        public bool IsNullableType()                                 => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}
