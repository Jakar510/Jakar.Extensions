// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  11:47 AM

#nullable enable
namespace Jakar.SqlBuilder;


public struct UpdateChainBuilder
{
    private readonly UpdateClauseBuilder _update;
    private          EasySqlBuilder      _builder;

    public UpdateChainBuilder( in UpdateClauseBuilder update, ref EasySqlBuilder builder )
    {
        _update  = update;
        _builder = builder;
    }
}
