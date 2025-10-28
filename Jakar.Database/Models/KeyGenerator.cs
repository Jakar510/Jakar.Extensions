// Jakar.Extensions :: Jakar.Database
// 09/02/2022  5:43 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
public record struct KeyGenerator<TSelf> : IEnumerator<RecordID<TSelf>>, IEnumerable<RecordID<TSelf>>
    where TSelf : class, ITableRecord<TSelf>
{
    private readonly ReadOnlyMemory<RecordPair<TSelf>> __pairs;
    private          int                               __index = -1;


    public readonly RecordID<TSelf>    Current => __pairs.Span[__index].ID;
    readonly        object IEnumerator.Current => Current;
    public readonly bool               IsEmpty => __pairs.IsEmpty;


    public KeyGenerator( scoped in ReadOnlyMemory<RecordPair<TSelf>>         pairs ) => __pairs = pairs;
    public static implicit operator KeyGenerator<TSelf>( RecordPair<TSelf>[] pairs ) => new(pairs);


    public   void                                                      Dispose()       => this = default;
    public   void                                                      Reset()         => __index = -1;
    public   bool                                                      MoveNext()      => !__pairs.IsEmpty && ++__index < __pairs.Length;
    readonly IEnumerator<RecordID<TSelf>> IEnumerable<RecordID<TSelf>>.GetEnumerator() => this;
    readonly IEnumerator IEnumerable.                                  GetEnumerator() => this;


    public static KeyGenerator<TSelf> Create( RecordPair<TSelf>[]            records ) => new(records.Sorted());
    public static KeyGenerator<TSelf> Create( IEnumerable<RecordPair<TSelf>> records ) => Create(records.ToArray());
    public static KeyGenerator<TSelf> Create( IEnumerable<TSelf>             records ) => Create(records.Select(static x => x.ToPair()));
}
