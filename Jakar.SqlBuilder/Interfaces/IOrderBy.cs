namespace Jakar.SqlBuilder.Interfaces;


public interface IOrderBy
{
    /// <summary> Starts an ORDER BY chain </summary>
    /// <returns> <see cref="IOrderByChain"/> </returns>
    public IOrderByChain Chain();

    /// <summary> Starts an ORDER BY chain starting with <paramref name="columnName"/> </summary>
    /// <returns> <see cref="IOrderByChain"/> </returns>
    public IOrderByChain Chain( string columnName );
    /// <summary> Simple ORDER BY <paramref name="columnNames"/> delimited by <paramref name="separator"/> </summary>
    /// <returns> <see cref="ISqlBuilderRoot"/> </returns>
    public ISqlBuilderRoot By( string separator, params string[] columnNames );
}



public interface IOrderByChain : IChainEnd<ISqlBuilderRoot>, INextChain<IOrderBy>, ISorters<IOrderByChain>, IAggregateFunctions<IOrderByChain>
{
    /// <summary> continues previous clause and adds <paramref name="columnName"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country, CustomerName; </example>
    /// <returns> <see cref="IOrderByChain"/> </returns>
    public IOrderByChain By( string columnName );
}
