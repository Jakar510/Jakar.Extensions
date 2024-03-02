namespace Jakar.SqlBuilder;


public struct GroupByClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    public EasySqlBuilder By( string separator, params string[] columnNames ) => _builder.Add( GROUP, BY, string.Join( separator, columnNames ) );

    public GroupByChainBuilder Chain()
    {
        _builder.Add( GROUP, BY );
        return new GroupByChainBuilder( this, ref _builder );
    }

    public GroupByChainBuilder Chain( string columnName )
    {
        _builder.Add( GROUP, BY, columnName );
        return new GroupByChainBuilder( this, ref _builder );
    }


    public EasySqlBuilder Done() => _builder.VerifyParentheses().NewLine();


    public GroupByClauseBuilder Next()
    {
        _builder.VerifyParentheses().NewLine();

        return this;
    }


    public GroupByClauseBuilder And( string columnName )
    {
        _builder.Add( columnName + ',' );
        return this;
    }
}
