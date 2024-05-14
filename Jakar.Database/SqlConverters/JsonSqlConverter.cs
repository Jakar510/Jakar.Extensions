namespace Jakar.Database;


public class JsonSqlHandler<TConverter, T> : SqlConverter<JsonSqlHandler<T>, T>
    where TConverter : JsonSqlHandler<TConverter, T>, new()
{
    public JsonSqlHandler() { }


    public override T Parse( object? value )
    {
        string? item = value?.ToString();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        return item is null
                   ? default!
                   : item.FromJson<T>();
    }

    public override void SetValue( IDbDataParameter parameter, T? value )
    {
        parameter.Value  = value?.ToPrettyJson();
        parameter.DbType = DbType.String;
    }
}



public sealed class JsonSqlHandler<T> : JsonSqlHandler<JsonSqlHandler<T>, T>
{
    public JsonSqlHandler() { }


    public override T Parse( object? value )
    {
        string? s = value?.ToString();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        return s is null
                   ? default!
                   : s.FromJson<T>();
    }

    public override void SetValue( IDbDataParameter parameter, T? value )
    {
        parameter.Value  = value?.ToPrettyJson();
        parameter.DbType = DbType.String;
    }
}
