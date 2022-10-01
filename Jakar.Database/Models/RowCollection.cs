namespace Jakar.Database;


[Serializable]
public sealed class RecordCollection<TRecord> : ObservableCollection<TRecord> where TRecord : BaseRecord, IUniqueID<long>
{
    public RecordCollection() : base() { }
    public RecordCollection( params TRecord[]     items ) : this() => Add(items);
    public RecordCollection( IEnumerable<TRecord> items ) : this() => Add(items);


    public void Add( params TRecord[]     items ) => items.ForEach(Add);
    public void Add( IEnumerable<TRecord> items ) => items.ForEach(Add);
    public new void Add( TRecord item )
    {
        if ( item.IsValidID() )
        {
            base.Add(item);
            return;
        }

        base.Add(item with
                 {
                     ID = Count
                 });
    }
}
