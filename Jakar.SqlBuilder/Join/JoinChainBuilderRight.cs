namespace Jakar.SqlBuilder;


public struct JoinChainBuilderRight( in JoinClauseBuilder join, ref EasySqlBuilder builder )
{
    private readonly JoinClauseBuilder _join    = join;
    private          EasySqlBuilder    _builder = builder;

    public JoinClauseBuilder Right<TValue>( string columnName )
    {
        _builder.Add( columnName.GetName<TValue>() );
        _builder.VerifyParentheses();
        return _join;
    }
    public JoinClauseBuilder Right<TValue>( TValue _, string columnName )
    {
        _builder.Add( columnName.GetName<TValue>() );
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
