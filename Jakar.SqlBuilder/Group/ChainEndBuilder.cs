// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:31 AM

namespace Jakar.SqlBuilder;


public struct GroupByChainEndBuilder( in GroupByClauseBuilder group, ref EasySqlBuilder builder )
{
    private readonly GroupByClauseBuilder __group   = group;
    private          EasySqlBuilder       __builder = builder;


    public EasySqlBuilder Done() => __builder.VerifyParentheses();
}
