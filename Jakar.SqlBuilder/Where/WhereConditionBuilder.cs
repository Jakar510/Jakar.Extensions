// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:45 AM

namespace Jakar.SqlBuilder;


public struct WhereConditionBuilder<TNext>
{
    private readonly TNext                      _next;
    private          EasySqlBuilder             _builder;
    private readonly Dictionary<string, string> _cache = new();

    public WhereConditionBuilder( in TNext next, ref EasySqlBuilder builder )
    {
        _next    = next;
        _builder = builder;
    }


    public TNext Done()
    {
        foreach ( ( string? columnName, string value ) in _cache ) { _builder.Add(columnName, "=", value + ','); }

        _builder.VerifyParentheses().NewLine();
        return _next;
    }
    public WhereConditionBuilder<TNext> With<T>( string columnName, T obj )
    {
        _cache[columnName] = obj?.ToString() ?? KeyWords.NULL;
        return this;
    }
}
