namespace Jakar.Database;


public class JsonSqlHandler<TConverter, TValue> : SqlConverter<JsonSqlHandler<TValue>, TValue>
    where TConverter : JsonSqlHandler<TConverter, TValue>, new()
{
    public JsonSqlHandler() { }


    public override TValue Parse( object? value )
    {
        string? item = value?.ToString();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        return item is null
                   ? default!
                   : item.FromJson<TValue>();
    }

    public override void SetValue( IDbDataParameter parameter, TValue? value )
    {
        parameter.Value  = value?.ToPrettyJson();
        parameter.DbType = DbType.String;
    }
}



public sealed class JsonSqlHandler<TValue> : JsonSqlHandler<JsonSqlHandler<TValue>, TValue>
{
    public JsonSqlHandler() { }


    public override TValue Parse( object? value )
    {
        string? s = value?.ToString();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        return s is null
                   ? default!
                   : s.FromJson<TValue>();
    }

    public override void SetValue( IDbDataParameter parameter, TValue? value )
    {
        parameter.Value  = value?.ToPrettyJson();
        parameter.DbType = DbType.String;
    }
}
