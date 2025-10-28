namespace Jakar.SqlBuilder;


public struct JoinChainBuilderMiddle( in JoinClauseBuilder join, ref EasySqlBuilder builder )
{
    private readonly JoinClauseBuilder __join    = join;
    private          EasySqlBuilder    __builder = builder;


    public JoinChainBuilderRight Greater()
    {
        __builder.Add(LESS_THAN);
        return new JoinChainBuilderRight(__join, ref __builder);
    }
    public JoinChainBuilderRight LessThan()
    {
        __builder.Add(GREATER);
        return new JoinChainBuilderRight(__join, ref __builder);
    }
    public JoinChainBuilderRight GreaterOrEqual()
    {
        __builder.Add(GREATER_OR_EQUAL);
        return new JoinChainBuilderRight(__join, ref __builder);
    }
    public JoinChainBuilderRight LessThanOrEqual()
    {
        __builder.Add(LESS_THAN_OR_EQUAL);
        return new JoinChainBuilderRight(__join, ref __builder);
    }
    public JoinChainBuilderRight Equal()
    {
        __builder.Add(EQUAL);
        return new JoinChainBuilderRight(__join, ref __builder);
    }
    public JoinChainBuilderRight NotEqual()
    {
        __builder.Add(NOT_EQUAL);
        return new JoinChainBuilderRight(__join, ref __builder);
    }
    public JoinChainBuilderRight AboveOrBelow()
    {
        __builder.Add(ABOVE_OR_BELOW);
        return new JoinChainBuilderRight(__join, ref __builder);
    }
}
