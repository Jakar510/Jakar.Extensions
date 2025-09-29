// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:45 AM

namespace Jakar.SqlBuilder;


public struct WhereConditionBuilder<TNext>( in TNext next, ref EasySqlBuilder builder )
{
    private readonly TNext          __next    = next;
    private          EasySqlBuilder __builder = builder;
    private readonly List<string>   __cache   = new();


    public TNext Done()
    {
        __builder.AddRange( ',', __cache );

        __builder.VerifyParentheses().NewLine();

        return __next;
    }

    public WhereConditionBuilder<TNext> With( string columnName, string? value )
    {
        __cache.Add( $"{columnName}={value ?? NULL}" );
        return this;
    }
    public WhereConditionBuilder<TNext> With<TValue>( string columnName, TValue value )
        where TValue : struct
    {
        __cache.Add( $"{columnName}={value}" );
        return this;
    }
}
