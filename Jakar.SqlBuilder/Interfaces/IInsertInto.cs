#nullable enable
using Jakar.Extensions;



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
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(System.Type)"/> </para>
    ///     Starts a <see cref="IDataInsert"/> chain.
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public IDataInsert In<T>() where T : class;

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/> </para>
    ///     Starts a <see cref="IDataInsert"/> chain.
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public IDataInsert In<T>( T obj ) where T : class;


    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/> </para>
    ///     Uses <see cref="System.Reflection"/> to determine the column names and its values. Uses public properties only.
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public ISqlBuilderRoot Into<T>( string tableName, T obj );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/> </para>
    ///     Also uses <see cref="System.Reflection"/> to determine the column names and its values.
    ///     <para> Checks public properties only. </para>
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public ISqlBuilderRoot Into<T>( T obj ) where T : class;

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/> </para>
    ///     Also uses <see cref="System.Reflection"/> to determine the column names and its values.
    ///     <para> Checks public properties only. </para>
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public ISqlBuilderRoot Into<T>( IEnumerable<T> obj ) where T : class;
}



public interface IDataInsert : IChainEnd<ISqlBuilderRoot>
{
    public IDataInsert With<T>( string columnName, T data );
    public IDataInsert With<T>( T      data );
}
