namespace Jakar.Database;


public static class TableExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static IDictionary<string, JToken?>? GetAdditionalData( this DbDataReader reader ) => reader.GetFieldValue<object?>( nameof(JsonModels.IJsonModel.AdditionalData) ) is string value
                                                                                                     ? value.FromJson<Dictionary<string, JToken?>>()
                                                                                                     : null;

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TValue? GetData<TValue>( this DbDataReader reader, string propertyName )
        where TValue : class => reader.GetFieldValue<object?>( propertyName ) is string json
                               ? json.FromJson<TValue>()
                               : null;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static TValue? GetValue<TValue>( this DbDataReader reader, string propertyName ) => reader.GetFieldValue<object?>( propertyName ) is TValue value
                                                                                         ? value
                                                                                         : default;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetTableName( this Type type )
    {
        string name = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()?.Name ?? type.Name;

        // name = name.ToSnakeCase(CultureInfo.InvariantCulture)
        return name;
    }
}
