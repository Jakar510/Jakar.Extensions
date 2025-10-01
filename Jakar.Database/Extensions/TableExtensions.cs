namespace Jakar.Database;


public static class TableExtensions
{
    [Pure] public static TClass Validate<TClass>( this TClass self )
        where TClass : ITableRecord<TClass>
    {
        if ( !Debugger.IsAttached ) { return self; }

        if ( !string.Equals(TClass.TableName, TClass.TableName.ToSnakeCase()) ) { throw new InvalidOperationException($"{typeof(TClass).Name}: {nameof(TClass.TableName)} is not snake_case: '{TClass.TableName}'"); }

        DynamicParameters parameters     = self.ToDynamicParameters();
        string[]          parameterNames = parameters.ParameterNames.ToArray();
        int               length         = parameterNames.Length;


        if ( length == TClass.ClassProperties.Length ) { return self; }


        HashSet<string> missing =
        [
            .. TClass.ClassProperties.AsValueEnumerable()
                     .Select(static x => x.Name)
        ];

        missing.ExceptWith(parameterNames);

        string message = $"""
                          {typeof(TClass).Name}: {nameof(self.ToDynamicParameters)}.Length ({length}) != {nameof(TClass.ClassProperties)}.Length ({TClass.ClassProperties.Length})
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
