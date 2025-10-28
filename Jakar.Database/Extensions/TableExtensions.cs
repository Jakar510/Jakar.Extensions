namespace Jakar.Database;


public static class TableExtensions
{
    [Pure] public static TSelf Validate<TSelf>( this TSelf self )
        where TSelf : ITableRecord<TSelf>
    {
        if ( !Debugger.IsAttached ) { return self; }

        if ( !string.Equals(TSelf.TableName, TSelf.TableName.ToSnakeCase()) ) { throw new InvalidOperationException($"{typeof(TSelf).Name}: {nameof(TSelf.TableName)} is not snake_case: '{TSelf.TableName}'"); }

        PostgresParameters parameters     = self.ToDynamicParameters();
        string[]           parameterNames = parameters.ParameterNames.ToArray();
        int                length         = parameterNames.Length;


        if ( length == TSelf.ClassProperties.Length ) { return self; }


        HashSet<string> missing =
        [
            .. TSelf.ClassProperties.AsValueEnumerable()
                    .Select(static x => x.Name)
        ];

        missing.ExceptWith(parameterNames);

        string message = $"""
                          {typeof(TSelf).Name}: {nameof(self.ToDynamicParameters)}.Length ({length}) != {nameof(TSelf.ClassProperties)}.Length ({TSelf.ClassProperties.Length})
                          {missing.ToJson(JakarDatabaseContext.Default.HashSetString)}
                          """;

        throw new InvalidOperationException(message);
    }


    public static JsonObject? GetAdditionalData( this DbDataReader reader ) =>
        reader.GetFieldValue<object?>(nameof(IJsonModel.AdditionalData)) is string value
            ? value.GetAdditionalData()
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
        string name = type.GetCustomAttribute<TableAttribute>()
                         ?.Name ??
                      type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()
                         ?.Name ??
                      type.Name;

        if ( convertToSnakeCase ) { name = name.ToSnakeCase(CultureInfo.InvariantCulture); }

        return name;
    }
}
