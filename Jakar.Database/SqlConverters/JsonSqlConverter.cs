namespace Jakar.Database;


public abstract class JsonSqlHandler<TClass, TValue> : SqlConverter<TClass, TValue>
    where TClass : JsonSqlHandler<TClass, TValue>, new()
{
    public abstract JsonTypeInfo<TValue> TypeInfo { get; }
    public override TValue Parse( object? value )
    {
        string? item = value?.ToString();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        return item is null
                   ? default!
                   : item.FromJson(TypeInfo);
    }
    public override void SetValue( IDbDataParameter parameter, TValue? value )
    {
        parameter.Value = value is null
                              ? null
                              : value.ToJson(TypeInfo);

        parameter.DbType = DbType.String;
    }
}



public sealed class JsonSqlHandler<TValue> : JsonSqlHandler<JsonSqlHandler<TValue>, TValue>
    where TValue : IJsonModel<TValue>
{
    public override JsonTypeInfo<TValue> TypeInfo => TValue.JsonTypeInfo;
    public override TValue Parse( object? value )
    {
        string? item = value?.ToString();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        return item is null
                   ? default!
                   : item.FromJson(TypeInfo);
    }
    public override void SetValue( IDbDataParameter parameter, TValue? value )
    {
        parameter.Value = value is null
                              ? null
                              : value.ToJson(TypeInfo);

        parameter.DbType = DbType.String;
    }
}
