// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:31 AM

namespace Jakar.SqlBuilder;


public struct GroupByChainEndBuilder( in GroupByClauseBuilder group, ref EasySqlBuilder builder )
{
    private readonly GroupByClauseBuilder _group   = group;
    private          EasySqlBuilder       _builder = builder;


    public EasySqlBuilder Done() => _builder.VerifyParentheses();
}
