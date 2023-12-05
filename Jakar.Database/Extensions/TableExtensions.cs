namespace Jakar.Database;


public static class TableExtensions
{
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static Dictionary<string, JsonElement>? GetAdditionalData( this DbDataReader reader ) => reader.GetData<Dictionary<string, JsonElement>>();


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static T? GetData<T>( this DbDataReader reader )
        where T : class => reader.GetFieldValue<object?>( nameof(MsJsonModels.IJsonModel.AdditionalData) ) is string json
                               ? JsonSerializer_.Deserialize<T>( json )
                               : default;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static T? GetValue<T>( this DbDataReader reader ) => reader.GetFieldValue<object?>( nameof(JsonModels.IJsonModel.AdditionalData) ) is T value
                                                                    ? value
                                                                    : default;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static string GetTableName( this Type type )
    {
        string name = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()?.Name ?? type.Name;

        // name = name.ToSnakeCase(CultureInfo.InvariantCulture)
        return name;
    }
}
