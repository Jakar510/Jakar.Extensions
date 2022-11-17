#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


public interface IComparators<out TNext>
{
    /// <summary>
    ///     <para> Continues chain and goes to <typeparamref name="TNext"/> . </para>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext AboveOrBelow();

    /// <summary>
    ///     <para> Continues chain and goes to <typeparamref name="TNext"/> . </para>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Equal();
    /// <summary>
    ///     <para> Continues chain and goes to <typeparamref name="TNext"/> . </para>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext Greater();

    /// <summary>
    ///     <para> Continues chain and goes to <typeparamref name="TNext"/> . </para>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext GreaterOrEqual();

    /// <summary>
    ///     <para> Continues chain and goes to <typeparamref name="TNext"/> . </para>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext LessThan();

    /// <summary>
    ///     <para> Continues chain and goes to <typeparamref name="TNext"/> . </para>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext LessThanOrEqual();

    /// <summary>
    ///     <para> Continues chain and goes to <typeparamref name="TNext"/> . </para>
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TNext"/>
    /// </returns>
    public TNext NotEqual();
}
