// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
public sealed class KeyGenerator<TValue> : IEnumerator<string> where TValue : ITableRecord
{
    private readonly List<string>        _keys  = new();
    private          int                 _index = -1;
    private readonly IEnumerable<TValue> _records;


    public int         Count   => _keys.Count;
    public string      Current => _keys[_index];
    object IEnumerator.Current => Current;


    public KeyGenerator( IEnumerable<TValue> records )
    {
        _records = records;
        Reset();
    }
    public void Dispose() => _keys.Clear();


    public bool MoveNext() => ++_index < Count;
    public void Reset()
    {
        _index = -1;
        _keys.Clear();
        var dictionary = new SortedDictionary<DateTimeOffset, string>( ValueSorter<DateTimeOffset>.Default );
        foreach ( TValue value in _records ) { dictionary.Add( value.DateCreated, value.ID ); }

        _keys.AddRange( dictionary.Values );
    }
}
