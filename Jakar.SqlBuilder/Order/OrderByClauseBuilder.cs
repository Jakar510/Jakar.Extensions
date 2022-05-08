namespace Jakar.SqlBuilder;


public struct OrderByClauseBuilder
{
    private EasySqlBuilder _builder;
    public OrderByClauseBuilder( ref EasySqlBuilder builder ) => _builder = builder;


    public EasySqlBuilder By( string separator, params string[] columnNames ) => _builder.Begin().Add(KeyWords.ORDER, KeyWords.BY).AddRange(separator, columnNames).End();

    public OrderByClauseChainBuilder Chain()
    {
        _builder.Add(KeyWords.ORDER, KeyWords.BY).Begin();
        return new OrderByClauseChainBuilder(this, ref _builder);
    }
    public OrderByClauseChainBuilder Chain( string columnName )
    {
        _builder.Add(KeyWords.ORDER, KeyWords.BY).Begin().Add(columnName);
        return new OrderByClauseChainBuilder(this, ref _builder);
    }


    public EasySqlBuilder Done()
    {
        _builder.VerifyParentheses().NewLine();
        return _builder;
    }
    public OrderByClauseBuilder Next()
    {
        _builder.VerifyParentheses().Add(KeyWords.ORDER, KeyWords.BY).Begin();
        return this;
    }


    public OrderByClauseBuilder Ascending()
    {
        _builder.Add(KeyWords.ASC).Add(',');
        return this;
    }
    public OrderByClauseBuilder Ascending( string columnName )
    {
        _builder.Add(columnName, KeyWords.ASC).Add(',');
        return this;
    }


    public OrderByClauseBuilder Descending()
    {
        _builder.Add(KeyWords.DESC).Add(',');
        return this;
    }
    public OrderByClauseBuilder Descending( string columnName )
    {
        _builder.Add(columnName, KeyWords.DESC).Add(',');
        return this;
    }


    public OrderByClauseBuilder By( string columnName )
    {
        _builder.Begin().Add(KeyWords.ORDER, KeyWords.BY).Space().Add(columnName);
        return this;
    }
}
