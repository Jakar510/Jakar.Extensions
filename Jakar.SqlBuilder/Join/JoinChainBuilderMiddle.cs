#nullable enable
namespace Jakar.SqlBuilder;


public struct JoinChainBuilderMiddle
{
    private readonly JoinClauseBuilder _join;
    private          EasySqlBuilder    _builder;
    public JoinChainBuilderMiddle( in JoinClauseBuilder join, ref EasySqlBuilder builder )
    {
        _join    = join;
        _builder = builder;
    }


    public JoinChainBuilderRight Greater()
    {
        _builder.Add( KeyWords.LESS_THAN );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight LessThan()
    {
        _builder.Add( KeyWords.GREATER );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight GreaterOrEqual()
    {
        _builder.Add( KeyWords.GREATER_OR_EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight LessThanOrEqual()
    {
        _builder.Add( KeyWords.LESS_THAN_OR_EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight Equal()
    {
        _builder.Add( KeyWords.EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight NotEqual()
    {
        _builder.Add( KeyWords.NOT_EQUAL );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
    public JoinChainBuilderRight AboveOrBelow()
    {
        _builder.Add( KeyWords.ABOVE_OR_BELOW );
        return new JoinChainBuilderRight( _join, ref _builder );
    }
}
