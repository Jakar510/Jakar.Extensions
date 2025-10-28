namespace Jakar.SqlBuilder;


public struct OrderByClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder __builder = builder;


    /// <summary> Simple ORDER BY <paramref name="columnNames"/> delimited by <paramref name="separator"/> </summary>
    /// <returns>
    ///     <see cref="EasySqlBuilder"/>
    /// </returns>
    public EasySqlBuilder By( string separator, params string[] columnNames ) => __builder.Begin()
                                                                                          .Add(ORDER, BY)
                                                                                          .AddRange(separator, columnNames)
                                                                                          .End();


    /// <summary> Starts an ORDER BY chain </summary>
    /// <returns>
    ///     <see cref="OrderByClauseChainBuilder"/>
    /// </returns>
    public OrderByClauseChainBuilder Chain()
    {
        __builder.Add(ORDER, BY)
                 .Begin();

        return new OrderByClauseChainBuilder(this, ref __builder);
    }
    /// <summary> Starts an ORDER BY chain starting with <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <see cref="OrderByClauseChainBuilder"/>
    /// </returns>
    public OrderByClauseChainBuilder Chain( string columnName )
    {
        __builder.Add(ORDER, BY)
                 .Begin()
                 .Add(columnName);

        return new OrderByClauseChainBuilder(this, ref __builder);
    }


    public EasySqlBuilder Done()
    {
        __builder.VerifyParentheses()
                 .NewLine();

        return __builder;
    }
    public OrderByClauseBuilder Next()
    {
        __builder.VerifyParentheses()
                 .Add(ORDER, BY)
                 .Begin();

        return this;
    }


    public SortersBuilder<OrderByClauseBuilder> SortBy() => new(this, ref __builder);


    /// <summary> continues previous clause and adds <paramref name="columnName"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country, CustomerName; </example>
    /// <returns>
    ///     <see cref="OrderByClauseBuilder"/>
    /// </returns>
    public OrderByClauseBuilder By( string columnName )
    {
        __builder.Begin()
                 .Add(ORDER, BY)
                 .Space()
                 .Add(columnName);

        return this;
    }
}
