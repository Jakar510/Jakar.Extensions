namespace Jakar.Extensions.Interfaces;


public interface IReadOnlyIndexable<T> : ICollection<T>
{
    public T this[ int index ] { get; }
}
