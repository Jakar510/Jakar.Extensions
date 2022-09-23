// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
public class KeyGenerator<TKey, TValue> : IEnumerator<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
{
    private readonly IReadOnlyDictionary<TKey, TValue> _dictionary;
    private readonly IComparer<TKey>                   _comparer;
    private readonly List<TKey>                        _keys  = new();
    private          int                               _index = -1;
    public           TKey                              Current => _keys[_index];
    object IEnumerator.                                Current => Current;
    public int                                         Count   => _keys.Count;


    public KeyGenerator( IReadOnlyDictionary<TKey, TValue> dictionary ) : this(dictionary, Comparer<TKey>.Default) { }
    public KeyGenerator( IReadOnlyDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer )
    {
        _dictionary = dictionary;
        _comparer   = comparer;
        Reset();
    }
    public void Dispose() => _keys.Clear();


    public bool MoveNext() => ++_index < Count;
    public void Reset()
    {
        _index = -1;
        _keys.Clear();
        _keys.EnsureCapacity(_dictionary.Count);
        _keys.AddRange(_dictionary.Keys);
        _keys.Sort(_comparer);
    }
}
