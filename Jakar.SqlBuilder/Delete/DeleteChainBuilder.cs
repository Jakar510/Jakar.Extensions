// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  3:13 PM

namespace Jakar.SqlBuilder;


public struct DeleteChainBuilder( in DeleteClauseBuilder delete, ref EasySqlBuilder builder )
{
    private readonly DeleteClauseBuilder __delete  = delete;
    private          EasySqlBuilder      __builder = builder;
}
