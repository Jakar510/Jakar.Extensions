#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


public interface IHaving : IAggregateFunctions<IHaving>, IComparators<IHaving>, IChainEnd<ISqlBuilderRoot>
{
    /// <summary>
    /// <para>
    /// finishes the Having chain with the <paramref name="obj"/>. 
    /// </para>
    /// </summary>
    /// <typeparam name="T">the <see cref="Type"/> of value</typeparam>
    /// <param name="obj">the value being inserted</param>
    /// <returns><see cref="IHaving"/></returns>
    public IHaving Value<T>( T obj );
}