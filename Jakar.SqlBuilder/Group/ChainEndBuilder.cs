// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:31 AM

namespace Jakar.SqlBuilder;


public struct GroupByChainEndBuilder
{
    private readonly GroupByClauseBuilder _group;
    private          EasySqlBuilder       _builder;


    public GroupByChainEndBuilder( in GroupByClauseBuilder group, ref EasySqlBuilder builder )
    {
        _group   = group;
        _builder = builder;
    }

    public EasySqlBuilder Done() => _builder.VerifyParentheses();
}
