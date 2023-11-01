namespace Jakar.SqlBuilder.Interfaces;


public interface IAggregateFunctions<out TNext>
{
    /// <summary> Return all columns (*) </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext All();


    /// <summary> Return the average value of values in all columns </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Average();

    /// <summary> Return the average value of values in <paramref name="columnName"/> </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Average( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Average<T>( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Average<T>( T obj, string columnName );


    /// <summary> Return count of values in all columns </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Count();

    /// <summary> Return count of values in <paramref name="columnName"/> </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Count( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Count<T>( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Count<T>( T obj, string columnName );

    /// <summary> return only distinct (different) values. </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Distinct();


    /// <summary> Return the maximum value of values in all columns </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Max();

    /// <summary> Return the maximum value of values in <paramref name="columnName"/> </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Max( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Max<T>( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Max<T>( T obj, string columnName );


    /// <summary> Return the minimum value of values in all columns </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Min();

    /// <summary> Return the minimum value of values in <paramref name="columnName"/> </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Min( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Min<T>( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Min<T>( T obj, string columnName );


    /// <summary> Return the total value of values in all columns </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Sum();

    /// <summary> Return the total value of values in <paramref name="columnName"/> </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Sum( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Sum<T>( string columnName );

    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns> <typeparamref name="TNext"/> </returns>
    public TNext Sum<T>( T obj, string columnName );
}
