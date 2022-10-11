// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  9:31 AM

#nullable enable
using Jakar.Extensions;



namespace Jakar.SqlBuilder;


public struct AggregateFunctionsBuilder<TNext> where TNext : struct
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


    /// <summary>
    ///     Return all columns (*)
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext All()
    {
        _builder.AggregateFunction();
        return _next;
    }

    /// <summary>
    ///     return only distinct (different) values.
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Distinct()
    {
        _builder.AggregateFunction( KeyWords.DISTINCT );
        return _next;
    }


    /// <summary>
    ///     Return the average value of values in all columns
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Average()
    {
        _builder.AggregateFunction( KeyWords.AVERAGE );
        return _next;
    }
    /// <summary>
    ///     Return the average value of values in
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Average( string columnName )
    {
        _builder.AggregateFunction( KeyWords.AVERAGE, columnName );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName(System.Type)" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Average<T>( string columnName )
    {
        _builder.AggregateFunction( KeyWords.AVERAGE, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Average<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( KeyWords.AVERAGE, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary>
    ///     Return the total value of values in all columns
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Sum()
    {
        _builder.AggregateFunction( KeyWords.SUM );
        return _next;
    }
    /// <summary>
    ///     Return the total value of values in
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Sum( string columnName )
    {
        _builder.AggregateFunction( KeyWords.SUM, columnName );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Sum<T>( string columnName )
    {
        _builder.AggregateFunction( KeyWords.SUM, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Sum<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( KeyWords.SUM, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary>
    ///     Return count of values in all columns
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Count()
    {
        _builder.AggregateFunction( KeyWords.COUNT );
        return _next;
    }
    /// <summary>
    ///     Return count of values in
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Count( string columnName )
    {
        _builder.AggregateFunction( KeyWords.COUNT, columnName );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Count<T>( string columnName )
    {
        _builder.AggregateFunction( KeyWords.COUNT, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Count<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( KeyWords.COUNT, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary>
    ///     Return the minimum value of values in all columns
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Min()
    {
        _builder.AggregateFunction( KeyWords.MIN );
        return _next;
    }
    /// <summary>
    ///     Return the minimum value of values in
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Min( string columnName )
    {
        _builder.AggregateFunction( KeyWords.MIN, columnName );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Min<T>( string columnName )
    {
        _builder.AggregateFunction( KeyWords.MIN, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Min<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( KeyWords.MIN, obj.GetName( columnName ) );
        return _next;
    }


    /// <summary>
    ///     Return the maximum value of values in all columns
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Max()
    {
        _builder.AggregateFunction( KeyWords.MAX );
        return _next;
    }
    /// <summary>
    ///     Return the maximum value of values in
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Max( string columnName )
    {
        _builder.AggregateFunction( KeyWords.MAX, columnName );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Max<T>( string columnName )
    {
        _builder.AggregateFunction( KeyWords.MAX, columnName.GetName<T>() );
        return _next;
    }
    /// <summary>
    ///     <para>
    ///         Uses the
    ///         <see cref = "Type" />
    ///         of
    ///         <typeparamref name = "T" />
    ///         to get the table_name using
    ///         <see cref = "DapperTableExtensions.GetTableName{T}" />
    ///     </para>
    ///     Return the maximum value of values in table_name.
    ///     <paramref name = "columnName" />
    /// </summary>
    /// <returns>
    ///     <typeparamref name = "TNext" />
    /// </returns>
    public TNext Max<T>( T obj, string columnName )
    {
        _builder.AggregateFunction( KeyWords.MAX, obj.GetName( columnName ) );
        return _next;
    }
}
