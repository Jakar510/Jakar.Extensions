namespace Jakar.SqlBuilder;


public struct JoinChainBuilderRight
{
    private readonly JoinClauseBuilder _join;
    private          EasySqlBuilder    _builder;
    public JoinChainBuilderRight( in JoinClauseBuilder join, ref EasySqlBuilder builder )
    {
        _join    = join;
        _builder = builder;
    }

    public JoinClauseBuilder Right<T>( string columnName )
    {
        _builder.Add(columnName.GetName<T>());
        _builder.VerifyParentheses();
        return _join;
    }
    public JoinClauseBuilder Right<T>( T _, string columnName )
    {
        _builder.Add(columnName.GetName<T>());
        _builder.VerifyParentheses();
        return _join;
    }
    public JoinClauseBuilder Right( string columnName )
    {
        _builder.Add(columnName);
        _builder.VerifyParentheses();
        return _join;
    }
}