namespace Jakar.Extensions.Types;


public static partial class TypeExtensions
{
    public static JsonPropertyAttribute? GetJsonProperty( this   PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonPropertyAttribute>();
    public static string                 GetJsonKey( this        PropertyInfo propInfo ) => GetJsonProperty(propInfo)?.PropertyName ?? propInfo.Name;
    public static bool                   GetJsonIsRequired( this PropertyInfo propInfo ) => GetJsonProperty(propInfo)?.Required is Required.Always or Required.AllowNull;
}
