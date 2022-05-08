namespace Jakar.SqlBuilder;


public struct SortersBuilder<TNext>
{
    private readonly TNext          _next;
    private          EasySqlBuilder _builder;
    public SortersBuilder( in TNext next, ref EasySqlBuilder builder )
    {
        _next    = next;
        _builder = builder;
    }


    /// <summary>
    /// Ends with a ASC and returns to <typeparamref name="TNext"/>
    /// </summary>
    /// <example>
    /// SELECT * FROM Customers
    /// ORDER BY Country ASC, CustomerName DESC;
    /// </example>
    /// <returns><typeparamref name="TNext"/></returns>
    public TNext Ascending()
    {
        _builder.Add(KeyWords.ASC);
        return _next;
    }

    /// <summary>
    /// continues previous clause and adds <paramref name="columnName"/> followed by ASC
    /// </summary>
    /// <example>
    /// SELECT * FROM Customers
    /// ORDER BY Country ASC, CustomerName DESC;
    /// </example>
    /// <returns><typeparamref name="TNext"/></returns>
    public TNext Ascending( string columnName )
    {
        _builder.Add(columnName, KeyWords.ASC);
        return _next;
    }

    /// <summary>
    /// Ends with a DESC and returns to <typeparamref name="TNext"/>
    /// </summary>
    /// <example>
    /// SELECT * FROM Customers
    /// ORDER BY Country ASC, CustomerName DESC;
    /// </example>
    /// <returns><typeparamref name="TNext"/></returns>
    public TNext Descending()
    {
        _builder.Add(KeyWords.DESC);
        return _next;
    }

    /// <summary>
    /// continues previous clause and adds <paramref name="columnName"/> followed by DESC
    /// </summary>
    /// <example>
    /// SELECT * FROM Customers
    /// ORDER BY Country ASC, CustomerName DESC;
    /// </example>
    /// <returns><typeparamref name="TNext"/></returns>
    public TNext Descending( string columnName )
    {
        _builder.Add(columnName, KeyWords.DESC);
        return _next;
    }
}
