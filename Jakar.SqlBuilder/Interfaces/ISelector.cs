using Jakar.Extensions;



namespace Jakar.SqlBuilder.Interfaces;


public interface ISelector : IFromSyntax<ISqlBuilderRoot>, IAggregateFunctions<ISelector>
{
    /// <summary> Adds
    ///     <param name="columnName"> </param>
    ///     to SELECT set </summary>
    /// <returns> <see cref="ISelector"/> </returns>
    public ISelector Next( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName(System.Type)"/> </para>
    ///     Adds table_name.
    ///     <param name="columnName"> </param>
    ///     to SELECT set
    /// </summary>
    /// <example> SELECT Orders.OrderID, Customers.CustomerName, Orders.OrderDate </example>
    /// <returns> <see cref="ISelector"/> </returns>
    public ISelector Next<T>( string columnName ) where T : class;


    /// <summary> Adds
    ///     <param name="columnNames"> </param>
    ///     separated by
    ///     <param name="separator"> </param>
    ///     to SELECT set and setting it to the
    ///     <param name="alias"> </param>
    ///     variable </summary>
    /// <returns> <see cref="ISelector"/> </returns>
    public ISelector Next( string alias, string separator, params string[] columnNames );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName(Type)"/> </para>
    ///     Adds table_name.columnName separated by
    ///     <param name="separator"> </param>
    ///     to SELECT set and setting it to the
    ///     <param name="alias"> </param>
    ///     variable
    /// </summary>
    /// <returns> <see cref="ISelector"/> </returns>
    public ISelector Next<T>( string alias, string separator, params string[] columnNames ) where T : class;
}
