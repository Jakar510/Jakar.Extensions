#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


public interface INextChain<out TNext>
{
    /// <summary>
    /// Ends chain and return to parent syntax.
    /// </summary>
    /// <returns><typeparamref name="TNext"/></returns>
    public TNext Next();
}