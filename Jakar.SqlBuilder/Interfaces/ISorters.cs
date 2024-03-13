namespace Jakar.SqlBuilder.Interfaces;


public interface ISorters<out TNext>
{
    /// <summary> Ends with a ASC and returns to <typeparamref name="TNext"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Ascending();

    /// <summary> continues previous clause and adds <paramref name="columnName"/> followed by ASC </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Ascending( string columnName );

    /// <summary> Ends with a DESC and returns to <typeparamref name="TNext"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Descending();

    /// <summary> continues previous clause and adds <paramref name="columnName"/> followed by DESC </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Descending( string columnName );
}
