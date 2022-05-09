// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:45 AM

namespace Jakar.SqlBuilder;


public struct WhereConditionBuilder<TNext>
{
    private readonly TNext          _next;
    private          EasySqlBuilder _builder;
    private readonly List<string>   _cache = new();

    public WhereConditionBuilder( in TNext next, ref EasySqlBuilder builder )
    {
        _next    = next;
        _builder = builder;
    }


    public TNext Done()
    {
        _builder.AddRange(',', _cache);

        _builder.VerifyParentheses().NewLine();
        return _next;
    }

    public WhereConditionBuilder<TNext> With( string columnName, string? value )
    {
        _cache.Add($"{columnName}={value ?? KeyWords.NULL}");
        return this;
    }
    public WhereConditionBuilder<TNext> With<T>( string columnName, T value ) where T : struct
    {
        _cache.Add($"{columnName}={value}");
        return this;
    }
}
