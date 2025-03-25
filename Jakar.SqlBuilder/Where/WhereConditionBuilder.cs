// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:45 AM

namespace Jakar.SqlBuilder;


public struct WhereConditionBuilder<TNext>( in TNext next, ref EasySqlBuilder builder )
{
    private readonly TNext          _next    = next;
    private          EasySqlBuilder _builder = builder;
    private readonly List<string>   _cache   = new();


    public TNext Done()
    {
        _builder.AddRange( ',', _cache );

        _builder.VerifyParentheses().NewLine();

        return _next;
    }

    public WhereConditionBuilder<TNext> With( string columnName, string? value )
    {
        _cache.Add( $"{columnName}={value ?? NULL}" );
        return this;
    }
    public WhereConditionBuilder<TNext> With<TValue>( string columnName, TValue value )
        where TValue : struct
    {
        _cache.Add( $"{columnName}={value}" );
        return this;
    }
}
