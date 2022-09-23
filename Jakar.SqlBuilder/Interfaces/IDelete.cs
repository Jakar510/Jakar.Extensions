#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


public interface IDelete : IFromSyntax<IDeleteChain>
{
    /// <summary>
    /// Deletes all rows and returns to root.
    /// </summary>
    /// <returns><see cref="ISqlBuilderRoot"/></returns>
    public IDeleteChain All();

    /// <summary>
    /// Deletes all rows of <paramref name="columnName"/> and returns to root.
    /// </summary>
    /// <returns><see cref="ISqlBuilderRoot"/></returns>
    public ISqlBuilderRoot Column( string columnName );
}



public interface IDeleteChain : IChainEnd<ISqlBuilderRoot>
{
    /// <summary>
    /// Starts a <see cref="IWhere"/> clause
    /// </summary>
    /// <returns><see cref="IWhere"/></returns>
    public IWhere Where();
}