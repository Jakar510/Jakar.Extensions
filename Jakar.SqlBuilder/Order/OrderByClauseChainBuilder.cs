// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  10:21 AM

namespace Jakar.SqlBuilder;


public struct OrderByClauseChainBuilder
{
    private readonly OrderByClauseBuilder _order;
    private          EasySqlBuilder       _builder;
    public OrderByClauseChainBuilder( in OrderByClauseBuilder order, ref EasySqlBuilder builder )
    {
        _order   = order;
        _builder = builder;
    }
}
