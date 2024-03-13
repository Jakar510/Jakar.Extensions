namespace Jakar.SqlBuilder.Interfaces;


public interface IUpdate : IToSyntax<IUpdateChain> { }



public interface IUpdateChain : IChainEnd<ISqlBuilderRoot>
{
    /// <summary>
    ///     <para> Sets the value of <paramref name="columnName"/> to <paramref name="obj"/> </para>
    /// </summary>
    /// <typeparam name="T"> The type being passed </typeparam>
    /// <returns>
    ///     <see cref="ISqlBuilderRoot"/>
    /// </returns>
    public IUpdateChain Set<T>( string columnName, T obj );

    /// <summary> Starts a <see cref="IWhere"/> clause </summary>
    /// <returns>
    ///     <see cref="IWhere"/>
    /// </returns>
    public IWhere Where();
}
