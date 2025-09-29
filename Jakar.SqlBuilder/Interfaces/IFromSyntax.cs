namespace Jakar.SqlBuilder.Interfaces;


public interface IFromSyntax<out TNext>
{
    /// <summary> Uses the passed <paramref name="tableName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext From( string tableName, string? alias = null );

    /// <summary>
    ///     <para>
    ///         <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     </para>
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext From<TValue>( TValue obj, string? alias = null );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    /// </summary>
    /// <typeparam name="TValue"> The type being passed </typeparam>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext From<TValue>( string? alias = null )
        where TValue : class;
}
