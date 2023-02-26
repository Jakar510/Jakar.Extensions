// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
public struct KeyGenerator : IEnumerator<Guid>
{
    private readonly List<Guid> _keys = new();
    private          int        _index = -1;


    public int         Count   => _keys.Count;
    public Guid        Current => _keys[_index];
    object IEnumerator.Current => Current;


    public KeyGenerator( IEnumerable<Guid> keys ) => _keys.AddRange( keys );


    public void Dispose() => _keys.Clear();
    public void Reset() => _index = -1;
    public bool MoveNext() => ++_index < Count;


    public static KeyGenerator Create<TValue>( IEnumerable<TValue> records ) where TValue : ITableRecord
    {
        var dictionary = new SortedDictionary<DateTimeOffset, Guid>( ValueSorter<DateTimeOffset>.Default );
        foreach ( TValue value in records ) { dictionary.Add( value.DateCreated, value.ID ); }

        return new KeyGenerator( dictionary.Values );
    }
}
