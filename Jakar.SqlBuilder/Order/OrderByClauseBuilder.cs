namespace Jakar.SqlBuilder;


public struct OrderByClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    /// <summary> Simple ORDER BY <paramref name="columnNames"/> delimited by <paramref name="separator"/> </summary>
    /// <returns>
    ///     <see cref="EasySqlBuilder"/>
    /// </returns>
    public EasySqlBuilder By( string separator, params string[] columnNames ) => _builder.Begin().Add( ORDER, BY ).AddRange( separator, columnNames ).End();


    /// <summary> Starts an ORDER BY chain </summary>
    /// <returns>
    ///     <see cref="OrderByClauseChainBuilder"/>
    /// </returns>
    public OrderByClauseChainBuilder Chain()
    {
        _builder.Add( ORDER, BY ).Begin();

        return new OrderByClauseChainBuilder( this, ref _builder );
    }
    /// <summary> Starts an ORDER BY chain starting with <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <see cref="OrderByClauseChainBuilder"/>
    /// </returns>
    public OrderByClauseChainBuilder Chain( string columnName )
    {
        _builder.Add( ORDER, BY ).Begin().Add( columnName );

        return new OrderByClauseChainBuilder( this, ref _builder );
    }


    public EasySqlBuilder Done()
    {
        _builder.VerifyParentheses().NewLine();

        return _builder;
    }
    public OrderByClauseBuilder Next()
    {
        _builder.VerifyParentheses().Add( ORDER, BY ).Begin();

        return this;
    }


    public SortersBuilder<OrderByClauseBuilder> SortBy() => new(this, ref _builder);


    /// <summary> continues previous clause and adds <paramref name="columnName"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country, CustomerName; </example>
    /// <returns>
    ///     <see cref="OrderByClauseBuilder"/>
    /// </returns>
    public OrderByClauseBuilder By( string columnName )
    {
        _builder.Begin().Add( ORDER, BY ).Space().Add( columnName );

        return this;
    }
}
