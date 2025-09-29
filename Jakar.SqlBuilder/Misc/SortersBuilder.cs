namespace Jakar.SqlBuilder;


public struct SortersBuilder<TNext>( in TNext next, ref EasySqlBuilder builder )
{
    private readonly TNext          __next    = next;
    private          EasySqlBuilder __builder = builder;


    /// <summary> Ends with a ASC and returns to <typeparamref name="TNext"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Ascending()
    {
        __builder.Add( ASC );
        return __next;
    }

    /// <summary> continues previous clause and adds <paramref name="columnName"/> followed by ASC </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Ascending( string columnName )
    {
        __builder.Add( columnName, ASC );
        return __next;
    }


    /// <summary> Ends with a DESC and returns to <typeparamref name="TNext"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Descending()
    {
        __builder.Add( DESC );
        return __next;
    }

    /// <summary> continues previous clause and adds <paramref name="columnName"/> followed by DESC </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Descending( string columnName )
    {
        __builder.Add( columnName, DESC );
        return __next;
    }
}
