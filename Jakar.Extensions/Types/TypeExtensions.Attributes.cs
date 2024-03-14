namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    public static bool                   GetJsonIsRequired( this PropertyInfo propInfo ) => GetJsonProperty( propInfo )?.Required is Required.Always or Required.AllowNull;
    public static JsonPropertyAttribute? GetJsonProperty( this   PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonPropertyAttribute>();
    public static string                 GetJsonKey( this        PropertyInfo propInfo ) => GetJsonProperty( propInfo )?.PropertyName ?? propInfo.Name;
}
