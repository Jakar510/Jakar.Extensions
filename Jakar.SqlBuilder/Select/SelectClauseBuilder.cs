#nullable enable
using Jakar.Extensions;



namespace Jakar.SqlBuilder;


public struct SelectClauseBuilder<TNext>
{
    private readonly TNext          _next;
    private          EasySqlBuilder _builder;
    public SelectClauseBuilder( in TNext next, ref EasySqlBuilder builder )
    {
        _next    = next;
        _builder = builder;
    }

    public TNext Done() => _next;


    public EasySqlBuilder From( string tableName, string? alias )
    {
        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM); }

        else { _builder.Add(KeyWords.FROM, tableName, KeyWords.AS, alias); }

        return _builder.NewLine();
    }
    public EasySqlBuilder From<T>( T obj, string? alias )
    {
        if ( obj is null ) { throw new NullReferenceException(nameof(obj)); }

        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, obj.GetTableName()); } // TODO: Bug...?

        else { _builder.Add(KeyWords.FROM, obj.GetName(), KeyWords.AS, alias); }

        return _builder.NewLine();
    }
    public EasySqlBuilder From<T>( string? alias )
    {
        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, typeof(T).GetTableName()); }

        else { _builder.Add(KeyWords.FROM, typeof(T).GetName(), KeyWords.AS, alias); }

        return _builder.NewLine();
    }


    public AggregateFunctionsBuilder<SelectClauseBuilder<TNext>> WithFunction() => new(this, ref _builder);


    /// <summary>
    /// Adds <param name="columnName"></param> to SELECT set
    /// </summary>
    /// <returns><see cref="SelectClauseBuilder"/></returns>
    public SelectClauseBuilder<TNext> Next( string columnName )
    {
        _builder.Add(columnName);
        return this;
    }
    /// <summary>
    /// <para>
    /// Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/>
    /// </para>
    /// Adds table_name.<param name="columnName"></param> to SELECT set
    /// </summary>
    /// <example>SELECT Orders.OrderID, Customers.CustomerName, Orders.OrderDate</example>
    /// <returns><see cref="SelectClauseBuilder"/></returns>
    public SelectClauseBuilder<TNext> Next<T>( string columnName )
    {
        _builder.Add(columnName.GetName<T>());
        return this;
    }
    /// <summary>
    /// <para>
    /// Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/>
    /// </para>
    /// Adds table_name.<param name="columnNames"></param> to SELECT set
    /// </summary>
    /// <example>SELECT Orders.OrderID, Customers.CustomerName, Orders.OrderDate</example>
    /// <returns><see cref="SelectClauseBuilder"/></returns>
    public SelectClauseBuilder<TNext> Next( params string[] columnNames )
    {
        _builder.Begin().AddRange(',', columnNames).End();
        return this;
    }


    public SelectClauseBuilder<TNext> Next<T>( params string[] columnNames )
    {
        _builder.Begin().AddRange<T>(',', columnNames).End();
        return this;
    }


    /// <summary>
    /// Adds <param name="columnNames"></param> to SELECT set and setting it to the <param name="alias"></param> variable
    /// </summary>
    /// <returns><see cref="SelectClauseBuilder"/></returns>
    public SelectClauseBuilder<TNext> NextAs( string alias, params string[] columnNames )
    {
        _builder.Begin().AddRange(',', columnNames).End().Add(KeyWords.AS, alias);
        return this;
    }

    /// <summary>
    /// <para>
    /// Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/>
    /// </para>
    /// Adds table_name.columnName to SELECT set and setting it to the <param name="alias"></param> variable
    /// </summary>
    /// <returns><see cref="SelectClauseBuilder"/></returns>
    public SelectClauseBuilder<TNext> NextAs<T>( string alias, params string[] columnNames )
    {
        _builder.Begin().AddRange<T>(',', columnNames).End().Add(KeyWords.AS, alias);
        return this;
    }
}
