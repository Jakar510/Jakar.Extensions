using System.Collections.ObjectModel;



namespace Jakar.Database;


[Serializable]
public class RowCollection<TRecord> : ObservableCollection<TRecord> where TRecord : BaseRecord, IUniqueID<long>
{
    public RowCollection() : base() { }
    public RowCollection( params TRecord[]     items ) : base(items) { }
    public RowCollection( IEnumerable<TRecord> items ) : base(items) { }


    public new void Add( TRecord item )
    {
        if ( item.IsValidRowID() )
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
