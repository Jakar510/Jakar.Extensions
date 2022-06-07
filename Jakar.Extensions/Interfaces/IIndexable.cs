#nullable enable
namespace Jakar.Extensions.Interfaces;


public interface IIndexable<T> : ICollection<T>
{
    public T this[ int index ] { get; set; }
}
