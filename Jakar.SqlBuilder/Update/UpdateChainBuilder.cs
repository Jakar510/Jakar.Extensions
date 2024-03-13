// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:47 AM

namespace Jakar.SqlBuilder;


public struct UpdateChainBuilder( in UpdateClauseBuilder update, ref EasySqlBuilder builder )
{
    private readonly UpdateClauseBuilder _update  = update;
    private          EasySqlBuilder      _builder = builder;
}
