// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  10:21 AM

namespace Jakar.SqlBuilder;


public struct OrderByClauseChainBuilder( in OrderByClauseBuilder order, ref EasySqlBuilder builder )
{
    private readonly OrderByClauseBuilder __order   = order;
    private          EasySqlBuilder       __builder = builder;
}
