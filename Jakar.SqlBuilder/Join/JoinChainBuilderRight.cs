namespace Jakar.SqlBuilder;


public struct JoinChainBuilderRight( in JoinClauseBuilder join, ref EasySqlBuilder builder )
{
    private readonly JoinClauseBuilder __join    = join;
    private          EasySqlBuilder    __builder = builder;

    public JoinClauseBuilder Right<TValue>( string columnName )
    {
        __builder.Add( columnName.GetName<TValue>() );
        __builder.VerifyParentheses();
        return __join;
    }
    public JoinClauseBuilder Right<TValue>( TValue _, string columnName )
    {
        __builder.Add( columnName.GetName<TValue>() );
        __builder.VerifyParentheses();
        return __join;
    }
    public JoinClauseBuilder Right( string columnName )
    {
        __builder.Add( columnName );
        __builder.VerifyParentheses();
        return __join;
    }
}
