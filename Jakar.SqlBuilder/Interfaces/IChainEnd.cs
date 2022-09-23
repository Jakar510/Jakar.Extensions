#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


public interface IChainEnd<out TNext>
{
    /// <summary>
    /// Ends chain and goes to <typeparamref name="TNext"/>.
    /// </summary>
    /// <returns><typeparamref name="TNext"/></returns>
    public TNext Done();
}