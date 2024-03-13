namespace Jakar.SqlBuilder;


public struct JoinChainBuilderRight( in JoinClauseBuilder join, ref EasySqlBuilder builder )
{
    private readonly JoinClauseBuilder _join    = join;
    private          EasySqlBuilder    _builder = builder;

    public JoinClauseBuilder Right<T>( string columnName )
    {
        _builder.Add( columnName.GetName<T>() );
        _builder.VerifyParentheses();
        return _join;
    }
    public JoinClauseBuilder Right<T>( T _, string columnName )
    {
        _builder.Add( columnName.GetName<T>() );
        _builder.VerifyParentheses();
        return _join;
    }
    public JoinClauseBuilder Right( string columnName )
    {
        _builder.Add( columnName );
        _builder.VerifyParentheses();
        return _join;
    }
}
