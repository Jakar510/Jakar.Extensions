// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

using System.Linq;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
public struct KeyGenerator : IEnumerator<Guid>, IEnumerable<Guid>
{
    private readonly ReadOnlyMemory<RecordPair> _pairs;
    private          int                        _index = -1;


    public bool IsEmpty => _pairs.IsEmpty;

    public Guid Current => _pairs.Span[_index]
                                 .ID;

    object IEnumerator.Current => Current;


    public KeyGenerator( ReadOnlyMemory<RecordPair>            pairs ) => _pairs = pairs;
    public static implicit operator KeyGenerator( RecordPair[] pairs ) => new(pairs);


    public void Dispose() => this = default;
    public void Reset() => _index = -1;
    public bool MoveNext() => !_pairs.IsEmpty && ++_index < _pairs.Length;
    IEnumerator<Guid> IEnumerable<Guid>.GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;


    public static KeyGenerator Create<TValue>( IEnumerable<TValue> records ) where TValue : IRecordPair =>
        new(records.Select( x => new RecordPair( x.ID, x.DateCreated ) )
                   .ToArray());
}
