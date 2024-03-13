namespace Jakar.Database;


[Serializable]
public sealed class RecordCollection<TRecord> : IReadOnlyList<TRecord>
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private readonly List<TRecord> _records = new();
    public           int           Count => _records.Count;


    public TRecord this[ int index ] => _records[index];


    public RecordCollection() : base() { }
    public RecordCollection( params TRecord[]     items ) : this() => Add( items );
    public RecordCollection( IEnumerable<TRecord> items ) : this() => Add( items );


    public RecordCollection<TRecord> Add( params TRecord[] records ) => Add( records.AsEnumerable() );
    public RecordCollection<TRecord> Add( IEnumerable<TRecord> records )
    {
        foreach ( TRecord record in records ) { Add( record ); }

        return this;
    }
    public RecordCollection<TRecord> Add( TRecord item )
    {
        if ( item.IsValidID() )
        {
            _records.Add( item );
            return this;
        }


        _records.Add( item.NewID( Guid.NewGuid() ) );

        return this;
    }


    public IEnumerator<TRecord> GetEnumerator() => _records.GetEnumerator();
    IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
}
