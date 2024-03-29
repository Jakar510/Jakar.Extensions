﻿namespace Jakar.SqlBuilder;


public struct SortersBuilder<TNext>( in TNext next, ref EasySqlBuilder builder )
{
    private readonly TNext          _next    = next;
    private          EasySqlBuilder _builder = builder;


    /// <summary> Ends with a ASC and returns to <typeparamref name="TNext"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Ascending()
    {
        _builder.Add( ASC );
        return _next;
    }

    /// <summary> continues previous clause and adds <paramref name="columnName"/> followed by ASC </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Ascending( string columnName )
    {
        _builder.Add( columnName, ASC );
        return _next;
    }


    /// <summary> Ends with a DESC and returns to <typeparamref name="TNext"/> </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Descending()
    {
        _builder.Add( DESC );
        return _next;
    }

    /// <summary> continues previous clause and adds <paramref name="columnName"/> followed by DESC </summary>
    /// <example> SELECT * FROM Customers ORDER BY Country ASC, CustomerName DESC; </example>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Descending( string columnName )
    {
        _builder.Add( columnName, DESC );
        return _next;
    }
}
