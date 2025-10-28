// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  9:31 AM

namespace Jakar.SqlBuilder;


public struct AggregateFunctionsBuilder<TNext>
    where TNext : struct
{
    private readonly TNext          __next;
    private          EasySqlBuilder __builder;


    public AggregateFunctionsBuilder( in TNext next, ref EasySqlBuilder builder )
    {
        __next    = next;
        __builder = builder.Begin();
    }


    public TNext None()
    {
        __builder.VerifyParentheses();
        return __next;
    }


    /// <summary> Return all columns (*) </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext All()
    {
        __builder.AggregateFunction();
        return __next;
    }

    /// <summary> return only distinct (different) values. </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Distinct()
    {
        __builder.AggregateFunction(DISTINCT);
        return __next;
    }


    /// <summary> Return the average value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average()
    {
        __builder.AggregateFunction(AVERAGE);
        return __next;
    }
    /// <summary> Return the average value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average( string columnName )
    {
        __builder.AggregateFunction(AVERAGE, columnName);
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName(System.Type)"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average<TValue>( string columnName )
    {
        __builder.AggregateFunction(AVERAGE, columnName.GetName<TValue>());
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average<TValue>( TValue obj, string columnName )
    {
        __builder.AggregateFunction(AVERAGE, columnName.GetName(obj));
        return __next;
    }


    /// <summary> Return the total value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum()
    {
        __builder.AggregateFunction(SUM);
        return __next;
    }
    /// <summary> Return the total value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum( string columnName )
    {
        __builder.AggregateFunction(SUM, columnName);
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum<TValue>( string columnName )
    {
        __builder.AggregateFunction(SUM, columnName.GetName<TValue>());
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum<TValue>( TValue obj, string columnName )
    {
        __builder.AggregateFunction(SUM, columnName.GetName(obj));
        return __next;
    }


    /// <summary> Return count of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count()
    {
        __builder.AggregateFunction(COUNT);
        return __next;
    }
    /// <summary> Return count of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count( string columnName )
    {
        __builder.AggregateFunction(COUNT, columnName);
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count<TValue>( string columnName )
    {
        __builder.AggregateFunction(COUNT, columnName.GetName<TValue>());
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count<TValue>( TValue obj, string columnName )
    {
        __builder.AggregateFunction(COUNT, columnName.GetName(obj));
        return __next;
    }


    /// <summary> Return the minimum value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min()
    {
        __builder.AggregateFunction(MIN);
        return __next;
    }
    /// <summary> Return the minimum value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min( string columnName )
    {
        __builder.AggregateFunction(MIN, columnName);
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min<TValue>( string columnName )
    {
        __builder.AggregateFunction(MIN, columnName.GetName<TValue>());
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min<TValue>( TValue obj, string columnName )
    {
        __builder.AggregateFunction(MIN, columnName.GetName(obj));
        return __next;
    }


    /// <summary> Return the maximum value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max()
    {
        __builder.AggregateFunction(MAX);
        return __next;
    }
    /// <summary> Return the maximum value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max( string columnName )
    {
        __builder.AggregateFunction(MAX, columnName);
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max<TValue>( string columnName )
    {
        __builder.AggregateFunction(MAX, columnName.GetName<TValue>());
        return __next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max<TValue>( TValue obj, string columnName )
    {
        __builder.AggregateFunction(MAX, columnName.GetName(obj));
        return __next;
    }
}
