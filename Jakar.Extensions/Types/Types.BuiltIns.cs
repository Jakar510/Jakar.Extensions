namespace Jakar.Extensions;


public static partial class Types
{
    public static bool IsAnyBuiltInType( this Type type ) => type.IsBuiltInType() || type.IsBuiltInNullableType();

    public static bool IsBuiltInNullableType( this Type type ) => type.IsOneOfType( [
                                                                                        typeof(bool?),
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
                                                                                        typeof(TimeSpan?)
                                                                                    ] );

    public static bool IsBuiltInType( this Type type ) => type.IsOneOfType( [
                                                                                typeof(bool),
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
                                                                                typeof(TimeSpan)
                                                                            ] );
    public static bool IsGenericType( this  Type propertyType )                                  => propertyType.IsGenericType( [typeof(JObject), typeof(JToken)] );
    public static bool IsGenericType( this  Type propertyType, params ReadOnlySpan<Type> types ) => propertyType.IsAnyBuiltInType() || propertyType.IsGenericType || propertyType.IsOneOfType( types );
    public static bool IsNullableType( this Type type ) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
}
