namespace Jakar.SqlBuilder;


public struct JoinClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder __builder = builder;


    public EasySqlBuilder Done() => __builder.VerifyParentheses().NewLine();


    public JoinClauseBuilder Left( string columnName )
    {
        __builder.Add( LEFT, JOIN, columnName );
        return this;
    }
    public JoinClauseBuilder Inner( string columnName )
    {
        __builder.Add( INNER, JOIN, columnName );
        return this;
    }
    public JoinClauseBuilder Right( string columnName )
    {
        __builder.Add( RIGHT, JOIN, columnName );
        return this;
    }
    public JoinClauseBuilder Full( string columnName )
    {
        __builder.Add( FULL, JOIN, columnName );
        return this;
    }


    public JoinChainBuilderLeft On()
    {
        __builder.Add( ON ).Begin();

        return new JoinChainBuilderLeft( this, ref __builder );
    }
}
