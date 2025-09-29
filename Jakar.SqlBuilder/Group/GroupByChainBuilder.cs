// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:31 AM

namespace Jakar.SqlBuilder;


public struct GroupByChainBuilder( in GroupByClauseBuilder group, ref EasySqlBuilder builder )
{
    private readonly GroupByClauseBuilder __group   = group;
    private          EasySqlBuilder       __builder = builder;


    public SortersBuilder<GroupByChainBuilder> SortBy() => new(this, ref __builder);
}
