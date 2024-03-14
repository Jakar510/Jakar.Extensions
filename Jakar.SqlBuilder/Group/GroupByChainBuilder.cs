// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:31 AM

namespace Jakar.SqlBuilder;


public struct GroupByChainBuilder( in GroupByClauseBuilder group, ref EasySqlBuilder builder )
{
    private readonly GroupByClauseBuilder _group   = group;
    private          EasySqlBuilder       _builder = builder;


    public SortersBuilder<GroupByChainBuilder> SortBy() => new(this, ref _builder);
}
