namespace Jakar.SqlBuilder.Interfaces;


public interface IToSyntax<out TNext>
{
    /// <summary> Uses the passed <paramref name="tableName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext To( string tableName );

    /// <summary>
    ///     <para>
    ///         <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     </para>
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext To<TValue>( TValue obj );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext To<TValue>()
        where TValue : class;
}
