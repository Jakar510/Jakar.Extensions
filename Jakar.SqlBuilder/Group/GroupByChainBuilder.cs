// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:31 AM

#nullable enable
namespace Jakar.SqlBuilder;


public struct GroupByChainBuilder
{
    private readonly GroupByClauseBuilder _group;
    private          EasySqlBuilder       _builder;


    public GroupByChainBuilder( in GroupByClauseBuilder group, ref EasySqlBuilder builder )
    {
        _group   = group;
        _builder = builder;
    }

    public SortersBuilder<GroupByChainBuilder> SortBy() => new(this, ref _builder);
}
