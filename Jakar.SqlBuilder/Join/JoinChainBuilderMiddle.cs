namespace Jakar.SqlBuilder;


public struct JoinChainBuilderMiddle( in JoinClauseBuilder join, ref EasySqlBuilder builder )
{
    private readonly JoinClauseBuilder _join = join;
    private          EasySqlBuilder    _builder = builder;


    public JoinChainBuilderRight Greater()
    {
        _builder.Add( LESS_THAN );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight LessThan()
    {
        _builder.Add( GREATER );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight GreaterOrEqual()
    {
        _builder.Add( GREATER_OR_EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight LessThanOrEqual()
    {
        _builder.Add( LESS_THAN_OR_EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight Equal()
    {
        _builder.Add( EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight NotEqual()
    {
        _builder.Add( NOT_EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight AboveOrBelow()
    {
        _builder.Add( ABOVE_OR_BELOW );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
}
