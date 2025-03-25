namespace Jakar.SqlBuilder.Interfaces;


public interface IInsertInto
{
    /// <summary>
    ///     <para> Uses the passed <paramref name="tableName"/> </para>
    ///     Starts a <see cref="IDataInsert"/> chain.
    /// </summary>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public IDataInsert In( string tableName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName(System.Type)"/> </para>
    ///     Starts a <see cref="IDataInsert"/> chain.
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public IDataInsert In<TValue>()
        where TValue : class;

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName(Type)"/> </para>
    ///     Starts a <see cref="IDataInsert"/> chain.
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public IDataInsert In<TValue>( TValue obj )
        where TValue : class;


    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName(Type)"/> </para>
    ///     Uses <see cref="System.Reflection"/> to determine the column names and its values. Uses public properties only.
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public ISqlBuilderRoot Into<TValue>( string tableName, TValue obj );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName(Type)"/> </para>
    ///     Also uses <see cref="System.Reflection"/> to determine the column names and its values.
    ///     <para> Checks public properties only. </para>
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public ISqlBuilderRoot Into<TValue>( TValue obj )
        where TValue : class;

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName(Type)"/> </para>
    ///     Also uses <see cref="System.Reflection"/> to determine the column names and its values.
    ///     <para> Checks public properties only. </para>
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public ISqlBuilderRoot Into<TValue>( IEnumerable<TValue> obj )
        where TValue : class;
}



public interface IDataInsert : IChainEnd<ISqlBuilderRoot>
{
    public IDataInsert With<TValue>( string columnName, TValue data );
    public IDataInsert With<TValue>( TValue      data );
}
