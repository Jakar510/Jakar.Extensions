// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
public sealed class KeyGenerator<TValue> : IEnumerator<long>
{
    private readonly IComparer<long> _comparer;
    private readonly IReadOnlyDictionary<long, TValue> _dictionary;
    private readonly List<long> _keys = new();
    private int _index = -1;
    public int Count => _keys.Count;
    public long Current => _keys[_index];
    object IEnumerator.Current => Current;


    public KeyGenerator(IReadOnlyDictionary<long, TValue> dictionary) : this( dictionary, Comparer<long>.Default ) { }
    public KeyGenerator(IReadOnlyDictionary<long, TValue> dictionary, IComparer<long> comparer)
    {
        _dictionary = dictionary;
        _comparer = comparer;
        Reset();
    }
    public void Dispose() => _keys.Clear();


    public bool MoveNext() => ++_index < Count;
    public void Reset()
    {
        _index = -1;
        _keys.Clear();
        _keys.EnsureCapacity( _dictionary.Count );
        _keys.AddRange( _dictionary.Keys );
        _keys.Sort( _comparer );
    }
}
