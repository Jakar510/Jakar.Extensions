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
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="TValue"> </typeparam>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChainMiddle Left<TValue>( string columnName );

    /// <summary>
    ///     <para> Uses the <paramref name="obj"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="TValue"> </typeparam>
    /// <param name="obj"> </param>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChainMiddle Left<TValue>( TValue obj, string columnName );


    /// <summary> </summary>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChainMiddle Left( string columnName );
}



public interface IJoinChainMiddle : IComparators<IJoinChainRight> { }



public interface IJoinChainRight
{
    /// <summary>
    ///     <para> Uses the <see cref="Type"/> of <typeparamref name="TValue"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="TValue"> </typeparam>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChain Right<TValue>( string columnName );

    /// <summary>
    ///     <para> Uses the <paramref name="obj"/> to get the table_name using <see cref="TableExtensions.GetTableName{TValue}"/> </para>
    /// 
    /// </summary>
    /// <typeparam name="TValue"> </typeparam>
    /// <param name="obj"> </param>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChain Right<TValue>( TValue obj, string columnName );


    /// <summary> </summary>
    /// <param name="columnName"> </param>
    /// <returns> </returns>
    public IJoinChain Right( string columnName );
}
