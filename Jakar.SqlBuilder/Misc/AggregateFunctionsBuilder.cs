// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  9:31 AM

namespace Jakar.SqlBuilder;


public struct AggregateFunctionsBuilder<TNext>
    where TNext : struct
{
    private readonly TNext          _next;
    private          EasySqlBuilder _builder;


    public AggregateFunctionsBuilder( in TNext next, ref EasySqlBuilder builder )
    {
        _next    = next;
        _builder = builder.Begin();
    }


    public TNext None()
    {
        _builder.VerifyParentheses();
        return _next;
    }


    /// <summary> Return all columns (*) </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext All()
    {
        _builder.AggregateFunction();
        return _next;
    }

    /// <summary> return only distinct (different) values. </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Distinct()
    {
        _builder.AggregateFunction( DISTINCT );
        return _next;
    }


    /// <summary> Return the average value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average()
    {
        _builder.AggregateFunction( AVERAGE );
        return _next;
    }
    /// <summary> Return the average value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average( string columnName )
    {
        _builder.AggregateFunction( AVERAGE, columnName );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName(System.Type)"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average<T>( string columnName )
    {
        _builder.AggregateFunction( AVERAGE, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Average<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( AVERAGE, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary> Return the total value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum()
    {
        _builder.AggregateFunction( SUM );
        return _next;
    }
    /// <summary> Return the total value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum( string columnName )
    {
        _builder.AggregateFunction( SUM, columnName );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum<T>( string columnName )
    {
        _builder.AggregateFunction( SUM, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Sum<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( SUM, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary> Return count of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count()
    {
        _builder.AggregateFunction( COUNT );
        return _next;
    }
    /// <summary> Return count of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count( string columnName )
    {
        _builder.AggregateFunction( COUNT, columnName );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count<T>( string columnName )
    {
        _builder.AggregateFunction( COUNT, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Count<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( COUNT, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary> Return the minimum value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min()
    {
        _builder.AggregateFunction( MIN );
        return _next;
    }
    /// <summary> Return the minimum value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min( string columnName )
    {
        _builder.AggregateFunction( MIN, columnName );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min<T>( string columnName )
    {
        _builder.AggregateFunction( MIN, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Min<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( MIN, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary> Return the maximum value of values in all columns </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max()
    {
        _builder.AggregateFunction( MAX );
        return _next;
    }
    /// <summary> Return the maximum value of values in <paramref name="columnName"/> </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max( string columnName )
    {
        _builder.AggregateFunction( MAX, columnName );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max<T>( string columnName )
    {
        _builder.AggregateFunction( MAX, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    ///     Return the maximum value of values in table_name. <paramref name="columnName"/>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Max<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( MAX, obj.GetName( columnName ) );
        return _next;
    }
}
