namespace Jakar.SqlBuilder;


public abstract class BaseClauseBuilder
{
    protected readonly EasySqlBuilder _builder;

    protected BaseClauseBuilder( EasySqlBuilder builder ) => _builder = builder;
}