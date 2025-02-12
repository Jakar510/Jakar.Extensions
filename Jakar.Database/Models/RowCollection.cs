namespace Jakar.Database;


[Serializable]
public class RecordCollection<TRecord>( int capacity = Buffers.DEFAULT_CAPACITY ) : IReadOnlyList<TRecord>
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private readonly List<TRecord> _records = new(capacity);


    public int Count => _records.Count;
    public TRecord this[ int index ] => _records[index];


    public RecordCollection( params ReadOnlySpan<TRecord> values ) : this() => Add( values );
    public RecordCollection( IEnumerable<TRecord>         values ) : this() => Add( values );


    public RecordCollection<TRecord> Add( params ReadOnlySpan<TRecord> values )
    {
        foreach ( TRecord value in values ) { Add( value ); }

        return this;
    }
    public RecordCollection<TRecord> Add( IEnumerable<TRecord> values )
    {
        foreach ( TRecord value in values ) { Add( value ); }

        return this;
    }
    public RecordCollection<TRecord> Add( TRecord value )
    {
        if ( value.IsValidID() )
        {
            _records.Add( value );
            return this;
        }


        _records.Add( value.NewID( RecordID<TRecord>.New() ) );

        return this;
    }


    public IEnumerator<TRecord> GetEnumerator() => _records.GetEnumerator();
    IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
}
