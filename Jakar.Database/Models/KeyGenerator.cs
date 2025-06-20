// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
public record struct KeyGenerator<TClass> : IEnumerator<RecordID<TClass>>, IEnumerable<RecordID<TClass>>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    private readonly ReadOnlyMemory<RecordPair<TClass>> _pairs;
    private          int                                _index = -1;


    public readonly RecordID<TClass>   Current => _pairs.Span[_index].ID;
    readonly        object IEnumerator.Current => Current;
    public readonly bool               IsEmpty => _pairs.IsEmpty;


    public KeyGenerator( scoped in ReadOnlyMemory<RecordPair<TClass>>          pairs ) => _pairs = pairs;
    public static implicit operator KeyGenerator<TClass>( RecordPair<TClass>[] pairs ) => new(pairs);


    public   void                                                        Dispose()       => this = default;
    public   void                                                        Reset()         => _index = -1;
    public   bool                                                        MoveNext()      => _pairs.IsEmpty is false && ++_index < _pairs.Length;
    readonly IEnumerator<RecordID<TClass>> IEnumerable<RecordID<TClass>>.GetEnumerator() => this;
    readonly IEnumerator IEnumerable.                                    GetEnumerator() => this;


    public static KeyGenerator<TClass> Create( RecordPair<TClass>[]            records ) => new(records.Sorted());
    public static KeyGenerator<TClass> Create( IEnumerable<RecordPair<TClass>> records ) => Create( records.ToArray() );
    public static KeyGenerator<TClass> Create( IEnumerable<TClass>             records ) => Create( records.Select( static x => x.ToPair() ) );
}
