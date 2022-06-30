#nullable enable
namespace Jakar.Extensions;


public interface IIndexable<T> : ICollection<T>
{
    public T this[ int index ] { get; set; }
}
