#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


public interface IGroupBy
{
    /// <summary>
    ///     Simple ORDER BY
    ///     <paramref name = "columnNames" />
    /// </summary>
    /// <returns>
    ///     <see cref = "ISqlBuilderRoot" />
    /// </returns>
    public ISqlBuilderRoot By( string separator, params string[] columnNames );

    /// <summary>
    ///     Starts an ORDER BY chain
    /// </summary>
    /// <returns>
    ///     <see cref = "IGroupByChain" />
    /// </returns>
    public IGroupByChain Chain();

    /// <summary>
    ///     Starts an ORDER BY chain starting with
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <see cref = "IGroupByChain" />
    /// </returns>
    public IGroupByChain Chain( string columnName );
}



public interface IGroupByChain : IChainEnd<ISqlBuilderRoot>, INextChain<IGroupBy>, ISorters<IGroupByChain>
{
    /// <summary>
    ///     continues previous clause and adds
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <example>
    ///     SELECT * FROM Customers
    ///     ORDER BY Country, CustomerName;
    /// </example>
    /// <returns>
    ///     <see cref = "IGroupByChain" />
    /// </returns>
    public IGroupByChain And( string columnName );
}
