#nullable enable
using Jakar.Extensions;



namespace Jakar.SqlBuilder.Interfaces;


public interface IJoin : IChainEnd<ISqlBuilderRoot>
{
    public IJoin Full( string  columnName );
    public IJoin Inner( string columnName );
    public IJoin Left( string  columnName );
    public IJoin Right( string columnName );


    /// <summary> Starts a JOIN chain </summary>
    /// <returns>
    ///     <see cref="IJoinChain"/>
    /// </returns>
    public IJoinChain On();
}



public interface IJoinChain : IChainEnd<ISqlBuilderRoot>
{
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChainMiddle Left<T>( string columnName );

    /// <summary>
    ///     <para> Uses the <paramref name="obj"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="obj"> </param>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChainMiddle Left<T>( T obj, string columnName );


    /// <summary> </summary>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChainMiddle Left( string columnName );
}



public interface IJoinChainMiddle : IComparators<IJoinChainRight> { }



public interface IJoinChainRight
{
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChain Right<T>( string columnName );

    /// <summary>
    ///     <para> Uses the <paramref name="obj"/> to get the table_name using <see cref="TableExtensions.GetTableName{T}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="obj"> </param>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChain Right<T>( T obj, string columnName );


    /// <summary> </summary>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChain Right( string columnName );
}
