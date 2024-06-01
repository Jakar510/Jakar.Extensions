// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
public record struct KeyGenerator<TRecord> : IEnumerator<RecordID<TRecord>>, IEnumerable<RecordID<TRecord>>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private readonly ReadOnlyMemory<RecordPair<TRecord>> _pairs;
    private          int                                 _index = -1;


    public readonly RecordID<TRecord>  Current => _pairs.Span[_index].ID;
    readonly        object IEnumerator.Current => Current;
    public readonly bool               IsEmpty => _pairs.IsEmpty;


    public KeyGenerator( scoped in ReadOnlyMemory<RecordPair<TRecord>>           pairs ) => _pairs = pairs;
    public static implicit operator KeyGenerator<TRecord>( RecordPair<TRecord>[] pairs ) => new(pairs);


    public   void                                                          Dispose()       => this = default;
    public   void                                                          Reset()         => _index = -1;
    public   bool                                                          MoveNext()      => _pairs.IsEmpty is false && ++_index < _pairs.Length;
    readonly IEnumerator<RecordID<TRecord>> IEnumerable<RecordID<TRecord>>.GetEnumerator() => this;
    readonly IEnumerator IEnumerable.                                      GetEnumerator() => this;


    public static KeyGenerator<TRecord> Create( RecordPair<TRecord>[]            records ) => new(records.Sorted());
    public static KeyGenerator<TRecord> Create( IEnumerable<RecordPair<TRecord>> records ) => Create( records.ToArray() );
    public static KeyGenerator<TRecord> Create( IEnumerable<TRecord>             records ) => Create( records.Select( static x => x.ToPair() ) );
}
