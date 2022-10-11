namespace Jakar.Database;


public class JsonSqlHandler<T> : SqlConverter<JsonSqlHandler<T>, T> where T : notnull
{
    public JsonSqlHandler() { }


    public override T Parse( object? value )
    {
        string? item = value?.ToString();
        if (item is null) { return default!; }

        return item.FromJson<T>();
    }

    public override void SetValue( IDbDataParameter parameter, T? value )
    {
        parameter.Value  = value?.ToPrettyJson();
        parameter.DbType = DbType.String;
    }
}
