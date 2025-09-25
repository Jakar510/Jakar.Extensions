namespace Jakar.Database;


public static class TableExtensions
{
    public static JsonObject? GetAdditionalData( this DbDataReader reader ) =>
        reader.GetFieldValue<object?>(nameof(IJsonModel.AdditionalData)) is string value
            ? Json.GetAdditionalData(value)
            : null;


    public static TValue? GetData<TValue>( this DbDataReader reader, string propertyName )
        where TValue : class, IJsonModel<TValue> => reader.GetData(propertyName, TValue.JsonTypeInfo);
    public static TValue? GetData<TValue>( this DbDataReader reader, string propertyName, JsonTypeInfo<TValue> info )
        where TValue : class, IJsonModel<TValue> => reader.GetFieldValue<object?>(propertyName) is string json
                                                        ? json.FromJson(info)
                                                        : null;


    public static TValue? GetValue<TValue>( this DbDataReader reader, string propertyName ) =>
        reader.GetFieldValue<object?>(propertyName) is TValue value
            ? value
            : default;


    public static string GetTableName( this Type type, bool convertToSnakeCase = true )
    {
        string name = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()?.Name ?? type.Name;
        if ( convertToSnakeCase ) { name = name.ToSnakeCase(CultureInfo.InvariantCulture); }

        return name;
    }
}
