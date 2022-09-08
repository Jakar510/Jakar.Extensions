using System.Collections.ObjectModel;



namespace Jakar.Database;


[Serializable]
public sealed class RecordCollection<TRecord, TID> : ObservableCollection<TRecord>, IDisposable where TRecord : BaseRecord, IUniqueID<TID>
                                                                                                where TID : struct, IComparable<TID>, IEquatable<TID>
{
    private Counter<TID> _counter;

    
    public RecordCollection( Counter<TID> counter ) : base() => _counter = counter;
    public RecordCollection( Counter<TID> counter, params TRecord[]     items ) : this(counter) => Add(items);
    public RecordCollection( Counter<TID> counter, IEnumerable<TRecord> items ) : this(counter) => Add(items);
    public void Dispose() => _counter.Dispose();


    public void Add( params TRecord[]     items ) => items.ForEach(Add);
    public void Add( IEnumerable<TRecord> items ) => items.ForEach(Add);
    public new void Add( TRecord item )
    {
        if ( item.IsValidID() )
        {
            base.Add(item);
            return;
        }

        _counter.MoveNext();

        base.Add(item with
                 {
                     ID = _counter.Current
                 });
    }
}
