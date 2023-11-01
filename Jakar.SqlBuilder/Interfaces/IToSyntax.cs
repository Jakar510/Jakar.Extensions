namespace Jakar.SqlBuilder.Interfaces;


public interface IToSyntax<out TNext>
{
    /// <summary> Uses the passed <paramref name="tableName"/> </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext To( string tableName );

    /// <summary>
    ///     <para>
    ///         <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     </para>
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext To<T>( T obj );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext To<T>() where T : class;
}
