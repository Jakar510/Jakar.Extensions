#nullable enable
namespace Jakar.Extensions;


public interface IReadOnlyIndexable<T> : ICollection<T>
{
    public T this[ int index ] { get; }
}
