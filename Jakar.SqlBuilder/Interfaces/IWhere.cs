#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


public interface IWhere
{
    /// <summary>
    /// Filters WHERE <paramref name="columnName"/> = "<paramref name="obj"/>"
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="obj">The Target Value</param>
    /// <typeparam name="T">The type being passed</typeparam>
    /// <returns></returns>
    public ISqlBuilderRoot Filter<T>( string columnName, T obj );

    /// <summary>
    /// filters WHERE <paramref name="condition"/>  is "<see langword="true"/>"
    /// </summary>
    /// <returns></returns>
    public ISqlBuilderRoot Filter( string condition );


    /// <summary>
    /// adds WHERE EXISTS
    /// </summary>
    /// <returns></returns>
    public ISqlBuilderRoot Exists();


    /// <summary>
    /// Begins a WHERE condition chain starting with <paramref name="columnName"/> = "<paramref name="target"/>"
    /// </summary>
    /// <example>
    /// 
    /// </example>
    /// <param name="columnName"></param>
    /// <param name="target">The Target Value</param>
    /// <typeparam name="T">The type being passed</typeparam>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain Chain<T>( string columnName, T target );

    /// <summary>
    /// Begins a WHERE condition chain starting with <paramref name="condition"/> is "<see langword="true"/>"
    /// </summary>
    /// <example>
    /// 
    /// </example>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain Chain( string condition );

    /// <summary>
    /// Begins a WHERE condition chain starting with <paramref name="columnName"/>
    /// </summary>
    /// <example>
    /// 
    /// </example>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain Between( string columnName );


    /// <summary>
    /// Checks if the <paramref name="columnName"/> is <see langword="null"/>
    /// </summary>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhere IsNull( string columnName );

    /// <summary>
    /// Checks if the <paramref name="columnName"/> is not <see langword="null"/>
    /// </summary>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhere IsNotNull( string columnName );


    /// <summary>
    /// Checks if the column's value matches the <paramref name="pattern"/>.
    /// </summary>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhere Like( string pattern );


    /// <summary>
    /// <para>
    /// WHERE <paramref name="columnName"/> IN (condition1, condition2, ...);
    /// </para>
    /// <para>
    /// WHERE <paramref name="columnName"/> IN (SELECT STATEMENT);
    /// </para>
    /// </summary>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereInChain In( string columnName );

    /// <summary>
    /// <para>
    /// WHERE <paramref name="columnName"/> IN (<paramref name="conditions"/>)
    /// </para>
    /// </summary>
    /// <returns><see cref="ISqlBuilderRoot"/></returns>
    public ISqlBuilderRoot In( string columnName, params string[] conditions );


    /// <summary>
    /// <para>
    /// WHERE <paramref name="columnName"/> IN (condition1, condition2, ...);
    /// </para>
    /// <para>
    /// WHERE <paramref name="columnName"/> IN (SELECT STATEMENT);
    /// </para>
    /// </summary>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereInChain NotIn( string columnName );

    /// <summary>
    /// <para>
    /// WHERE <paramref name="columnName"/> IN (<paramref name="conditions"/>)
    /// </para>
    /// </summary>
    /// <returns><see cref="ISqlBuilderRoot"/></returns>
    public ISqlBuilderRoot NotIn( string columnName, params string[] conditions );
}



// public interface IWhereExists : IChainEnd<IWhere>
// {
// 	public ISqlBuilderRoot Simple();
//
// 	public ISqlBuilderRoot Sub();
// }



public interface IWhereOperator : IComparators<IWhere>
{
    public ISqlBuilderRoot Any();

    public ISqlBuilderRoot All();
}



public interface IWhereInChain
{
    /// <summary>
    /// Ends chain and return to parent syntax.
    /// </summary>
    /// <returns><see cref="IWhereInChain"/></returns>
    public IWhereInChain Next();


    /// <summary>
    /// Ends chain and return to root.
    /// </summary>
    /// <returns><see cref="ISqlBuilderRoot"/></returns>
    public ISqlBuilderRoot Done();


    /// <summary>
    /// 
    /// </summary>
    /// <returns><see cref="ISelector"/></returns>
    public ISelectorLoop<IWhereInChain> Select();


    // /// <summary>
    // /// 
    // /// </summary>
    // /// <returns><see cref="ISelector"/></returns>
    // public ISelectorLoop<IWhereInChain> Select( string columnName );
}



public interface ISelectorLoop<out TNext> : IFromSyntax<TNext>, IAggregateFunctions<ISelectorLoop<TNext>> { }



public interface IWhereChain : IChainEnd<ISqlBuilderRoot>, INextChain<IWhere>
{
    /// <summary>
    /// joins the chain with a AND and <paramref name="columnName"/> = "<paramref name="obj"/>"
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="obj">The Target Value</param>
    /// <typeparam name="T">The type being passed</typeparam>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain And<T>( T obj, string columnName );


    /// <summary>
    /// joins the chain with a AND and <paramref name="condition"/> is "<see langword="true"/>"
    /// </summary>
    /// <param name="condition"></param>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain And( string condition );


    /// <summary>
    /// joins the chain with a OR and <paramref name="columnName"/> = "<paramref name="obj"/>"
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="obj">The Target Value</param>
    /// <typeparam name="T">The type being passed</typeparam>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain Or<T>( T obj, string columnName );

    /// <summary>
    /// joins the chain with a AND and <paramref name="condition"/> is "<see langword="true"/>"
    /// </summary>
    /// <param name="condition"></param>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain Or( string condition );


    /// <summary>
    /// joins the chain with a NOT and <paramref name="columnName"/> = "<paramref name="obj"/>"
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="obj">The Target Value</param>
    /// <typeparam name="T">The type being passed</typeparam>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain Not<T>( T obj, string columnName );

    /// <summary>
    /// joins the chain with a AND and <paramref name="condition"/> is "<see langword="true"/>"
    /// </summary>
    /// <param name="condition"></param>
    /// <returns><see cref="IWhereChain"/></returns>
    public IWhereChain Not( string condition );
}