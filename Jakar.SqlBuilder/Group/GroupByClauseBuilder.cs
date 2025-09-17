namespace Jakar.SqlBuilder;


public struct GroupByClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder __builder = builder;


    public EasySqlBuilder By( string separator, params string[] columnNames ) => __builder.Add( GROUP, BY, string.Join( separator, columnNames ) );

    public GroupByChainBuilder Chain()
    {
        __builder.Add( GROUP, BY );
        return new GroupByChainBuilder( this, ref __builder );
    }

    public GroupByChainBuilder Chain( string columnName )
    {
        __builder.Add( GROUP, BY, columnName );
        return new GroupByChainBuilder( this, ref __builder );
    }


    public EasySqlBuilder Done() => __builder.VerifyParentheses().NewLine();


    public GroupByClauseBuilder Next()
    {
        __builder.VerifyParentheses().NewLine();

        return this;
    }


    public GroupByClauseBuilder And( string columnName )
    {
        __builder.Add( columnName + ',' );
        return this;
    }
}
