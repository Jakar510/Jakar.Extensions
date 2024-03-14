// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  3:13 PM

namespace Jakar.SqlBuilder;


public struct DeleteChainBuilder( in DeleteClauseBuilder delete, ref EasySqlBuilder builder )
{
    private readonly DeleteClauseBuilder _delete  = delete;
    private          EasySqlBuilder      _builder = builder;
}
