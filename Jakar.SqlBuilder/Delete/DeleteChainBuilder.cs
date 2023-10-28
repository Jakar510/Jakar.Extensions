// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  3:13 PM

namespace Jakar.SqlBuilder;


public struct DeleteChainBuilder
{
    private readonly DeleteClauseBuilder _delete;
    private          EasySqlBuilder      _builder;

    public DeleteChainBuilder( in DeleteClauseBuilder delete, ref EasySqlBuilder builder )
    {
        _delete  = delete;
        _builder = builder;
    }
}
